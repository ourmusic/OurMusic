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


    tHub.client.addNewUser = function (firstName, lastName, userID) {

        var li = document.createElement("li");
        li.className = "list-group-item";
        var nameString = firstName + " " + lastName;
        if (document.getElementById("adminID").value == userID) {
            nameString += " " + "(Administrator)".italics();
        }
        li.appendChild(document.createTextNode(nameString));

        var xSpan = document.createElement("SPAN");
        xSpan.className = "glyphicon glyphicon-remove removeUser";
        xSpan.setAttribute('data-userID', userID);
        $(xSpan).css("float", "right");
        $(xSpan).css("color", "red");

        li.appendChild(xSpan);

        var ul = document.getElementById("userList");
        ul.appendChild(li);


    };


});