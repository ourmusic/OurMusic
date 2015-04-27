using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
using OurMusic.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace OurMusic.Models
{
    public class PublicRoom
    {
        private String _roomID;
        private VideoQueue _queue;
        private static Timer _timer = new Timer();
        private IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<TimerHub>();

        public PublicRoom(String guid)
        {
            _roomID = guid;
            _queue = new VideoQueue(true);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Done);
        }

        private void _timer_Done(Object source, ElapsedEventArgs e)
        {
            _timer.Stop();
            String video = GetNextVideo();
            _context.Clients.Group(_roomID).change(video);
        }

        public void CountDown(int seconds)
        {
            _timer.Interval = (seconds + 2) * 1000;
            _timer.Start();
        }

        public String GetNextVideo()
        {
            if (_queue.getLength() == 0)
            {
                return "Xpe-JoGyPsY";
            }

            Video toPlay = _queue.removeFirstVideo();
            return toPlay.getUrl();
        }


        public void AddToQueue(string vidTitle, string vidUrl)
        {
            Video newVid = new Video(vidTitle, vidUrl);
            _queue.addVideo(newVid);
            _context.Clients.Group(_roomID).addVideo(vidTitle, vidUrl);
        }






    }
}