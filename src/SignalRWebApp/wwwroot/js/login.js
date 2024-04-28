document.getElementById("loginButton").addEventListener("click", async function() {
    var mobileNumber = document.getElementById("mobileNumber").value;

    const url = "https://localhost:44345/signIn";
    
    
    // Make a POST request to the signIn endpoint
    try {
        var response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ MobileNumber: mobileNumber })
        });

        if (response.ok) {
            var payload = await response.text();
            const accessToken = payload.accessToken;

            // Store the JWT token in local storage
            localStorage.setItem("token", accessToken);
            console.log('Successfully signed with token', accessToken);
            //window.location.href = "/chat";
        } else {
            console.error('Sign-in request failed with status ' + response.status);
        }
    } catch (error) {
        console.error('Error occurred during sign-in request:', error);
    }
});
