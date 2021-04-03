# Hector 

![alt text](https://github.com/0x78654C/Hector/blob/main/Hector/Resources/hector_2.png?raw=true)

 
Hector - A Discord bot in csharp WPF made from ideas from all my friends. Even the name is choosen by them ;).





This Discord bot project was made entirely on stream as fun. I'm not a professional programmer and I do this as a hobby but when I put something in my mind I try solve/create it.

using following libs:

https://github.com/discord-net/Discord.Net (for api connect)

https://github.com/davcs86/csharp-uhwid (for oath hwid encrypt/decrypt)

https://github.com/GuOrg/Gu.Wpf.Adorners (for watermarks)
_____________________________________________________

**Requirement**: 

.NET Framework 4.7
The libs mentioned upper can be installed via NuGet in Visual Studio

_____________________________________________________


Project ongoing live on: https://www.twitch.tv/x_coding

**Features**:

    show status if client is connected/reconection
    saveing Discord Bot user oAuth and openweather api key to registry
    Commands:
    	 !botname - Shows the one who gave the name of this bot!
 		 !hector - Displays something about this bot!
 		 !weather - Displays the weather from a specific City. Example: !weather cityname
 		 !++  - Adds Yanni points to users/strings(text). Example: !++ @username
 		 !--  - Removes Yanni points to user/strings(text). Example: !-- @username
 		 !r  - Shows how many Yanni points has an user/string(text). Example: !r @username/string(text) or just !r for self points!
 		 !t10  - Display the Top 10 users/strings(text) with Yanni points
 		 !8ball  - Magic 8Ball game. Ex: !8ball Should I get a car?
 	Yanni points can be edited in real time from GUI interface.
 	Possibility to auto reconnect on windows restart
 	--------------This Secret Key Game--------------
 	This Secret Key Game is a capture the flag game.
	First who finds the Secret Key by moveing aorund the map will
	win a coin and position of players is reseted and all will start from middle of the map. Commands for move and ranks:
 		 !e - Player moves East
 		 !w - Player moves West
 		 !n - Player moves North
 		 !s - Player moves South
 		 !c - Shows how many coins a user has. Exmaple: !c @username or just !c for self display coins!
 		 !s10 - Display the Top 10 with Secret Key coins!
 	------------------------------------------------



(more to come)
