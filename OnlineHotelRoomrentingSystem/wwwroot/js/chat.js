"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var chatBox = document.getElementById("chat-box");
    var li = document.createElement("div");
    li.className = "chat-message";
    li.textContent = `${user} says: ${message}`;
    chatBox.appendChild(li);
    chatBox.scrollTop = chatBox.scrollHeight;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = '';
    event.preventDefault();
});
