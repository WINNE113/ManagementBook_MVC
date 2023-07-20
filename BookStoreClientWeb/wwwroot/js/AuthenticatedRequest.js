// Function to make an authenticated API request with the Bearer token
function makeAuthenticatedRequest(accessToken) {
    // Replace 'YOUR_API_ENDPOINT' with the actual URL of your API endpoint
    const apiEndpoint = 'https://localhost:7275/api';

    // Set the 'Authorization' header with the Bearer token
    const headers = {
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'application/json', // Adjust the 'Content-Type' according to your API's requirements
    };

    // Make the authenticated API request
    return fetch(apiEndpoint, {
        method: 'GET', // Replace with the HTTP method you need (GET, POST, etc.)
        headers: headers,
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
