﻿
//global which is set to the room name using javascript DOM.  sent as parameter to server functions
var roomName;


$(function () {

    var rHub = $.connection.roomHub;

    roomName = document.getElementById("roomName").innerHTML;

    /**
     * Called from server upon connecting and then never again.
     * Gets a JSON of the Room Queue, and constructs a HTML Table
     **/
    rHub.client.refreshList = function (jsonString) {

        //parse json representation of queue into JavaScript Array of Video objects
        var parsedList = JSON.parse(jsonString);
        var rowHTML = "";

        $('#tbd tr').remove();

        for (i = 0; i < parsedList.length; i++) {
            addRow(parsedList[i].title, parsedList[i].url, parsedList[i].votes);
        }
    };


    //Video Rows are ID'd by their urls, deleteVideo(videoUrl) finds the corresponding row and deletes it.
    rHub.client.deleteVideo = function (videoUrl) {
        var videoRow = document.getElementById("queueList").rows.namedItem(videoUrl);
        $(videoRow).remove();
    };

    /**Called by the sever after a user disconnects or was removed.
     * If calling client is the removed user, they are notified.
     * If not removed user, deletes user from user list.
     */
    rHub.client.userRemoved = function (uID) {
       
        var myID = document.getElementById("currentID").value;


        if (myID == uID) {
            rHub.server.leaveRoom(roomName);
            alert("You have been removed from the room.  Your actions will no longer affect the other users in the room.");
        } else {
            var listItem = document.getElementById(uID);
            var uList = document.getElementById("userList");
            uList.removeChild(listItem);
        }

    };

    /**
     * moved addUser to global namespace, similar to addRow
     * adds user to userlist after they join the room.
     */
    rHub.client.addNewUser = function (firstName, lastName, userID) {
        addUser(firstName, lastName, userID);
    };

    /**
     * When the admin deletes a room, this notifies room members that the room won't work anymore.
     */
    rHub.client.alertRoomHasBeenDeleted = function () {
        alert("Room " + roomName + " has been deleted by the administrator.  This room is no longer functional. :(");
    };


    /**
     * Called by the server after the server adjusts the queue after a vote.
     * param movement tells client how to move the row corresponding to that video.
     */
    rHub.client.adjustVotesAndPlacement = function (videoUrl, votesChange, movement) {

        if (movement == -999) return;

        var videoRow = document.getElementById("queueList").rows.namedItem(videoUrl);
        var votesCell = videoRow.cells[2];
        var oldVotes = parseInt(votesCell.innerHTML);
        votesCell.innerHTML = oldVotes + votesChange;


        while (movement > 0) {
            //move up

            $(videoRow).prev().before(videoRow);
            movement--;
        }
        while (movement < 0) {
            $(videoRow).next().after(videoRow);
            movement++;
        }


    };

    //Binds remove user button to call server removeUser
    $(document.body).on('click', 'span.removeUser', function () {


        var userID = this.getAttribute('data-userID');

        rHub.server.removeUser(userID, roomName);

    });

    //binds remove video button to call server deleteVideo
    $(document.body).on('click', 'button.delete', function () {

        var row = this.parentNode.parentNode;

        var rowIndex = row.rowIndex;
        var videoTitle = row.cells[0].innerHTML;
        var videoURL = row.cells[1].innerHTML;

        rHub.server.deleteVideo(videoTitle, videoURL, roomName);

    });

    //binds upvote to notify server and adjust this users row button color
    $(document.body).on('click', 'button.upvote', function () {


        var row = this.parentNode.parentNode;

        var votesCell = row.cells[2];
        var oldVotes = parseInt(votesCell.innerHTML);

        var rowIndex = row.rowIndex;
        var videoTitle = row.cells[0].innerHTML;
        var videoURL = row.cells[1].innerHTML;

        var spansArray = row.getElementsByTagName("SPAN");
        var upGlyphSpan = spansArray[0];
        var downGlyphSpan = spansArray[1];


        if (row.value == "neutral") {
            row.value = "up";
            $(upGlyphSpan).css("color", "#FF9933");
            // alert("neutral to up!  rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, 1, roomName);
        }
        else if (row.value == "up") {
            row.value = "neutral";
            $(upGlyphSpan).css("color", "#FFFFFF");
            // alert("up to neutral! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, -1, roomName);
        }
        else {
            //currently downvoted
            row.value = "up";
            $(upGlyphSpan).css("color", "#FF9933");
            $(downGlyphSpan).css("color", "#FFFFFF");
            // alert("down to up! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, 2, roomName);
        }

    });


    //binds downvote to notify server, and adjusts this user's row
    $(document.body).on('click', 'button.downvote', function () {

        var row = this.parentNode.parentNode;

        var votesCell = row.cells[2];
        var oldVotes = parseInt(votesCell.innerHTML);

        var rowIndex = row.rowIndex;
        var videoTitle = row.cells[0].innerHTML;
        var videoURL = row.cells[1].innerHTML;

        var spansArray = row.getElementsByTagName("SPAN");
        var upGlyphSpan = spansArray[0];
        var downGlyphSpan = spansArray[1];

        if (row.value == "neutral") {
            row.value = "down";
            $(downGlyphSpan).css("color", "#33CCFF");
            // alert("neutral to down!  rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, -1, roomName);
        }
        else if (row.value == "down") {
            row.value = "neutral";
            $(downGlyphSpan).css("color", "#FFFFFF");
            //alert("down to neutral! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, 1, roomName);
        }
        else {
            row.value = "down";
            $(upGlyphSpan).css("color", "#FFFFFF");
            $(downGlyphSpan).css("color", "#33CCFF");
            rHub.server.voteByTitleAndUrl(videoTitle, videoURL, -2, roomName);
        }



    });

    //called by server, calls global addRow to add row to queue in HTML table
    rHub.client.addVideo = function (vidTitle, vidURL) {
        addRow(vidTitle, vidURL, 0);
    };


    // Start the connection.
    $.connection.hub.start().done(function () {

        var rName = document.getElementById("roomName").innerHTML;
        var firstName = document.getElementById("firstName").value;
        var lastName = document.getElementById("lastName").value;
        var userID = document.getElementById("currentID").value;

        rHub.server.refreshClient(rName, userID);
        rHub.server.announceEntranceToRoom(rName, firstName, lastName, userID);



        $('#addVideo').click(function () {
            // Call the Send method on the hub.
            rHub.server.addToQueue($('#vidTitle').val(), $('#vidUrl').val(), roomName);
            // Clear text box and reset focus for next comment.
            $('#vidUrl').val('');
            $('#vidTitle').val('').focus();
        });

    });

});

