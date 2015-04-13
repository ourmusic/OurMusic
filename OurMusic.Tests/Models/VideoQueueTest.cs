using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OurMusic.Models;

namespace OurMusic.Tests.Models
{
    [TestClass]
    public class VideoQueueTest
    {
        VideoQueue vidQueue = new VideoQueue();


        [TestMethod]
        public void TestAddThenRemoveFront()
        {
            Video vid1 = new Video("Video One", "Url One", 1);
            Video vid2 = new Video("Video Two", "Url Two", 2);
            Video vid3 = new Video("Video Three", "Url Three", 3);

            vidQueue.addVideo(vid1);
            vidQueue.addVideo(vid2);
            vidQueue.addVideo(vid3);

            Assert.AreEqual(vidQueue.getLength(), 3);

            Assert.AreEqual(vidQueue.removeFirstVideo(), vid1);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid2);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid3);

            Assert.AreEqual(vidQueue.getLength(), 0);

        }

        [TestMethod]
        public void TestUpvoteByVideo()
        {

            Video vid1 = new Video("Video One", "Url One", 1);
            Video vid2 = new Video("Video Two", "Url Two", 1);
            Video vid3 = new Video("Video Three", "Url Three", 1);

            vidQueue.addVideo(vid1);
            vidQueue.addVideo(vid2);
            vidQueue.addVideo(vid3);

            vidQueue.upvote(vid2);
            vidQueue.upvote(vid3);
            vidQueue.upvote(vid3);

            Assert.AreEqual(vidQueue.removeFirstVideo(), vid3);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid2);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid1);

            Assert.AreEqual(vidQueue.getLength(), 0);

        }

        [TestMethod]
        public void TestDownvoteByVideo()
        {

            Video vid1 = new Video("Video One", "Url One", 3);
            Video vid2 = new Video("Video Two", "Url Two", 3);
            Video vid3 = new Video("Video Three", "Url Three", 3);

            vidQueue.addVideo(vid1);
            vidQueue.addVideo(vid2);
            vidQueue.addVideo(vid3);

            vidQueue.downvote(vid1);
            vidQueue.downvote(vid2);
            vidQueue.downvote(vid2);

            Assert.AreEqual(vidQueue.removeFirstVideo(), vid3);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid1);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid2);

            Assert.AreEqual(vidQueue.getLength(), 0);

        }



        [TestMethod]
        public void TestjsonQueue()
        {
            Video vid1 = new Video("Video One", "Url One", 1);
            Video vid2 = new Video("Video Two", "Url Two", 1);
            Video vid3 = new Video("Video Three", "Url Three", 1);

            vidQueue.addVideo(vid1);
            vidQueue.addVideo(vid2);
            vidQueue.addVideo(vid3);

            string expected = "[{\"$id\":\"1\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":1},{\"$id\":\"2\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":1},{\"$id\":\"3\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":1}]";
            Assert.AreEqual(expected, vidQueue.jsonQueue());
            vidQueue = new VideoQueue();
        }

        [TestMethod]
        public void TestVoteByTitleAndUrl1()
        {

            int movement;

            Video vid1 = new Video("Video One", "Url One", 10);
            Video vid2 = new Video("Video Two", "Url Two", 10);
            Video vid3 = new Video("Video Three", "Url Three", 10);

            vidQueue.addVideo(vid1);
            vidQueue.addVideo(vid2);
            vidQueue.addVideo(vid3);

            movement = vidQueue.vote("Video Two", "Url Two", 2);
            Assert.AreEqual(movement, 1);
            Assert.AreEqual(vidQueue.jsonQueue(), "[{\"$id\":\"1\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":12},{\"$id\":\"2\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":10},{\"$id\":\"3\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":10}]");

            movement = vidQueue.vote("Video Three", "Url Three", 1);
            Assert.AreEqual(movement, 1);
            Assert.AreEqual(vidQueue.jsonQueue(), "[{\"$id\":\"1\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":12},{\"$id\":\"2\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":11},{\"$id\":\"3\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":10}]");

            movement = vidQueue.vote("Video Two", "Url Two", -10);
            Assert.AreEqual(movement, -2);
            Assert.AreEqual(vidQueue.jsonQueue(), "[{\"$id\":\"1\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":11},{\"$id\":\"2\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":10},{\"$id\":\"3\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":2}]");

            movement = vidQueue.vote("Video One", "Url One", -10);
            Assert.AreEqual(movement, -1);

            Assert.AreEqual(vidQueue.removeFirstVideo(), vid3);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid2);
            Assert.AreEqual(vidQueue.removeFirstVideo(), vid1);

            Assert.AreEqual(vidQueue.getLength(), 0);

        }

        [TestMethod]
        public void TestVoteOnVideoThatDoesntExist()
        {
            Video vid1 = new Video("Video One", "Url One", 10);
            Video vid2 = new Video("Video Two", "Url Two", 10);
            Video vid3 = new Video("Video Three", "Url Three", 10);
            
            int movement = vidQueue.vote("fake name", "fake url", 100);
            Assert.AreEqual(VideoQueue.VIDEONOTFOUND, movement);
        }






    }
}
