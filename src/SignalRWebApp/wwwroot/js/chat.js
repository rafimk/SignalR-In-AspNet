// Retrieve the JWT token from local storage
var jwtToken = localStorage.getItem("token");

// Check if the token exists
if (jwtToken) {
    console.log("Token exists", jwtToken);
    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/notifications", {
            accessTokenFactory: () => jwtToken
        })
        .build();
   
    // Append the JWT token as a query parameter to the hub URL
    // var hubUrl = "/chatHub?access_token=" + encodeURIComponent(jwtToken);
    //
    // // Create a new hub connection
    // var connection = new signalR.HubConnectionBuilder()
    //     .withUrl(hubUrl)
    //     .build();

    // Start the connection
    hubConnection.on("ReceiveNotification", (message) => {
        const listItem = document.createElement("li");
        listItem.textContent = message;
        notificationList.appendChild(listItem);
    });
} else {
    // Redirect to index page or handle authentication failure
    window.location.href = "/index"; // Replace with the correct URL of your index page
}
