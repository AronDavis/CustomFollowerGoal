"use strict";

var _numFollowers = 0;
var _followerGoal = 100;

var _connection = new signalR.HubConnectionBuilder().withUrl("/followers").build();

document.getElementById("rightText").innerText = _followerGoal;

function updateDisplay() {
    document.getElementById("middleText").innerText = _numFollowers;
    document.getElementById("rightText").innerText = _followerGoal;

    document.getElementById("progress").style.width = `${(_numFollowers / _followerGoal) * 100}%`;
}

_connection.on("UpdateFollowers", function (numFollowers) {

    _numFollowers = numFollowers;
    updateDisplay();
});

_connection.on("UpdateFollowerGoal", function (followerGoal) {

    _followerGoal = followerGoal;
    updateDisplay();
});



function updateFollowers() {
    _connection.invoke("RequestFollowersUpdate").catch(function (err) {
        return console.error(err.toString());
    });
}

_connection.start().then(function () {
    updateFollowers();
}).catch(function (err) {
    return console.error(err.toString());
});


/*
 //keeping this shit here so I can reference how parameters are passed
 _connection.invoke("RequestFollowersUpdate", MY_ID_HERE).catch(function (err) {
        return console.error(err.toString());
    });
*/
