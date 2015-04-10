using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OurMusic.Models
{
    public class VideoQueue
    {
        private LinkedList<Video> videoList;

        public VideoQueue()
        {
            videoList = new LinkedList<Video>();
        }
        public VideoQueue(bool b)
        {
            videoList = new LinkedList<Video>();
            if (b)
            {
                populateTestQueueWithCatVideos();
            }
        }
        public int getLength()
        {
            return videoList.Count;
        }
        public void addVideo(Video v)
        {
            if (!videoList.Contains(v)) videoList.AddLast(v);
        }



        public Video removeFirstVideo()
        {
            Video ret = videoList.First();
            if (ret != null) videoList.RemoveFirst();
            return ret;
        }

        public void removeUnwantedVideo(Video v)
        {
            LinkedListNode<Video> l = videoList.Last;
            while (l.Previous.Value != v) l = l.Previous;
            videoList.Remove(l);

        }
        public void upvote(Video v)
        {
            LinkedListNode<Video> vid = videoList.Find(v);
            vid.Value.upvote();
            checkOrder(vid);
        }

        public void upvoteAt(int i)
        {
            Video vid = videoList.ElementAt(i);
            upvote(vid);
        }

        public void downvote(Video v)
        {
            LinkedListNode<Video> vid = videoList.Find(v);
            vid.Value.downvote();
            checkOrder(vid);
        }

        public void downvoteAt(int i)
        {
            Video vid = videoList.ElementAt(i);
            downvote(vid);
        }



        public void checkOrder(LinkedListNode<Video> vid)
        {
            if ((vid.Next != null) && (vid.Next.Value.getVotes() > vid.Value.getVotes()))
            {
                LinkedListNode<Video> seeker = vid.Next;
                while ((seeker.Next != null) && (seeker.Next.Value.getVotes() > vid.Value.getVotes()))
                {
                    seeker = seeker.Next;
                }
                videoList.Remove(vid);
                videoList.AddAfter(seeker, vid);
            }
            else if ((vid.Previous != null) && (vid.Previous.Value.getVotes() < vid.Value.getVotes()))
            {
                LinkedListNode<Video> seeker = vid.Previous;
                while ((seeker.Previous != null) && (seeker.Previous.Value.getVotes() < vid.Value.getVotes()))
                {
                    seeker = seeker.Previous;
                }
                videoList.Remove(vid);
                videoList.AddBefore(seeker, vid);
            }
        }

        public string jsonQueue()
        {

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            settings.NullValueHandling = NullValueHandling.Ignore;
            //settings.Formatting = Formatting.Indented;


            string json = JsonConvert.SerializeObject(videoList, settings);
            Console.WriteLine(json);
            return json;
        }

        /**
         * used for testing purposes
         * this function just adds a few cat videos to the queue so that the queue isn't empty
         **/
        public void populateTestQueueWithCatVideos()
        {
            Video sailCat = new Video("Cat Jump Fail with Music: Sail by AWOLNATION", "Awf45u6zrP0");
            Video thugCat = new Video("Gato malo :: Thug Life", "UoUEQYjYgf4");
            Video karmaCat = new Video("Cat Owner Instant Karma (original)", "22CrPtjODPY");

            addVideo(sailCat);
            addVideo(thugCat);
            addVideo(karmaCat);
        }

    }
}