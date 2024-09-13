Many people receive blood work results without having any idea what the results actually mean. We believe this is a result of many people not being informed or educated thoroughly on what each test entails. But even if they are informed there are just too many tests for a person to keep track of and remember what each one tests for. We want everyone to be able to get their bloodwork results back and be able to view it without having to feel uneasy waiting to hear back from the doctor. In order to help, we decided to create a program that assists people in helping them understand their bloodwork results immediately by providing them with a clear and concise explanation of each test.

This program that we are currently working on is a tool created to help people understand their bloodwork and give them a clear mind immediately without having to be constantly waiting for a doctor to explain the results. When the user uploads their results not only will they see their results explained to them in detail but there will be other features implemented to the program. The AI in the program will help the user come up with goals, diet plans and a workout routine. These will be created depending on the results of the test of and what the user lacks or has too much of. There will also be a supplement recommender and an appointment recommender. These recommendations will be crafted by AI depending on what needs need to be met for the user.

This program is also a great way to keep track of the user’s progression of their health. When uploading the bloodwork results to the program, the program will store all that information into the database. In doing so the user will be able to access previous results and compare them to recent ones showing them the progress in between each blood work test. There will also be a fasting tracker and a BMI tracker to help users monitor progress. These are only a few of the features that the program has to offer. Overall, the program uses medical information, AI help, and personalized recommendation to make it easier for users to understand their blood test results and make better choices about their health.

<hr>

Needed tools:
MSSQL: https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x409&culture=en-us&country=us

.Net 8 SDK (Included with Visual Studio): https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false

To run program locally:
After Visual Studio and MSSQL are installed and ready,
1) Clone repo, run "> git clone https://github.com/LukeStrazz/VitalVues"
2) Open project from .sln file
3) In the Package Manager Console run"> sqllocaldb create AiDB"
4) Copy secrets from Drive, put them in appsettings.Development.json. Ensure location matches.
5) Migration will be made on run.
6) Authorization will take you to Auth0, and your user will be given a secret Id that will be used for the program.

For Mac users, use Ubuntu VM, follow: https://github.com/seed-labs/seed-labs/blob/master/lab-setup/apple-arm/seedvm-fusion.md

Follow: https://www.youtube.com/watch?v=co6TkSzc0s8 for MSSQL.

Authorization will not work unless IIS Express is configured. 