"use strict";

var _numfollowers = 0;
var _followersGoal = 4000;

var _connection = new signalR.HubConnectionBuilder().withUrl("/followersHub").build();

document.getElementById("rightText").innerText = _followersGoal;

function updateDisplay() {
    document.getElementById("middleText").innerText = _numfollowers;
    document.getElementById("rightText").innerText = _followersGoal;

    document.getElementById("progress").style.width = `${(_numfollowers / _followersGoal) * 100}%`;
}

_connection.on("UpdateFollowers", function (numFollowers) {

    _numfollowers = numFollowers;
    updateDisplay();
});

_connection.on("UpdateFollowerGoal", function (followerGoal) {

    _followersGoal = followerGoal;
    updateDisplay();
});



function updateFollows() {
    _connection.invoke("RequestFollowersUpdate").catch(function (err) {
        return console.error(err.toString());
    });
}

_connection.start().then(function () {
    updateFollows();
}).catch(function (err) {
    return console.error(err.toString());
});


/*
 //keeping this shit here so I can reference how parameters are passed
 _connection.invoke("RequestFollowersUpdate", MY_ID_HERE).catch(function (err) {
        return console.error(err.toString());
    });
*/
