document.getElementById("loginButton").addEventListener("click", async function() {
    var mobileNumber = document.getElementById("mobileNumber").value;

    // Make a POST request to the signIn endpoint
    try {
        var response = await fetch('/signIn', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ MobileNumber: mobileNumber })
        });

        if (response.ok) {
            var jwtToken = await response.text();

            // Store the JWT token in local storage
            localStorage.setItem("token", jwtToken);
            console.log('Successfully signed');
            window.location.href = "/chat";
        } else {
            console.error('Sign-in request failed with status ' + response.status);
        }
    } catch (error) {
        console.error('Error occurred during sign-in request:', error);
    }
});
