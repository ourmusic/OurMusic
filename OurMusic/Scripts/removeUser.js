$(function () {

    var rName = document.getElementById("roomName").innerHTML;
    var tHub = $.connection.timerHub;
    $(document.body).on('click', 'span.removeUser', function () {


        var userID = this.getAttribute('data-userID');
        alert("userID set to : " + userID);

        tHub.server.removeUser(userID, rName);
        //alert("userName set to : " + userID);

    });

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
        li.id = userID;
        var nameString = firstName + " " + lastName;
        if (document.getElementById("adminID").value == userID) {
            var adminString = "(Administrator)";
            nameString += " " + adminString.italics();
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