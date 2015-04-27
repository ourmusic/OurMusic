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
        /*
         * upvotes a video by 1.  implemented before client queue.  not currently used.
         */
        public void upvote(Video v)
        {
            LinkedListNode<Video> vid = videoList.Find(v);
            vid.Value.upvote();
            checkOrder(vid);
        }

        //not currently used for iteration 5
        public void upvoteAt(int i)
        {
            Video vid = videoList.ElementAt(i);
            upvote(vid);
        }

        //not currently used for iteration 5
        public void downvote(Video v)
        {
            LinkedListNode<Video> vid = videoList.Find(v);
            vid.Value.downvote();
            checkOrder(vid);
        }

        //not currently used for iteration 5
        public void downvoteAt(int i)
        {
            Video vid = videoList.ElementAt(i);
            downvote(vid);
        }

        /*
         * Finds the video in the queue by title and url, then adjusts that video's vote score by voteChange
         * @param vidTitle title of video to vote on, passed from client javascript to timerhub to this
         * @param, vidUrl url of video to vote on, passed from client javascript to timerhub to this
         * @param, voteChange [-2, 2] inclusive vote swing from neutral, downvoted, or upvoted state to a different state
         * @return the net movement in the queue of the video that was voted on as a result.
         * Currently does not work for duplicate videos in the queue.
         */
        public int vote(string vidTitle, string vidUrl, int voteChange)
        {
            LinkedListNode<Video> vid = findByTitleAndUrl(vidTitle, vidUrl);
            if (vid == null) return VIDEONOTFOUND;
            vid.Value.vote(voteChange);
            return checkOrder(vid);
        }

        public void delete(string vidTitle, string vidUrl){
            LinkedListNode<Video> vid = findByTitleAndUrl(vidTitle, vidUrl);
            if(vid != null) videoList.Remove(vid);
        }


        /*
         * called in vote.  After a video's vote score is adjusted, checkOrder runs to see if that video needs to move in the queue to maintain sorted scores.
         * @return the net movement of the video voted on positive return mean the video moved up in the queue, negative return values mean the video moved down
         * i.e. a return value of 3 means the voted upon video moved up 3 spots in the queue.
         */
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
            while ((seeker != null) && ((seeker.Value.getTitle() != searchTitle) || (seeker.Value.getUrl() != searchURL)))
            {
                seeker = seeker.Next;
            }
            return seeker;
        }

        /*
         * @return a json representation of the queue in its current state
         */
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