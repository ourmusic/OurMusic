﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using OurMusic.Models;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OurMusic.Hubs
{
    public class RoomHub : Hub
    {
        private OurMusicEntities db = new OurMusicEntities();
        public static UserManager<ApplicationUser> umanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private static Dictionary<string, PublicRoom> rooms = new Dictionary<string, PublicRoom>();
        private static ConcurrentDictionary<string, Tuple<string, string>> connections2usersANDgroups = new ConcurrentDictionary<string, Tuple<string, string>>();


        /**
         * static method called by IHubContext in RoomController
         * when the admin deletes the room, this notifies all current room members
         */
        public static void alertRoomHasBeenDeleted(string roomName)
        {
            var notifyContext = GlobalHost.ConnectionManager.GetHubContext<RoomHub>();
            notifyContext.Clients.Group(roomName).alertRoomHasBeenDeleted();
        }

        
        /// <summary>
        /// Room admin starts the counter.
        /// Also adds the ElapsedEventHandler
        /// </summary>
        /// <param name="seconds">Number of seconds for the first video</param>
        /// Finds the room in the dictionary with key roomName, and starts the countdown.
        /// Initializes the room if it has not been yet.
        public void StartInitCountDown(int seconds, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("In StartInitCountDown");
            PublicRoom clientsRoom;
            if (rooms.TryGetValue(roomName, out clientsRoom))
            {
                System.Diagnostics.Debug.WriteLine("tryGetValue :  " + roomName + " evaluated to true");
                clientsRoom.CountDown(seconds);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("tryGetValue :  " + roomName + " evaluated to false");
                InitRoom(roomName);
                rooms[roomName].CountDown(seconds);

            }
        }

        /// <summary>
        /// Adds a song to the queue
        /// </summary>
        /// <param name="vidTitle">Video Title</param>
        /// <param name="vidUrl">Video URL</param>
        /// <param name="roomName">The room which the video will be played in</param>
        public void addToQueue(string vidTitle, string vidUrl, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("addToQueue(" + vidTitle + ", " + vidUrl + ", " + roomName + ")");
            rooms[roomName].AddToQueue(vidTitle, vidUrl);
        }

        /**
         * User list must constantly be updated, on disconnect events, this notifies clients to remove that user from their user list
         */
        public override Task OnDisconnected(bool stopCalled)
        {

                Tuple<string, string> IdRoomNameTuple;
                if (connections2usersANDgroups.TryRemove(Context.ConnectionId, out IdRoomNameTuple))
                {
                    string userID = IdRoomNameTuple.Item1;
                    string roomName = IdRoomNameTuple.Item2;
                    Clients.Group(roomName).userRemoved(userID);
                }

            return base.OnDisconnected(stopCalled);
        }




        /// <summary>
        /// called by a client, must specify roomName of existing room.
        /// changes the vote score for specified video by voteChange
        /// updates the queue, and then tells all members of that group what to change in their queues
        /// </summary>
        /// <param name="vidTitle">Video Title</param>
        /// <param name="vidUrl">Video URL</param>
        /// <param name="voteChange">Upvote or Downvote</param>
        /// <param name="roomName">The room which the video will be played in</param>
        public void voteByTitleAndUrl(string vidTitle, string vidUrl, int voteChange, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("in voteByTitleAndUrl, roomname = " + roomName + ".");
            int movement = rooms[roomName].voteAndUpdate(vidTitle, vidUrl, voteChange);
            System.Diagnostics.Debug.WriteLine("vote " + vidTitle + ", movement = " + movement);
            Clients.Group(roomName).adjustVotesAndPlacement(vidUrl, voteChange, movement);

        }

        /// <summary>
        /// Removes video from queue
        /// </summary>
        /// <param name="videoTitle">Video to delete</param>
        /// <param name="videoURL">Video URL</param>
        /// <param name="roomName">Room which video persides</param>
        public void deleteVideo(string videoTitle, string videoURL, string roomName)
        {
            rooms[roomName].deleteVideo(videoTitle, videoURL);
            Clients.Group(roomName).deleteVideo(videoURL);
        }

        /// <summary>
        /// called by room administrator's javascript
        /// updates removed user from db, and notifies room that user was removed
        /// the removed user will have to make a separate call to remove himself from the group
        /// </summary>
        /// <param name="uID">User's ID to be removed from the room</param>
        /// <param name="roomName">Room which user will be removed from</param>
        public void removeUser(Guid uID, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("in RemoveUser.  id set to : " + uID + " and roomName set to : " + roomName);
            Clients.Group(roomName).userRemoved(uID.ToString());

            var user = db.People.Find(uID);
            user.activeRoom = null;
            db.SaveChangesAsync();
        }

        /// <summary>
        /// Function called when person leaves
        /// </summary>
        /// <param name="roomName">Room which person is leaving</param>
        /// <returns></returns>
        public Task leaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        /// <summary>
        /// Adds a message to the room
        /// </summary>
        /// <param name="name">User's name</param>
        /// <param name="message">The message to send</param>
        /// <param name="roomName">The room to send the message to</param>
        public void Send(String name, String message, string roomName)
        {
            Clients.Group(roomName).addNewMessageToPage(name, message);
        }


        /// <summary>
        /// Called upon connecting.
        /// Refreshes the queue for the calling client, and updates connections2usersANDgroups dictionary.
        /// </summary>
        /// <param name="roomName">Room to refresh</param>
        /// <param userID="userID">user ID to map to connectionID</param>
        /// <returns></returns>
        public async Task refreshClient(string roomName, string userID)
        {
            await Groups.Add(Context.ConnectionId, roomName);
            connections2usersANDgroups.TryAdd(Context.ConnectionId, new Tuple<string, string>(userID, roomName));
            PublicRoom clientsRoom;
            string jsonOfQueue;
            if (rooms.TryGetValue(roomName, out clientsRoom))
            {
                clientsRoom.updateContext();
                jsonOfQueue = clientsRoom.jsonRoomsQueue();
            }
            else
            {
                InitRoom(roomName);
                jsonOfQueue = rooms[roomName].jsonRoomsQueue();
            }
            
            Clients.Caller.refreshList(jsonOfQueue);

        }


        /// <summary>
        /// Allows the room to know who that a user entered
        /// </summary>
        /// <param name="roomName">Room which user is entering</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="userID">User's ID</param>
        public void announceEntranceToRoom(string roomName, string firstName, string lastName, string userID)
        {
            Clients.OthersInGroup(roomName).addNewUser(firstName, lastName, userID);
        }



        /// <summary>
        /// Initializes the room
        /// </summary>
        /// <param name="guid">The room to initialize</param>
        public void InitRoom(String guid)
        {
            System.Diagnostics.Debug.WriteLine("In InitRoom(" + guid + ")");
            if (!rooms.ContainsKey(guid))
            {
                PublicRoom pub = new PublicRoom(guid);
                rooms.Add(guid, pub);
                System.Diagnostics.Debug.WriteLine("Value added for key = " + guid);
            }


        }

        /// <summary>
        /// Starts the countdown
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="guid"></param>
        public void CountDown(int seconds, String guid)
        {
            System.Diagnostics.Debug.WriteLine("THub CountDown(" + guid + ", " + seconds + ")");
            rooms[guid].CountDown(seconds);
        }


    }
}