/**
 * no server functionality, helper function to add HTML 'li' for a user that joins the room
 */
function addUser(firstName, lastName, userID) {

    var li = document.createElement("li");
    li.className = "list-group-item";
    li.id = userID;
    var nameString = firstName + " " + lastName;
    if (document.getElementById("adminID").value == userID) {
        var adminString = "(Administrator)";
        nameString += " " + adminString;
    }
    li.appendChild(document.createTextNode(nameString));

    if (document.getElementById("isAdmin").value == "admin") {
        var xSpan = document.createElement("SPAN");
        xSpan.className = "glyphicon glyphicon-remove removeUser";
        xSpan.setAttribute('data-userID', userID);
        $(xSpan).css("float", "right");
        $(xSpan).css("color", "red");

        li.appendChild(xSpan);

    }

    var ul = document.getElementById("userList");
    ul.appendChild(li);

};

/**
 * no server functionality, helper function to add a row (video) to html table representing the queue
 */
function addRow(title, url, votes) {
    var table = document.getElementById('queueList');
    var rowCount = table.rows.length;
    var row = table.insertRow(rowCount);
    row.value = "neutral";
    row.id = url;
    var cell1 = row.insertCell(0);
    cell1.innerHTML = title;
    var cell2 = row.insertCell(1);
    cell2.innerHTML = url;

    var cell3 = row.insertCell(2);
    cell3.className = "votesText";
    cell3.innerHTML = votes;

    var cell4 = row.insertCell(3);
    var upButton = document.createElement("button");
    upButton.id = url;
    upButton.type = "button";
    upButton.className = "btn btn-default btn-sm move upvote";



    var upBtnSpan = document.createElement("span");
    upBtnSpan.className = "glyphicon glyphicon-arrow-up";

    cell4.appendChild(upButton);
    upButton.appendChild(upBtnSpan);


    var cell5 = row.insertCell(4);
    var downButton = document.createElement("button");
    downButton.id = url + "down";
    downButton.type = "button";
    downButton.className = "btn btn-default btn-sm downvote";

    var downBtnSpan = document.createElement("span");
    downBtnSpan.className = "glyphicon glyphicon-arrow-down";

    downButton.appendChild(downBtnSpan);
    cell5.appendChild(downButton);

    if (document.getElementById("isAdmin").value == "admin") {
        var cell6 = row.insertCell(5);
        var delButton = document.createElement("button");
        delButton.id = url;
        delButton.type = "button";
        delButton.className = "btn btn-default btn-sm delete red";



        var delSpan = document.createElement("span");
        delSpan.className = "glyphicon glyphicon-remove red";
        $(delSpan).css("color", "#FF0000");

        delButton.appendChild(delSpan);
        cell6.appendChild(delButton);
    }
};

//WAS used for Qunit testing.  Prefer manual testing
/*
var refreshListTest = function (jsonString) {

    //parse json representation of queue into JavaScript Array of Video objects
    var parsedList = JSON.parse(jsonString);
    var rowHTML = ""

    $('#tbd tr').remove();

    for (i = 0; i < parsedList.length; i++) {
        addRow(parsedList[i].title, parsedList[i].url, parsedList[i].votes);

    }

};


//Was used for Qunit testing.
adjustVotesAndPlacementTest = function (videoUrl, votesChange, movement) {

    if (movement == -999) return;

    var videoRow = document.getElementById("queueList").rows.namedItem(videoUrl);
    //alert("movement = " + movement);
    var votesCell = videoRow.cells[2];
    var oldVotes = parseInt(votesCell.innerHTML);
    votesCell.innerHTML = oldVotes + votesChange;
    //var toMove = movement;

    while (movement > 0) {
        //move up
        //alert("moving up")
        $(videoRow).prev().before(videoRow);
        movement--;
    }
    while (movement < 0) {
        $(videoRow).next().after(videoRow);
        movement++;
    }


};
*/
