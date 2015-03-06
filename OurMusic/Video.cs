using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OurMusic
{
    public class Video
    {
        //may need to change these variables to be public
        private string title;        //title of the video 
        private string url;          //url of the video
        private string artist;       //artist of the video
        private string album;        //album of the video
        private int votes;           //votes to determine play order

        //constructor for video class, initially all unknown
        public void Video()
        {
            this.title = "";
            this.url = "";
            this.artist = "";
            this.album = "";
            this.votes = 0;
        }

        /*Title Helper Functions*/
        public string getTitle()
        {
            return this.title;
        }
        public void setTitle(string title)
        {
            this.title = title;
        }

        /*Artist Helper Functions*/
        public string getArtist()
        {
            return this.artist;
        }
        public void setArtist(string artist)
        {
            this.artist = artist;
        }

        /*Album Helper Functions*/
        public string getAlbum()
        {
            return this.album;
        }
        public void setAlbum(string album)
        {
            this.album = album;
        }

        /*URL Helper Functions*/
        public string getUrl()
        {
            return this.url;
        }
        public void setUrl(string url)
        {
            this.url = url;
        }

        /*Votes Helper Functions*/
        public int getVotes()
        {
            return this.votes;
        }
        public void upvote()
        {
            this.votes++;
        }
        public void downvote()
        {
            this.votes--;
        }
    }

}