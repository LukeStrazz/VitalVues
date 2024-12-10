function generateMeals() {
    isRunning = true;

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

    const prompt = `I need meal suggestions considering these allergies if any: If no allergies listed here assume none.${allergies} . Please suggest meals in this format:
    {
        "MealName": "",
        "DayOfWeek": "",
        "MealType": "",
        "MealDescription": "",
        "Ingredients": ["", ""],
        "PrepTime": (int)
    } for each meal. This is an expected output for a meal: Here are some meal suggestions that consider your allergies:\n\n\u0060\u0060\u0060json\n[\n    {\n        \u0022MealName\u0022: \u0022Grilled Lemon Herb Chicken\u0022,\n        \u0022DayOfWeek\u0022: \u0022Monday\u0022,\n        \u0022MealType\u0022: \u0022Dinner\u0022,\n        \u0022MealDescription\u0022: \u0022Succulent grilled chicken marinated in lemon and mixed herbs, served with a side of roasted vegetables.\u0022,\n        \u0022Ingredients\u0022: [\u0022Chicken breast\u0022, \u0022Lemon juice\u0022, \u0022Olive oil\u0022, \u0022Garlic\u0022, \u0022Thyme\u0022, \u0022Vegetables (zucchini, bell peppers, carrots)\u0022],\n        \u0022PrepTime\u0022: 30\n    },...`;


    inputField.value = '';
    inputField.style.height = '50px';
    inputField.disabled = true;
    spinner.style.display = 'inline-block';
    buttonText.textContent = 'Generating meals...';
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
        .then(response => response.text())
        .then(text => {
            let data = [];
            console.log('Full response text:', text);

            try {
                const jsonMatch = text.match(/\[.*\]/s);

                if (jsonMatch) {
                    const jsonString = jsonMatch[0]
                        .replace(/\\u0022/g, '"') 
                        .replace(/\\n/g, '');

                    data = JSON.parse(jsonString);
                } else {
                    throw new Error("No valid JSON found in response");
                }

                const chatMessages = document.getElementById('generatedMeals');
                chatMessages.innerHTML = '';

                data.forEach(meal => {
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
                    messages.push({ role: 'system', content: mealContent });
                    const converter = new showdown.Converter();
                    const htmlResponse = converter.makeHtml(mealContent);

                    const responseContainer = document.createElement('div');
                    responseContainer.className = 'message bot-message';
                    responseContainer.innerHTML = `<div class="response-content">${htmlResponse}</div>`;
                    

                    resetUI();
                });

                resetUI();
            } catch (error) {
                console.error('Error processing response:', error);
                displayErrorMessage("Error parsing meal suggestions. Please try again.");
                resetUI();
            } finally {
                generateMealsButton.disabled = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            displayErrorMessage("Error fetching meal suggestions. Please try again.");
            resetUI();
        });
}


function addToMealsTable(meal) {
    fetch('/api/MealController/CreateMeal', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            MealName: meal.MealName,
            DayOfWeek: meal.DayOfWeek,
            MealType: meal.MealType,
            MealDescription: meal.MealDescription,
            Ingredients: meal.Ingredients,
            PrepTime: meal.PrepTime
        })
    })
        .then(response => response.json())
        .then(data => {
            alert('Meal added successfully');
        })
        .catch(error => {
            console.error('Error adding meal:', error);
        });
}