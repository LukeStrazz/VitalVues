
document.querySelectorAll('.talk-bloodwork-btn').forEach(button => {
    button.addEventListener('click', function () {
        isRunning = true;
        document.querySelector('.previous-bloodworks').style.display = 'none';
        const inputField = document.getElementById('promptInput');
        const spinner = document.getElementById('spinner');
        const buttonText = document.getElementById('buttonText');
        const sendButton = document.getElementById('sendButton');

        inputField.value = '';
        inputField.style.height = '50px';
        inputField.disabled = true;
        spinner.style.display = 'inline-block';
        spinner.style.color = 'darkgoldenrod';
        sendButton.style.backgroundColor = 'goldenrod';
        buttonText.textContent = 'Loading...';
        sendButton.disabled = true;
        const bloodworkId = this.getAttribute('data-bloodwork-id');

        let allTestResults = "";
        const bloodworkItem = this.closest('.bloodwork-item');

        bloodworkItem.querySelectorAll('div').forEach(testDiv => {
            const testDetails = testDiv.textContent.trim();
            if (testDetails) {
                allTestResults += testDetails + "\n";
            }
        });
        const prompt = `Let's talk about bloodwork. Here is a list of all test results, separated by new lines. Now provide a detailed report on the bloodwork:\n${allTestResults}`;
        const userMessage = document.createElement('div');
        userMessage.className = 'message user-message';
        userMessage.textContent = prompt;
        document.getElementById('chatOutput').appendChild(userMessage);

        messages.push({ role: 'user', content: prompt });

        fetch('/api/LetsChat/GetChatResponse', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ messages: messages })
        })
            .then(response => response.json())
            .then(data => {
                try {
                    messages.push({ role: 'system', content: data.message });

                    const chatMessages = document.querySelectorAll('#chatOutput .message');
                    chatMessages.forEach(messageElement => {
                        const role = messageElement.classList.contains('bot-message') ? 'system' : 'user';
                        const content = messageElement.textContent;
                        messages.push({ role: role, content: content });
                    });

                    const converter = new showdown.Converter();
                    const htmlResponse = converter.makeHtml(data.message);

                    const responseContainer = document.createElement('div');
                    responseContainer.className = 'message bot-message';
                    responseContainer.innerHTML = `<div class="response-content">${htmlResponse}</div>`;

                    resetUI();
                    document.getElementById('chatOutput').appendChild(responseContainer);


                } catch (synchronousError) {
                    displayErrorMessage(`Error: <strong>${synchronousError.message}</strong>`);
                    resetUI();
                }
            })
            .catch(error => {
                console.error('Error:', error);
                resetUI();
            });
    });
});
