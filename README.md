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