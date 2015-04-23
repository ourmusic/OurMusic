
var roomName;
var refreshListTest = function (jsonString) {

    //parse json representation of queue into JavaScript Array of Video objects
    var parsedList = JSON.parse(jsonString);
    var rowHTML = ""

    $('#tbd tr').remove();

    for (i = 0; i < parsedList.length; i++) {
        addRow(parsedList[i].title, parsedList[i].url, parsedList[i].votes);

    }

};


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
$(function () {

    var tHub = $.connection.timerHub;

    roomName = document.getElementById("roomName").innerHTML;


    tHub.client.refreshList = function (jsonString) {

        //parse json representation of queue into JavaScript Array of Video objects
        var parsedList = JSON.parse(jsonString);
        var rowHTML = ""

        $('#tbd tr').remove();

        for (i = 0; i < parsedList.length; i++) {
            addRow(parsedList[i].title, parsedList[i].url, parsedList[i].votes);

        }

    };

    tHub.client.deleteVideo = function (videoUrl) {
        var videoRow = document.getElementById("queueList").rows.namedItem(videoUrl);
        $(videoRow).remove();
    }


    tHub.client.adjustVotesAndPlacement = function (videoUrl, votesChange, movement) {

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


    $(document.body).on('click', 'button.delete', function () {

        //alert("roomName = " + roomName);
        var row = this.parentNode.parentNode;

        var rowIndex = row.rowIndex;
        var videoTitle = row.cells[0].innerHTML;
        var videoURL = row.cells[1].innerHTML;

        tHub.server.deleteVideo(videoTitle, videoURL, roomName);

    });
    $(document.body).on('click', 'button.upvote', function () {

        //alert("roomName = " + roomName);
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
            //votesCell.innerHTML = oldVotes + 1;

            $(upGlyphSpan).css("color", "#FF9933");
            // alert("neutral to up!  rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, 1, roomName);
        }
        else if (row.value == "up") {
            row.value = "neutral";
            //votesCell.innerHTML = oldVotes - 1;
            $(upGlyphSpan).css("color", "#FFFFFF");
            // alert("up to neutral! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, -1, roomName);
        }
        else {
            //currently downvoted
            row.value = "up";
            //votesCell.innerHTML = oldVotes + 2;
            $(upGlyphSpan).css("color", "#FF9933");
            $(downGlyphSpan).css("color", "#FFFFFF");
            // alert("down to up! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, 2, roomName);
        }




    });



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
            //votesCell.innerHTML = oldVotes - 1;
            $(downGlyphSpan).css("color", "#33CCFF");
            // alert("neutral to down!  rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, -1, roomName);
        }
        else if (row.value == "down") {
            row.value = "neutral";
            //votesCell.innerHTML = oldVotes + 1;
            $(downGlyphSpan).css("color", "#FFFFFF");
            //alert("down to neutral! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, 1, roomName);
        }
        else {
            //currently upvoted
            row.value = "down";
            //votesCell.innerHTML = oldVotes - 2;
            $(upGlyphSpan).css("color", "#FFFFFF");
            $(downGlyphSpan).css("color", "#33CCFF");
            //alert("up to down! rowIndex: " + rowIndex + " videoTitle: " + videoTitle + " videoURL: " + videoURL + " oldVotes: " + oldVotes);
            tHub.server.voteByTitleAndUrl(videoTitle, videoURL, -2, roomName);
        }



    });

    tHub.client.addVideo = function (vidTitle, vidURL) {
        addRow(vidTitle, vidURL, 0);
    };


    // Start the connection.
    $.connection.hub.start().done(function () {

        var rName = document.getElementById("roomName").innerHTML;
        tHub.server.refreshClient(rName);

        $('#addVideo').click(function () {
            // Call the Send method on the hub.
            tHub.server.addToQueue($('#vidTitle').val(), $('#vidUrl').val(), roomName);
            // Clear text box and reset focus for next comment.
            $('#vidUrl').val('');
            $('#vidTitle').val('').focus();
        });







    });

});

function deleteRow(r) {
    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("myTable").deleteRow(i);
}

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

    if(document.getElementById("isAdmin").value == "admin") {
        var cell6 = row.insertCell(5);
        var delButton = document.createElement("button");
        delButton.id = url;
        delButton.type = "button";
        delButton.className = "btn btn-default btn-sm delete red";

   

        var delSpan = document.createElement("span");
        delSpan.className = "glyphicon glyphicon-remove red";
        delSpan.color = "#FF0000";
        
        delButton.appendChild(delSpan);
        cell6.appendChild(delButton);
    }
};
