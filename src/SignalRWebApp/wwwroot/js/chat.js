document.addEventListener('DOMContentLoaded', async function() {
    // Retrieve the JWT token from local storage
    const jwtToken = localStorage.getItem("token");

    // Check if the token exists
    if (jwtToken) {
        console.log("Token exists:", jwtToken);
        const baseUrl = window.location.origin; 

        const notificationUrl = `${baseUrl}/notifications`;

        // Configure and start the SignalR connection
        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(notificationUrl, {
                accessTokenFactory: () => jwtToken
            })
            .build();

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
