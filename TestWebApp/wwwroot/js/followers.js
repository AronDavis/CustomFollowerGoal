"use strict";

var _numSubs = 0;
var _subsGoal = 400;

var _connection = new signalR.HubConnectionBuilder().withUrl("/followersHub").build();

document.getElementById("rightText").innerText = _subsGoal;

function updateDisplay() {
    document.getElementById("middleText").innerText = _numSubs;
    document.getElementById("rightText").innerText = _subsGoal;

    document.getElementById("progress").style.width = `${(_numSubs / _subsGoal) * 100}%`;
}

_connection.on("UpdateFollowers", function (numFollowers) {

    _numSubs = numFollowers;
    updateDisplay();
});

_connection.on("UpdateFollowerGoal", function (followerGoal) {

    _subsGoal = followerGoal;
    updateDisplay();
});



function updateSubs() {
    _connection.invoke("RequestFollowersUpdate").catch(function (err) {
        return console.error(err.toString());
    });
}

_connection.start().then(function () {
    updateSubs();
}).catch(function (err) {
    return console.error(err.toString());
});


/*
 //keeping this shit here so I can reference how parameters are passed
 _connection.invoke("RequestFollowersUpdate", MY_ID_HERE).catch(function (err) {
        return console.error(err.toString());
    });
*/
