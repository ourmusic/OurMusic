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

        //Voting on a video returns an integer representing how many places the video moved as a result.  Must have a way to signal an error if that video cannot be found.
        public const int VIDEONOTFOUND = -999;

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

        public int vote(string vidTitle, string vidUrl, int voteChange)
        {
            LinkedListNode<Video> vid = findByTitleAndUrl(vidTitle, vidUrl);
            if (vid == null) return VIDEONOTFOUND;
            vid.Value.vote(voteChange);
            return checkOrder(vid);
        }



        public int checkOrder(LinkedListNode<Video> vid)
        {
            int movement = 0;
            if ((vid.Next != null) && (vid.Next.Value.getVotes() > vid.Value.getVotes()))
            {
                LinkedListNode<Video> seeker = vid.Next;
                while ((seeker.Next != null) && (seeker.Next.Value.getVotes() > vid.Value.getVotes()))
                {
                    movement--;
                    seeker = seeker.Next;
                }
                movement--;
                videoList.Remove(vid);

                videoList.AddAfter(seeker, vid);
            }
            else if ((vid.Previous != null) && (vid.Previous.Value.getVotes() < vid.Value.getVotes()))
            {
                LinkedListNode<Video> seeker = vid.Previous;
                while ((seeker.Previous != null) && (seeker.Previous.Value.getVotes() < vid.Value.getVotes()))
                {
                    movement++;
                    seeker = seeker.Previous;
                }
                movement++;
                videoList.Remove(vid);
                videoList.AddBefore(seeker, vid);
            }
            return movement;
        }

        /*
         * There's a chance that synchonicity could cause searching by video with votes included could result in LinkedList's Find() method not working.
         * This function searches for the first video by title and url only.
         * Will be used for voting on songs.  Currently not compatible with duplicate videos in the queue.
         */
        public LinkedListNode<Video> findByTitleAndUrl(string searchTitle, string searchURL)
        {
            LinkedListNode<Video> seeker = videoList.First;
            while (((seeker.Value.getTitle() != searchTitle) || (seeker.Value.getUrl() != searchURL)) && (seeker != null))
            {
                seeker = seeker.Next;
            }
            return seeker;
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