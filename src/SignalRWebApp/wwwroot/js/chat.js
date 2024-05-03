document.addEventListener('DOMContentLoaded', async function() {
    // Retrieve the JWT token from local storage
    const jwtToken = localStorage.getItem("token");

    // Check if the token exists
    if (jwtToken) {
        console.log("Token exists:", jwtToken);
        const baseUrl = window.location.origin; 

        const notificationUrl = `${baseUrl}/notifications`;

        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(notificationUrl, {
                accessTokenFactory: () => jwtToken
            })
            .build();

        //const hubConnection = new HubConnectionBuilder()
        //    .WithUrl(notificationUrl, options => {
        //        options.AccessTokenProvider = () => Task.FromResult(jwtToken);
        //    })
        //    .WithAutomaticReconnect()
        //    .Build();


        //const hubConnection = new signalR.HubConnectionBuilder()
        //    .withUrl(notificationUrl, {
        //        accessTokenFactory: () => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImY0NWZlNDc1LTg0NjYtNDg0Zi1hZjY5LWEyNjU4YThlZTkxNSIsInN1YiI6ImY0NWZlNDc1LTg0NjYtNDg0Zi1hZjY5LWEyNjU4YThlZTkxNSIsImp0aSI6IjRhZDhhZDIiLCJhdWQiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDAwIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzgiXSwibmJmIjoxNzEzODkyODczLCJleHAiOjE3MjE3NTUyNzMsImlhdCI6MTcxMzg5Mjg3NCwiaXNzIjoiZG90bmV0LXVzZXItand0cyJ9.wvn-A1CirE9VR6qmLiWZoXJRgavDgve9xi4RZjajnRE"
        //    })
        //    .build();

        try {
            // Start the SignalR connection
            await hubConnection.start();
            console.log("SignalR connection established.");

            // Handle incoming notifications
            hubConnection.on("ReceiveNotification", (message) => {
                const notificationList = document.getElementById("notificationList");
                if (notificationList) {
                    const listItem = document.createElement("li");
                    listItem.textContent = message;
                    notificationList.appendChild(listItem);
                }
            });
        } catch (error) {
            console.error("Error establishing SignalR connection:", error);
            // Handle connection error (e.g., display error message to user)
        }
    } else {
        // Redirect to index page or handle authentication failure
        console.error("JWT token not found. Redirecting to index page.");
        window.location.href = "/index"; // Replace with the correct URL of your index page
    }
});
