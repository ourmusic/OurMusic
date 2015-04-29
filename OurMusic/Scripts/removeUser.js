$(function () {

    $(document.body).on('click', 'span.removeUser', function () {


        var userID = this.getAttribute('data-userID');

        tHub.server.removeUser(userID, roomName);
        //alert("userName set to : " + userID);

    });

    var tHub = $.connection.timerHub;
    var rName = document.getElementById("roomName").innerHTML;
    tHub.client.userRemoved = function (uID) {
        if (document.getElementById("currentID").value == uID) {
            tHub.server.leaveRoom(rName);
            alert("You have been removed from the room.  Your actions will no longer affect the other users in the room.");
        } else {
            var listItem = document.getElementById(uID);
            var uList = listItem.parentNode;
            uList.removeChild(listItem);
        }

    };

});