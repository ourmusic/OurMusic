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
        videoId: '2Jp5IZYyma4'

    });
}

//This is where the server-client code goes
$(function () {
    var prox = $.connection.timerHub;

    function init() {
        time = player.getDuration();
        player.playVideo();
        prox.server.startCountDown(time);
    }

    prox.client.change = function (video) {
        player.cueVideoById(video);
        time = player.getDuration();
        player.playVideo();
        prox.server.startCountDown(time);
    }



    $.connection.hub.start().done(init);
});