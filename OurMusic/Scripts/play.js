var tag = document.createElement('script');

tag.src = "https://www.youtube.com/iframe_api";
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

var player;
var time;

function onYouTubeIframeAPIReady() {
    player = new YT.Player('player', {
        height: '390',
        width: '640',
        videoId: 'zjQlCbshk1A',
    });
}




//This is where the server-client code goes
$(function () {
    var prox = $.connection.timerHub;

    function init() {

        if (getLoggedInPerson().userID == ViewBag.room.getAdministartor()) {
            time = player.getDuration();
        prox.server.startCountDown(time);
        }
        player.playVideo();
        
    }

    prox.client.change = function (video) {
        document.getElementById("queueList").deleteRow(1);
        player.loadVideoById(video);
        setTimeout(getTime, 2000);
    }

    function getTime() {
        time = player.getDuration();
        prox.server.startCountDown(time);
    }
    
    $.connection.hub.start().done(init);
});