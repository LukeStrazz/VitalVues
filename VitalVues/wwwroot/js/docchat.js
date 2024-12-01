function generateRecs() {
    isRunning = true;

    const generateMealsButton = document.getElementById('generateRecsButton');
    generateMealsButton.disabled = true;

    const inputField = document.getElementById('promptInput');
    const spinner = document.getElementById('spinner');
    const sendButton = document.getElementById('sendButton');
    const buttonText = document.getElementById('buttonText');
    let allergies = '';

    document.querySelectorAll('.diet-interactions div').forEach(allergyDiv => {
        if (allergyDiv.textContent) {
            allergies += allergyDiv.textContent.trim() + ", ";
        }
    });

    const prompt = `I need doctor and suppliment suggestions considering these allergies and my most recent blood work test with results: ${allergies}. DO NOT TALK ABOUT MY BLOODWORK AND TESTS INDIVIDUALLY`;


    inputField.value = '';
    inputField.style.height = '50px';
    inputField.disabled = true;
    spinner.style.display = 'inline-block';
    buttonText.textContent = 'Generating recommendations...';
    sendButton.disabled = true;

    const userMessage = document.createElement('div');
    userMessage.className = 'message user-message';
    userMessage.textContent = prompt;

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


                generateMealsButton.disabled = false;
            } catch (synchronousError) {
                displayErrorMessage(`Error: <strong>${synchronousError.message}</strong>`);
                resetUI();

                generateMealsButton.disabled = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            resetUI();
        });
}