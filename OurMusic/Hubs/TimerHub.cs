using System;
using System.Collections.Generic;
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
    //[HubName("timerHub")]
    public class TimerHub : Hub
    {
        private OurMusicEntities db = new OurMusicEntities();
        public static UserManager<ApplicationUser> umanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        
        //private static Timer _timer = new Timer();
        //private static VideoQueue videoQueue = new VideoQueue(true);

        private static Dictionary<string, PublicRoom> rooms = new Dictionary<string, PublicRoom>();

        /// StartInitCountDown COMMENTS OUT OF DATE AS OF 4/20
        /// <summary>
        /// Room admin starts the counter.
        /// Also adds the ElapsedEventHandler
        /// </summary>
        /// <param name="seconds">Number of seconds for the first video</param>
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
        /// Starts the countdown timer for the video to finish.
        /// Used by the play.js file which handles the client-server communication functions
        /// </summary>
        /// <param name="seconds">Video duration</param>
        /**
        public void StartCountDown(int seconds)
        {
            _timer.Interval = (seconds + 2) * 1000;
            _timer.Start();
        }
        **/

        /**
        /// <summary>
        /// Event handler for when the timer is done
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void _timer_Done(Object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            String video = GetNextVideo();
            Clients.All.change(video);

        }

        /// <summary>
        /// This method is incomplete and needs to be work on by Jake and Fidel
        /// It will pull the next video from the queue and add play it.
        /// 
        /// This function gets used by the Timer event handler.
        /// </summary>
        /// <returns>The next video ID in the queue</returns>
        public String GetNextVideo()
        {
            if (videoQueue.getLength() == 0)
            {
                return "Xpe-JoGyPsY";
            }
            Video toPlay = videoQueue.removeFirstVideo();
            return toPlay.getUrl();
        }
        **/


        public void addToQueue(string vidTitle, string vidUrl, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("addToQueue(" + vidTitle + ", " + vidUrl + ", " + roomName + ")");
            rooms[roomName].AddToQueue(vidTitle, vidUrl);
            //PublicRoom.AddToQueue() tells clients about new video
        }

        /**
         * called by a client, must specify roomName of existing room.
         * changes the vote score for specified video by voteChange
         * updates the queue, and then tells all members of that group what to change in their queues
         **/
        public void voteByTitleAndUrl(string vidTitle, string vidUrl, int voteChange, string roomName)
        {
            System.Diagnostics.Debug.WriteLine("in voteByTitleAndUrl, roomname = " + roomName + ".");
            int movement = rooms[roomName].voteAndUpdate(vidTitle, vidUrl, voteChange);
            System.Diagnostics.Debug.WriteLine("vote " + vidTitle + ", movement = " + movement);
            Clients.Group(roomName).adjustVotesAndPlacement(vidUrl, voteChange, movement);
            
        }


        /**
         * This implements the generic JoinRoom function
         * If room already exists, then retrieves that room's queue, and sends back a json of it to requesting client.
         * If room doesn't exist, adds it to dictionary and groups
         **/
        public async Task refreshClient(string roomName)
        {
            System.Diagnostics.Debug.WriteLine("refreshClient : " + roomName);
            await Groups.Add(Context.ConnectionId, roomName);
            PublicRoom clientsRoom;
            string jsonOfQueue;
            if(rooms.TryGetValue(roomName, out clientsRoom)){
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



        //NEW MOTHODS FOR ROOMS
        public void InitRoom(String guid)
        {
            //Need to check if this guid had already been used
            //This part relies on how you implement the data structure, Jake.
            System.Diagnostics.Debug.WriteLine("In InitRoom(" + guid + ")");
            if (!rooms.ContainsKey(guid))
            {
                PublicRoom pub = new PublicRoom(guid);
                rooms.Add(guid, pub);
                System.Diagnostics.Debug.WriteLine("Value added for key = " + guid);
            }

            
        }

        public void CountDown(int seconds, String guid)
        {
            //Find room with guid and start the countdown.
            System.Diagnostics.Debug.WriteLine("THub CountDown(" + guid + ", " + seconds + ")");
            rooms[guid].CountDown(seconds);
        }


























        //Room group methods
        /*public override Task OnConnected()
        {
            return base.OnConnected();
        }


        public Task AddToRoom(string roomName)
        {

            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task RemoveFromRoom(string roomName)
        {

            return Groups.Remove(Context.ConnectionId, roomName);
        }
        */
    }




}