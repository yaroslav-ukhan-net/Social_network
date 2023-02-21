"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

document.getElementById("sendButton").disabled = true;



connection.on("ReceiveMessage", function (chatsViewModel) {            //answer from server
    var user = document.getElementById("userInput").value;

    var mesId = chatsViewModel.lastMessage.id;

    var wiersz = document.createElement("tr"); wiersz.id = mesId;
    var tdNMes = document.createElement("td");     //left td

    var img = document.createElement("img");    
    img.src = chatsViewModel.avatarURL;
    img.width = "15";

    tdNMes.appendChild(img);

    var td1_2 = chatsViewModel.name;
    tdNMes.innerHTML += td1_2 + " ";
    const event = new Date(chatsViewModel.lastMessage.date);
    const options = { day: 'numeric',  month: 'numeric', year: 'numeric'  };

    var sub = document.createElement("sub");
    var date = new Date(chatsViewModel.lastMessage.date);
    date.toLocaleDateString();
    sub.innerHTML += "(" + event.toLocaleDateString('uk-UA', options) + " " + event.toLocaleTimeString('uk-UA', { hour: "2-digit", minute: "2-digit" }) +")";
    tdNMes.appendChild(sub);

    var td1_3 = chatsViewModel.lastMessage.text;
    tdNMes.innerHTML += " : " + td1_3;



    var tdLinks = document.createElement("td");  //right td
    if (chatsViewModel.lastMessage.idSender == user)
        tdLinks.innerHTML = ' <i>  <input type="button" onclick="RemoveRowId(' + mesId + ')" value="Remove" />   </i><br>';
    else
        tdLinks.innerHTML = ' <i>  <input type="button" onclick="RemoveRowId(' + mesId + ')" value="Another" />   </i><br>';
    wiersz.appendChild(tdNMes);
    wiersz.appendChild(tdLinks);
    var dodac = document.getElementById("tbd");
    dodac.appendChild(wiersz);
});


connection.start().then(function () {                                 //my
    var groupName = document.getElementById("groupNameInput").value;
    connection.invoke("JoinRoom", groupName);
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var groupName = document.getElementById("groupNameInput").value;
    document.getElementById("messageInput").value = "";
    connection.invoke("SendMessage", groupName, user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});


function RemoveRowId(mesId) {
    var groupName = document.getElementById("groupNameInput").value;
    connection.invoke("RemoveMes", mesId, groupName).catch(function (err2) {
        return console.error(err2.toString());
    });
    event.preventDefault();
}

connection.on("ReceiveRemove", function (mesId) {
    document.getElementById(mesId).remove();
});