function generateWorkouts() {
 isRunning = true;
 
 const generateWorkoutsButton = document.getElementById('generateWorkoutsButton');
 generateWorkoutsButton.disabled = true;

 const userInput = document.getElementById('promptInput').value;
 console.log(userInput);

 const inputField = document.getElementById('promptInput');
 const spinner = document.getElementById('spinner');
 const sendButton = document.getElementById('sendButton');
 const buttonText = document.getElementById('buttonText');


 

 const prompt = ` I need workout suggestions. Please suggest workouts based on user expectation if there is any: Here is the User input: ${userInput}
 There are 5 fields: Type, SubType, Set, Rep, and Day.
 "Type" you can choose from: "Build", "Maintain".
 "SubType" you can choose according to Type. you are not limited for the SubType. ex: Push-Up, Pull-Up, Squat, Deadlift etc.w
 "Set" can be most 5.
 "Rep" can be most 20.
 "Day" can be any day in the week. Only restriction is that it needs to start with capital. ex:Modnay.
 NOW I WILL PROVIDE YOU THE DATA STRUCTURE THAT IS REQUESTED. YOU WONT MIX TWO DATA TYPE. ITS EITHER INT OR STRING. NOT 30 SECONDS, JUST 30 . SO NUMBERS AND STRINGS ARE NOT ALLOWED TO BE IN THE SAME FIELD.

 {
     "Type": "",
     "SubType": "",
     "Set": (int),
     "Rep": (int),
     "Day": ""

 } for each workout. This is an expected output for a workout: Here are some workout suggestions:\n\n\u0060\u0060\u0060json\n[\n    {\n        \u0022Type\u0022: \u0022Build\u0022,\n        \u0022SubType\u0022: \u0022Push Up\u0022,\n        \u0022Set\u0022: 5,\n        \u0022Rep\u0022: 10,\n      \u0022Day\u0022: \u0022Tuesday\u0022\n   },...`;


 inputField.value = '';
 inputField.style.height = '50px';
 inputField.disabled = true;
 spinner.style.display = 'inline-block';
 buttonText.textContent = 'Generating workouts...';
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

             const chatMessages = document.getElementById('generatedWorkouts');
             chatMessages.innerHTML = '';

             data.forEach(workout => {
                 const workoutDiv = document.createElement('div');
                 workoutDiv.className = 'workout-suggestion';

                 const workoutContent = `
                 <strong>Type:</strong> ${workout.Type} <br>
                 <strong>SubType:</strong> ${workout.SubType} <br>
                 <strong>Set:</strong> ${workout.Set} <br>
                 <strong>Rep:</strong> ${workout.Rep} <br>
                 <strong>Day:</strong> ${workout.Day} <br>

             `;

             workoutDiv.innerHTML = workoutContent;

                 const addButton = document.createElement('button');
                 addButton.textContent = 'Add to workouts';
                 addButton.className = 'add-workout-button';
                 addButton.onclick = function () {
                     addToWorkout(workout);
                 };

                 workoutDiv.appendChild(addButton);
                 chatMessages.appendChild(workoutDiv);
                 messages.push({ role: 'system', content: workoutContent });
                 const converter = new showdown.Converter();
                 const htmlResponse = converter.makeHtml(workoutContent);

                 const responseContainer = document.createElement('div');
                 responseContainer.className = 'message bot-message';
                 responseContainer.innerHTML = `<div class="response-content">${htmlResponse}</div>`;
                 

                 resetUI();
             });

             resetUI();
         } catch (error) {
             console.error('Error processing response:', error);
             displayErrorMessage("Error parsing workout suggestions. Please try again.");
             resetUI();
         } finally {
          generateWorkoutsButton.disabled = false;
         }
     })
     .catch(error => {
         console.error('Error:', error);
         displayErrorMessage("Error fetching workout suggestions. Please try again.");
         resetUI();
     });
}


function addToWorkout(workout) {

  const workoutData = {
    userSecretId: document.getElementById("userSecretId").value,
    Type: workout.Type,
    SubType: workout.SubType,
    Day: workout.Day,
    Set: workout.Set,
    Rep: workout.Rep,
    Duration: 0,
    resolved: false,
  }

 fetch('/api/WorkoutController/CreateWorkout', {
     method: 'POST',
     headers: {
         'Content-Type': 'application/json'
     },
     body: JSON.stringify(workoutData) 
 })
     .then(response => response.json())
     .then(data => {
         alert('Workout added successfully');
     })
     .catch(error => {
         console.error('Error adding workout:', error);
     });
}