if (!("Notification" in window)) {
    console.log("This browser does not support notifications");
}
if (Notification.permission !== "granted") {
    Notification.requestPermission();
}
var browserNotification = Notification;

var wsUrlBase = "wss://pusher.adin.ir/api/";
var pusherUrlBase = "https://pusher.adin.ir/api/";



window.addEventListener("load", onLoad, false);

function onLoad() {
    var wsUri = wsUrlBase + "WebSocket/Get";
    websocket = new WebSocket(wsUri);
    websocket.onopen = function (evt) { onOpen(evt) };
    websocket.onclose = function (evt) { onClose(evt) };
    websocket.onmessage = function (evt) { onMessage(evt) };
    websocket.onerror = function (evt) { onError(evt) };
}

function onOpen(evt) {

    console.log("Connected to server");
}

function onClose(evt) {
    console.log("Not connected");
	onLoad();
}

function onMessage(evt) {

    console.log(evt);
    handleData(evt.data);


}

function onError(evt) {
    console.log("Communication error");
}


function handleData(dataa) {	
    var data = JSON.parse(dataa);
    var userID = Xrm.Page.context.getUserId().substring(1,37).toLowerCase();
	console.log(userID);
    switch (data.Code) {
        case 0:
            {

                fetch(
                    pusherUrlBase +
                    "WebSocket/JoinGroup?groupname=" +
                    userID +
                    "&userId=" +
                    data.Data,
                    {
                        method: "Get",
                        headers: {
                            Accept: "application/json",
                            "Content-Type": "application/json",
                        }
                    }
                )
                    .then(function (res) { })
                    .catch(error => {

                        console.error(error);
                    });

            }
            break;
        case 100:
            {
                console.log(data.Message);
                ShowAlert(data.Title, data.Message,decodeURIComponent( data.Link));
            }
            break;
    }
}


function ShowAlert(title, message, link) {
    if (browserNotification.permission !== "granted") {
        browserNotification.requestPermission();
    }
    else {
		 var options = {
			body: message,			
			icon: "https://adin.ir/images/IranCard-logo.png",
			requireInteraction: true,			
			title:"Default Title",
		}
        var notification = new browserNotification(title, options);
        notification.onclick = function () {
            window.open(link);
        };
		setTimeout(notification.close.bind(notification), 60000);	
    }
}

