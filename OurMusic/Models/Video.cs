using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OurMusic.Models
{
    public class Video
    {
        //these member variables must be public for json serialization
        public string title;        //title of the video 
        public string url;          //url of the video
        public int votes;           //votes to determine play order

        /// <summary>
        /// constructor for video class, initially all unknown
        /// </summary>
        public Video()
        {
            this.title = "";
            this.url = "";
            //this.artist = "";
            //this.album = "";
            this.votes = 0;
        }

        public Video(string t, string u, int v)
        {
            this.title = t;
            this.url = u;
            //this.artist = "";
            //this.album = "";
            this.votes = v;
        }

        public Video(string t, string u)
        {
            this.title = t;
            this.url = u;
            this.votes = 0;
        }


        public string getTitle()
        {
            return this.title;
        }


        public void setTitle(string title)
        {
            this.title = title;
        }


        public string getUrl()
        {
            return this.url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void setUrl(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Votes Helper Functions
        /// </summary>
        /// <returns></returns>
        public int getVotes()
        {
            return this.votes;
        }

        /// <summary>
        /// 
        /// </summary>
        public void upvote()
        {
            this.votes++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void downvote()
        {
            this.votes--;
        }

        /*
         * when a user has a video downvoted, and then clicks upvote, this triggers a 2 vote swing;
         * int voteUpOrDown will be positive for upvotes, negative for downvotes
         */
        public void vote(int votesChange)
        {
            this.votes += votesChange;
        }
    }

}