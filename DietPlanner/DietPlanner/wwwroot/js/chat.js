const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

$(document).ready(function () {
    let userName; // Declare a global variable to hold the logged-in user data

    $.ajax({
        url: '/Auth/LayoutData',
        type: 'GET',
        success: function (data) {
            console.log('Raw data from server:', data); // Log the raw data

            let loggedUser;
            if (typeof data === 'string') {
                try {
                    loggedUser = JSON.parse(data); // Attempt to parse the data
                } catch (e) {
                    console.error('Error parsing JSON:', e);
                    return; // Exit if parsing fails
                }
            } else {
                loggedUser = data;
            }

            console.log('Parsed user data:', loggedUser); // Log the parsed user data
            if (loggedUser && loggedUser.Username) {
                console.log('Username:', loggedUser.Username); // Access Username
                userName = loggedUser.Username;
            } else {
                console.error('Username not found in the logged user data');
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error('Error occurred while retrieving logged-in user:', errorThrown);
        }
    });

    // Send the message
    document.getElementById("sendMessage").addEventListener("click", event => {
        event.preventDefault(); // Prevent default form submission

        if (!userName) {
            console.error('Username is not available');
            return;
        }

        const message = document.getElementById("userMessage").value;
        if (!message) {
            console.error('Message is empty');
            return;
        }

        connection.invoke("SendMessage", userName, message).catch(err => console.error(err.toString()));
    });
});

// This method receives the message and appends it to our list
connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = `${user} :: ${msg}`;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(err => console.error(err.toString()));
