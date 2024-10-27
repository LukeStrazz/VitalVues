document.getElementById('generateMealsButton').addEventListener('click', function () {
    const generateMealsButton = document.getElementById('generateMealsButton');
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

    const prompt = `I need meal suggestions considering these allergies: ${allergies}. Please suggest meals in this format:
    {
        "MealName": "",
        "DayOfWeek": "",
        "MealType": "",
        "MealDescription": "",
        "Ingredients": "",
        "PrepTime": (int)
    } for each meal.`;

    inputField.value = '';
    inputField.style.height = '50px';
    inputField.disabled = true;
    spinner.style.display = 'inline-block';
    buttonText.textContent = 'Generating meals...';
    sendButton.disabled = true;

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
                const chatMessages = document.getElementById('generatedMeals');
                chatMessages.innerHTML = ''; 

                const meals = JSON.parse(data.message);

                meals.forEach(meal => {
                    const mealDiv = document.createElement('div');
                    mealDiv.className = 'meal-suggestion';

                    const mealContent = `
                    <strong>Meal Name:</strong> ${meal.MealName} <br>
                    <strong>Day of Week:</strong> ${meal.DayOfWeek} <br>
                    <strong>Meal Type:</strong> ${meal.MealType} <br>
                    <strong>Description:</strong> ${meal.MealDescription} <br>
                    <strong>Ingredients:</strong> ${meal.Ingredients} <br>
                    <strong>Prep Time:</strong> ${meal.PrepTime} mins <br>
                `;

                    mealDiv.innerHTML = mealContent;

                    const addButton = document.createElement('button');
                    addButton.textContent = 'Add to meals';
                    addButton.className = 'add-meal-button';
                    addButton.onclick = function () {
                        addToMealsTable(meal);
                    };

                    mealDiv.appendChild(addButton);

                    chatMessages.appendChild(mealDiv);
                });

                resetUI();
            } catch (error) {
                console.error('Error processing response:', error);
            } finally {
                generateMealsButton.disabled = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            resetUI();
        });
});

function addToMealsTable(meal) {
    fetch('/Meal/CreateMeal', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(meal)
    })
        .then(response => response.json())
        .then(data => {
            alert('Meal added successfully');
        })
        .catch(error => {
            console.error('Error adding meal:', error);
        });
}
