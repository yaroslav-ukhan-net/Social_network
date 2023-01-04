"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

document.getElementById("sendButton").disabled = true;



connection.on("ReceiveMessage", function (chatsViewModel) {            //answer from server

    var wiersz = document.createElement("tr");
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

    var mesId = chatsViewModel.lastMessage.id;
    tdLinks.innerHTML = ' <i>  <input type="button"  value="Remove" />   </i><br>';

    tdLinks.addEventListener("click", function (event) {
        var groupName = document.getElementById("groupNameInput").value;
        var idmes = 5;
        connection.invoke("RemoveMes", idmes, groupName).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

    ////
    //document.getElementById("sendButton").addEventListener("click", function (event) {
    //    var user = document.getElementById("userInput").value;
    //    var message = document.getElementById("messageInput").value;
    //    var groupName = document.getElementById("groupNameInput").value;
    //    connection.invoke("SendMessage", groupName, user, message).catch(function (err) {
    //        return console.error(err.toString());
    //    });
    //    event.preventDefault();
    //});

    //





    wiersz.appendChild(tdNMes);
    wiersz.appendChild(tdLinks);
    var dodac = document.getElementById("tbd");
    dodac.appendChild(wiersz);
});

connection.on("ReceiveRemove", function () {
    this.parentElement.parentElement.deleteRow(this.parentElement.rowIndex);
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
    connection.invoke("SendMessage", groupName, user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
