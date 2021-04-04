using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Hector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //declare Discord Socket client
        private DiscordSocketClient _client;
        //-----------------------------------------------

        //data and log directory declare
        readonly static string dataDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data";
        readonly static string logDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log";
        readonly static string logErrorDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log\errors";
        //------------------------------------------------


        //Declare keyname and subkey
        readonly static string keyName = "Hector";
        private static string d_Token;
        //------------------------------------------------

        //Define the background worker for bot start and random message
        BackgroundWorker worker;
        //--------------------------------

        //declare date variable
        private static string date;
        //---------------------------------

        //declare the bot forms variables
        settings sT;
        about aB;
        yanni_management yM;
        secretkey_game_manage sKG;
        //--------------------------------
        //declare timer for load icons and variables read value
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //--------------------------------

        //declare weather event variables
        private static string apiKey;
        static readonly HttpClient clientH = new HttpClient();
        //--------------------------------

        //declare variables for YanniBoi point system and ranking
        readonly static string user_Points = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\user_points.txt";
        readonly static string user_DateHistory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\user_DateHistory.txt";
        //private static string _pDate; //-disabled for future work
        //--------------------------------

        //menu status string control declaration
        private static string menuStatus = "0";
        //--------------------------------

        //declare variables fro 8Ball game by CodingWithScott
        readonly static string ballAnswer = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\8ball_answers.txt";
        Random r = new Random();
        //--------------------------------

        //declare variables for bot open windows startup
        private static string wStart;
        System.Windows.Threading.DispatcherTimer wStartTimer;
        //--------------------------------

        //Secret Key Game variables
        readonly static string PLAYER_DATA = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)  + @"\data\uData.txt";
        private static string GameDisplay;
        private static string sGame;
        private static string channelID;
        private static string sChannel;
        //--------------------------------

        public MainWindow()
        {
            InitializeComponent();

            //load connection icon
            this.Dispatcher.Invoke(() =>
            {
                statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/red_dot.png"));
            });
            //---------------------------------

            //data directory check and create if not exists
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            //------------------------------------------------


            //log/error directory check and create if not exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);

            }

            if (!Directory.Exists(logErrorDirectory))
            {
                Directory.CreateDirectory(logErrorDirectory);
            }
            //------------------------------------------------

            //point system files autocreate
            if (!File.Exists(user_Points))
            {
                File.WriteAllText(user_Points, "test|0");
            }

            if (!File.Exists(user_DateHistory))
            {
                File.Create(user_DateHistory);
            }
            //------------------------------------------------

            //8Ball system file autocreate
            if (!File.Exists(ballAnswer))
            {
                File.Create(ballAnswer);
            }
            //------------------------------------------------

            //SecretKey player data file create
            if (!File.Exists(PLAYER_DATA))
            {
                File.Create(PLAYER_DATA);
            }
            //------------------------------------------------

            //we check if regisrty variables exists and if not we create
            if (Reg.regKey_Read(keyName, "menuStatus") == "")
            {
                Reg.regKey_CreateKey(keyName, "menuStatus", "0");
            }

            if (Reg.regKey_Read(keyName, "d_Token") == "")
            {
                Reg.regKey_CreateKey(keyName, "d_Token", "0");
            }

            if (Reg.regKey_Read(keyName, "WeatherAPIKey") == "")
            {
                Reg.regKey_CreateKey(keyName, "WeatherAPIKey", "");
            }

            if (Reg.regKey_Read(keyName, "wStart") == "")
            {
                Reg.regKey_CreateKey(keyName, "wStart", "");
            }

            if (Reg.regKey_Read(keyName, "sGame") == "")
            {
                Reg.regKey_CreateKey(keyName, "sGame", "");
            }

            if (Reg.regKey_Read(keyName, "channelID") == "")
            {
                Reg.regKey_CreateKey(keyName, "channelID", "");
            }

            if (Reg.regKey_Read(keyName, "sChannel") == "")
            {
                Reg.regKey_CreateKey(keyName, "sChannel", "");
            }

            //--------------------------------

            //read variables values from registry
            menuStatus = Reg.regKey_Read(keyName, "menuStatus");
            apiKey = Reg.regKey_Read(keyName, "WeatherAPIKey");
            wStart = Reg.regKey_Read(keyName, "wStart");
            sGame = Reg.regKey_Read(keyName, "sGame");
            channelID = Reg.regKey_Read(keyName, "channelID");
            sChannel = Reg.regKey_Read(keyName, "sChannel");
            try
            {

                d_Token = Encryption._decryptData(Reg.regKey_Read(keyName, "d_Token"));

            }
            catch (Exception ex)
            {
                CLog.LogWriteError("Settigns - decrypt Discord Token: " + ex.ToString());
            }
            //---------------------------------

            //Menu state check and apply
            if (menuStatus == "1")
            {
                GridMenu.Width = 199;
                btnOpenMenu.Visibility = Visibility.Collapsed;
                btnCloseMenu.Visibility = Visibility.Visible;
                startBotBTN.Visibility = Visibility.Visible;
                logViewRTB.Margin = new Thickness(199, 50, 0, 0);

            }
            else
            {
                GridMenu.Width = 50;
                logViewRTB.Margin = new Thickness(50, 50, 0, 0);
                btnOpenMenu.Visibility = Visibility.Visible;
                btnCloseMenu.Visibility = Visibility.Collapsed;
                startBotBTN.Visibility = Visibility.Hidden;

            }
            //---------------------------------


            //variables load timer declaration
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += RegVarLoad;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //----------------------------------


            //variables windows startup timer declaration
            wStartTimer = new System.Windows.Threading.DispatcherTimer();
            wStartTimer.Tick += wStart_Tick;
            wStartTimer.Interval = new TimeSpan(0, 0, 1);
            if (wStart == "1")
            {
                if (_client == null)
                {
                    wStartTimer.Start();
                }
            }
            //----------------------------------

            //check internet connection
            if (!Network.inetCK())
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Internet is down! Try again later");
                CLog.LogWrite("[" + date + "] Internet is down! Try again later");
            }
            //----------------------------------


        }

        /// <summary>
        /// Function for background worker that calls the MainAsync
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BotConnect(object sender, DoWorkEventArgs e)
        {
            this.MainAsync().GetAwaiter().GetResult();

        }

        /// <summary>
        /// Main function for dicord bot accont to connect
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {

            this.Dispatcher.Invoke(() =>
            {
                statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/orange_dot.png"));
                startBotBTN.Content = "STOP";
            });

            try
            {
                _client = new DiscordSocketClient();

                _client.Log += Log;

                await _client.LoginAsync(TokenType.Bot, d_Token);
                _client.UserJoined += client_UserJoined;
                _client.MessageReceived += client_MessageReceived;
               
                await _client.StartAsync();;

                // Block this task until the program is closed.
                await Task.Delay(-1);
            }

            catch (Exception ex)
            {
                CLog.LogWriteError("Bot Connect: " + ex.ToString());
                this.Dispatcher.Invoke(() =>
                {
                    statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/red_dot.png"));
                    startBotBTN.Content = "START";
                });
            }


        }

        /// <summary>
        /// On user joined event 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task client_UserJoined(SocketGuildUser arg)
        {
            try
            {
                arg.SendMessageAsync("Welcome to my discord server ;). Let's have some fun and help weach other. Keept it cool and be nice to others! ;)");
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "][BOT] " + arg.Username + " has joined the channel!");
                CLog.LogWrite("[" + date + "][BOT] " + arg.Username + " has joined the channel!");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWrite("[" + date + "] User Joined Error: " + e.ToString());
            }
            return Task.CompletedTask;

        }

        /// <summary>
        /// Discrod message Received/Sent event
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task client_MessageReceived(SocketMessage arg)
        {
            var m = arg as SocketUserMessage;
            //!help command
            try
            {

                if (m.Content != null && m.Content == "!h")
                {
                    string commands = @"
**__List of commands__**:
 ****!botname**** - `Shows the one who gave the name of this bot!`
 ****!hector**** - `Displays something about this bot!`
 ****!weather**** - `Displays the weather from a specific City. Example: !weather cityname`
 ****!++ **** - `Adds Yanni points to user. Example: !++ @username`
 ****!-- **** - `Removes Yanni points to user. Example: !-- @username`
 ****!r **** - `Shows how many Yanni points has an user. Example: !r @username or just !r for self points!`
 ****!t10 **** - `Display the Top 10 users with Yanni points`
 ****!8ball **** - `Magic 8Ball game. Ex: !8ball Should I get a car?`

**__Secret Key Game__**
This Secret Key Game is a capture the flag game.
First who finds the Secret Key by moveing aorund the map will
win a coin and position of players is reseted and all will start from middle of the map. Commands for move and ranks:
 ****!e **** - `Player moves East`
 ****!w **** - `Player moves West`
 ****!n **** - `Player moves North`
 ****!s **** - `Player moves South`
 ****!c **** - `Shows how many coins a user has. Exmaple: !c @username or just !c for self display coins!`
 ****!s10 **** - `Display the Top 10 with Secret Key coins!`
";
                    logWrite(m.Author.ToString() + ": " + m.Content);
                    await arg.Channel.SendMessageAsync(commands, false, null);
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    logWrite("[BOT]: " + commands);
                    CLog.LogWrite("[" + date + "][BOT]: " + commands);
                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - hector Command: " + e.ToString());
            }
            //--------------------------------------------

            //!hecktor command
            try
            {

                if (m.Content != null && m.Content == "!hector")
                {
                    logWrite(m.Author.ToString() + ": " + m.Content);
                    await arg.Channel.SendMessageAsync("I'm your bot that will be made by your ideeas ;)", false, null);
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    logWrite("[BOT]: I'm your bot that will be made by your ideeas ;)");
                    CLog.LogWrite("[" + date + "][BOT]: I'm your bot that will be made by your ideeas ;)");
                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - hector Command: " + e.ToString());
            }
            //--------------------------------------------

            //!botname command
            try
            {

                if (m.Content != null && m.Content == "!botname")
                {
                    logWrite(m.Author.ToString() + ": " + m.Content);
                    await arg.Channel.SendMessageAsync("Thank you yanniboi for this name :* ;)", false, null);
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    logWrite("[BOT]: Thank you yanniboi for this name :* ;)");
                    CLog.LogWrite("[" + date + "][BOT]: Thank you yanniboi for this name :* ;)");
                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - botname Command: " + e.ToString());
            }
            //--------------------------------------------

            //!weather command
            try
            {

                if (m.Content != null && m.Content.StartsWith("!weather"))
                {
                    string[] cmd = m.Content.Split(' ');
                    if (cmd[1].Length > 0)
                    {
                        logWrite(m.Author.ToString() + ": " + m.Content);
                        await arg.Channel.SendMessageAsync("The weather in ****" + cmd[1] + "**** is: " + Environment.NewLine + "****Celsius****" + Environment.NewLine + weatherForecastMetric(cmd[1]) + Environment.NewLine + "****Fahrenheit****" + Environment.NewLine + weatherForecastImperial(cmd[1]), false, null);
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[BOT]: The weather in " + cmd[1] + " is: " + Environment.NewLine + "****Celsius****" + Environment.NewLine + weatherForecastMetric(cmd[1]) + Environment.NewLine + "****Fahrenheit****" + Environment.NewLine + weatherForecastImperial(cmd[1]));
                        CLog.LogWrite("[" + date + "][BOT]: The weather in " + cmd[1] + " is: " + Environment.NewLine + "****Celsius****" + Environment.NewLine + weatherForecastMetric(cmd[1]) + "****Fahrenheit****" + Environment.NewLine + weatherForecastImperial(cmd[1]));
                    }
                    else
                    {
                        await arg.Channel.SendMessageAsync("Oups! Please specify the City name for weather command. Example: !weather cityname", false, null);
                    }
                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - botname Command: " + e.ToString());
            }
            //--------------------------------------------

            #region Point System commands(by yanniboi)
            //!++ command
            try
            {

                if (m.Content != null && m.Content.StartsWith("!++"))
                {

                    if (m.Content.Contains(" "))
                    {
                        if (m.Content.Contains("<"))
                        {
                            string[] cmd = m.Content.Split(' ');
                            string b = string.Empty;

                            for (int i = 0; i < cmd[1].Length; i++)
                            {
                                if (Char.IsDigit(cmd[1][i]))
                                    b += cmd[1][i];
                            }

                            try
                            {
                                string eUser = _client.GetUser(Convert.ToUInt64(b)).ToString();
                                string ms = arg.MentionedUsers.ToList().ToString();
                                string[] u = eUser.Split('#');
                                string mUser = u[0];
                                string[] rUserPointsLines = File.ReadAllLines(user_Points);

                                List<string> pL = new List<string>();


                                foreach (var user in rUserPointsLines)
                                {

                                    pL.Add(user);
                                }


                                foreach (var line in pL.ToArray())
                                {

                                    if (line.Length > 0)
                                    {


                                        if (line.Contains(mUser))
                                        {
                                            string[] userline = line.Split('|');
                                            if (userline[0] != m.Author.Username)
                                            {
                                                int points = Convert.ToInt32(userline[1]);
                                                int p = points + 1;
                                                pL.Remove(line);
                                                string l = mUser + "|" + p;
                                                pL.Add(l);
                                                p = 0;
                                                await arg.Channel.SendMessageAsync("****" + mUser + "**** gained a Yanni point from ****" + m.Author.Username + "**** !", false, null);
                                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                logWrite("[" + date + "][BOT]" + mUser + " gained a Yanni point from " + m.Author.Username + "!");
                                                CLog.LogWrite("[" + date + "][BOT]" + mUser + " gained a Yanni point from " + m.Author.Username + "!");
                                            }
                                            else
                                            {
                                                await arg.Channel.SendMessageAsync("You cannot give yourself points, ****" + m.Author.Username + "**** !", false, null);
                                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                logWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");
                                                CLog.LogWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");

                                            }

                                        }

                                    }

                                }

                                var c = pL.FirstOrDefault(x => x.Contains(mUser));
                                if (c == null)
                                {
                                    if (mUser != m.Author.Username)
                                    {
                                        pL.Add(mUser + "|1");
                                        await arg.Channel.SendMessageAsync("****" + mUser + "**** gained a Yanni point from ****" + m.Author.Username + "**** !", false, null);
                                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        logWrite("[" + date + "][BOT]" + mUser + " gained a Yanni point from " + m.Author.Username + "!");
                                        CLog.LogWrite("[" + date + "][BOT]" + mUser + " gained a Yanni point from " + m.Author.Username + "!");
                                    }
                                    else
                                    {
                                        await arg.Channel.SendMessageAsync("You cannot give yourself points, ****" + m.Author.Username + "**** !", false, null);
                                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        logWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");
                                        CLog.LogWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");

                                    }
                                }

                                string finalPointsList = string.Join(Environment.NewLine, pL);
                                File.WriteAllText(user_Points, finalPointsList);



                            }
                            catch
                            {
                                await arg.Channel.SendMessageAsync("Mentioned user must be online for giving points!", false, null);
                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "]Mentioned user must be online for giving points!");
                                CLog.LogWrite("[" + date + "]Mentioned user must be online for giving points!");
                            }
                        }
                        else
                        {
                            string[] cmd = m.Content.Split(' ');

                            string[] rUserPointsLines = File.ReadAllLines(user_Points);

                            List<string> pL = new List<string>();


                            foreach (var user in rUserPointsLines)
                            {

                                pL.Add(user);
                            }


                            foreach (var line in pL.ToArray())
                            {

                                if (line.Length > 0)
                                {


                                    if (line.Contains(cmd[1]))
                                    {
                                        string[] userline = line.Split('|');
                                        if (userline[0] != m.Author.Username)
                                        {
                                            int points = Convert.ToInt32(userline[1]);
                                            int p = points + 1;
                                            pL.Remove(line);
                                            string l = cmd[1] + "|" + p;
                                            pL.Add(l);
                                            p = 0;
                                            await arg.Channel.SendMessageAsync("****" + cmd[1] + "**** gained a Yanni point from ****" + m.Author.Username + "**** !", false, null);
                                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                            logWrite("[" + date + "][BOT]" + cmd[1] + " gained a Yanni point from " + m.Author.Username + "!");
                                            CLog.LogWrite("[" + date + "][BOT]" + cmd[1] + " gained a Yanni point from " + m.Author.Username + "!");
                                        }
                                        else
                                        {
                                            await arg.Channel.SendMessageAsync("You cannot give yourself points, ****" + m.Author.Username + "**** !", false, null);
                                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                            logWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");
                                            CLog.LogWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");

                                        }

                                    }

                                }

                            }

                            var c = pL.FirstOrDefault(x => x.Contains(cmd[1]));
                            if (c == null)
                            {
                                if (cmd[1] != m.Author.Username)
                                {
                                    pL.Add(cmd[1] + "|1");
                                    await arg.Channel.SendMessageAsync("****" + cmd[1] + "**** gained a Yanni point from ****" + m.Author.Username + "**** !", false, null);
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT]" + cmd[1] + " gained a Yanni point from " + m.Author.Username + "!");
                                    CLog.LogWrite("[" + date + "][BOT]" + cmd[1] + " gained a Yanni point from " + m.Author.Username + "!");
                                }
                                else
                                {
                                    await arg.Channel.SendMessageAsync("You cannot give yourself points, ****" + m.Author.Username + "**** !", false, null);
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");
                                    CLog.LogWrite("[" + date + "][BOT] You cannot give yourself points, ****" + m.Author.Username + "**** !");

                                }
                            }

                            string finalPointsList = string.Join(Environment.NewLine, pL);
                            File.WriteAllText(user_Points, finalPointsList);



                        }
                    }
                    else
                    {
                        await arg.Channel.SendMessageAsync("The user must be mentioned with @ for run and must be a space between the command and user mentioned!", false, null);
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[" + date + "]The user must be mentioned with @ for run and must be a space between the command and user mentioned!");
                        CLog.LogWrite("[" + date + "]The user must be mentioned with @ for run and must be a space between the command and user mentioned!");
                    }

                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - !++ Command: " + e.ToString());
            }
            //--------------------------------------------

            //!-- command
            try
            {

                if (m.Content != null && m.Content.StartsWith("!--"))
                {
                    if (m.Content.Contains(" "))
                    {
                        if (m.Content.Contains("<"))
                        {
                            string[] cmd = m.Content.Split(' ');
                            string b = string.Empty;

                            for (int i = 0; i < cmd[1].Length; i++)
                            {
                                if (Char.IsDigit(cmd[1][i]))
                                    b += cmd[1][i];
                            }

                            try
                            {
                                string eUser = _client.GetUser(Convert.ToUInt64(b)).ToString();
                                string[] u = eUser.Split('#');
                                string mUser = u[0];
                                string[] rUserPointsLines = File.ReadAllLines(user_Points);
                                string UserPoints = File.ReadAllText(user_Points);
                                List<string> pL = new List<string>();


                                foreach (var user in rUserPointsLines)
                                {

                                    pL.Add(user);
                                }

                                bool c = false;
                                foreach (var line in pL.ToArray())
                                {

                                    if (line.Length > 0)
                                    {

                                        if (line.Contains(mUser))
                                        {
                                            string[] userline = line.Split('|');
                                            if (userline[0] != m.Author.Username)
                                            {
                                                int points = Convert.ToInt32(userline[1]);
                                                if (points != 0)
                                                {
                                                    int p = points - 1;
                                                    pL.Remove(line);
                                                    string l = mUser + "|" + p;
                                                    pL.Add(l);
                                                    p = 0;
                                                    await arg.Channel.SendMessageAsync("****" + m.Author.Username + "**** removed a Yanni point from ****" + mUser + "**** !", false, null);
                                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                    logWrite("[" + date + "]" + m.Author.Username + " removed a Yanni point from " + mUser + " !");
                                                    CLog.LogWrite("[" + date + "]" + m.Author.Username + " removed a Yanni point from " + mUser + " !");
                                                }
                                                else
                                                {
                                                    await arg.Channel.SendMessageAsync(" ****" + mUser + "**** has no points anymore !", false, null);
                                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                    logWrite("[" + date + "]" + mUser + " has no points anymore !");
                                                    CLog.LogWrite("[" + date + "]" + mUser + " has no points anymore !");
                                                }
                                            }
                                            else
                                            {
                                                await arg.Channel.SendMessageAsync("You cannot remove points from yourself, ****" + m.Author.Username + "**** !", false, null);
                                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                logWrite("[" + date + "][BOT] You cannot remove points from yourself, ****" + m.Author.Username + "**** !");
                                                CLog.LogWrite("[" + date + "][BOT] You cannot remove points from yourself, ****" + m.Author.Username + "**** !");
                                            }

                                        }


                                    }
                                }
                                if (!UserPoints.Contains(mUser))
                                {

                                    if (c == false)
                                    {

                                        await arg.Channel.SendMessageAsync(" ****" + mUser + "**** has no points anymore !", false, null);
                                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        logWrite("[" + date + "]" + mUser + " has no points anymore !");
                                        CLog.LogWrite("[" + date + "]" + mUser + " has no points anymore !");
                                        c = true;
                                    }
                                }
                                string finalPointsList = string.Join(Environment.NewLine, pL);
                                File.WriteAllText(user_Points, finalPointsList);

                            }
                            catch
                            {
                                await arg.Channel.SendMessageAsync("Mentioned user must be online for takeing points!", false, null);
                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "]Mentioned user must be online for takeing points!");
                                CLog.LogWrite("[" + date + "]Mentioned user must be online for takeing points!");
                            }
                        }
                        else
                        {
                            string[] cmd = m.Content.Split(' ');


                            string[] rUserPointsLines = File.ReadAllLines(user_Points);
                            string UserPoints = File.ReadAllText(user_Points);
                            List<string> pL = new List<string>();


                            foreach (var user in rUserPointsLines)
                            {

                                pL.Add(user);
                            }

                            bool c = false;
                            foreach (var line in pL.ToArray())
                            {

                                if (line.Length > 0)
                                {

                                    if (line.Contains(cmd[1]))
                                    {
                                        string[] userline = line.Split('|');
                                        if (userline[0] != m.Author.Username)
                                        {
                                            int points = Convert.ToInt32(userline[1]);
                                            if (points != 0)
                                            {
                                                int p = points - 1;
                                                pL.Remove(line);
                                                string l = cmd[1] + "|" + p;
                                                pL.Add(l);
                                                p = 0;
                                                await arg.Channel.SendMessageAsync("****" + m.Author.Username + "**** removed a Yanni point from ****" + cmd[1] + "**** !", false, null);
                                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                logWrite("[" + date + "]" + m.Author.Username + " removed a Yanni point from " + cmd[1] + " !");
                                                CLog.LogWrite("[" + date + "]" + m.Author.Username + " removed a Yanni point from " + cmd[1] + " !");
                                            }
                                            else
                                            {
                                                await arg.Channel.SendMessageAsync(" ****" + cmd[1] + "**** has no points anymore !", false, null);
                                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                                logWrite("[" + date + "]" + cmd[1] + " has no points anymore !");
                                                CLog.LogWrite("[" + date + "]" + cmd[1] + " has no points anymore !");
                                            }
                                        }
                                        else
                                        {
                                            await arg.Channel.SendMessageAsync("You cannot remove points from yourself, ****" + m.Author.Username + "**** !", false, null);
                                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                            logWrite("[" + date + "][BOT] You cannot remove points from yourself, ****" + m.Author.Username + "**** !");
                                            CLog.LogWrite("[" + date + "][BOT] You cannot remove points from yourself, ****" + m.Author.Username + "**** !");
                                        }

                                    }


                                }
                            }
                            if (!UserPoints.Contains(cmd[1]))
                            {

                                if (c == false)
                                {

                                    await arg.Channel.SendMessageAsync(" ****" + cmd[1] + "**** has no points anymore !", false, null);
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "]" + cmd[1] + " has no points anymore !");
                                    CLog.LogWrite("[" + date + "]" + cmd[1] + " has no points anymore !");
                                    c = true;
                                }
                            }
                            string finalPointsList = string.Join(Environment.NewLine, pL);
                            File.WriteAllText(user_Points, finalPointsList);


                        }

                    }
                    else
                    {
                        await arg.Channel.SendMessageAsync("The user must be mentioned with @ for run and must be a space between the command and user mentioned!", false, null);
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[" + date + "]The user must be mentioned with @ for run and must be a space between the command and user mentioned!");
                        CLog.LogWrite("[" + date + "]The user must be mentioned with @ for run and must be a space between the command and user mentioned!");
                    }

                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - botname Command: " + e.ToString());
            }
            //--------------------------------------------

            //!rank command
            try
            {

                if (m.Content != null && m.Content.StartsWith("!r"))
                {
                    string[] rUserPointsLines = File.ReadAllLines(user_Points);
                    string rUserPints = File.ReadAllText(user_Points);
                    if (m.Content.Contains(" "))
                    {
                        string[] mu = m.Content.Split(' ');
                        if (mu[0] == "!r")
                        {
                            if (mu[1] != null)
                            {
                                if (mu[1].Contains("<"))
                                {
                                    string b = string.Empty;

                                    for (int i = 0; i < mu[1].Length; i++)
                                    {
                                        if (Char.IsDigit(mu[1][i]))
                                            b += mu[1][i];
                                    }
                                    try
                                    {
                                        string eUser = _client.GetUser(Convert.ToUInt64(b)).ToString();
                                        string[] u = eUser.Split('#');
                                        string mUser = u[0];


                                        string user = string.Empty;

                                        foreach (var line in rUserPointsLines)
                                        {

                                            if (line.Contains(mUser))
                                            {
                                                string[] rank = line.Split('|');

                                                user = "****" + rank[0] + "****, has ****" + rank[1] + "**** Yanni points!";
                                            }

                                        }

                                        if (!rUserPints.Contains(mUser))
                                        {
                                            user = "****" + mUser + "****, has ****0**** Yanni points!";
                                        }

                                        await arg.Channel.SendMessageAsync(user, false, null);
                                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        logWrite("[" + date + "][BOT]: " + user);
                                        CLog.LogWrite("[" + date + "][BOT]: " + user);

                                    }
                                    catch (Exception ex)
                                    {
                                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                        CLog.LogWriteError("[" + date + "]Error - rank Command: " + ex.ToString());
                                    }
                                }
                                else
                                {

                                    string user = string.Empty;

                                    foreach (var line in rUserPointsLines)
                                    {

                                        if (line.Contains(mu[1]))
                                        {
                                            string[] rank = line.Split('|');

                                            user = "****" + rank[0] + "****, has ****" + rank[1] + "**** Yanni points!";
                                        }

                                    }

                                    if (!rUserPints.Contains(mu[1]))
                                    {
                                        user = "****" + mu[1] + "****, has ****0**** Yanni points!";
                                    }

                                    await arg.Channel.SendMessageAsync(user, false, null);
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT]: " + user);
                                    CLog.LogWrite("[" + date + "][BOT]: " + user);


                                }
                            }
                            else
                            {



                                string user = string.Empty;

                                foreach (var line in rUserPointsLines)
                                {

                                    if (line.Contains(m.Author.Username))
                                    {
                                        string[] rank = line.Split('|');

                                        user = "****" + rank[0] + "****, you have ****" + rank[1] + "**** Yanni points!";
                                    }


                                }
                                if (!rUserPints.Contains(m.Author.Username))
                                {
                                    user = "****" + m.Author.Username + "****, has ****0**** Yanni points!";
                                }

                                await arg.Channel.SendMessageAsync(user, false, null);
                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT]: " + user);
                                CLog.LogWrite("[" + date + "][BOT]: " + user);
                            }
                        }
                        else
                        {
                            await arg.Channel.SendMessageAsync("The command for Yanni points rank must start with !r", false, null);
                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            logWrite("[" + date + "][BOT]: The command for Yanni points rank must start with !r");
                            CLog.LogWrite("[" + date + "][BOT]: The command for Yanni points rank must start with !r");
                        }
                    }
                    else
                    {



                        string user = string.Empty;

                        foreach (var line in rUserPointsLines)
                        {

                            if (line.Contains(m.Author.Username))
                            {
                                string[] rank = line.Split('|');

                                user = "****" + rank[0] + "****, you have ****" + rank[1] + "**** Yanni points!";
                            }

                        }


                        if (!rUserPints.Contains(m.Author.Username))
                        {
                            user = "****" + m.Author.Username + "****, has ****0**** Yanni points!";
                        }
                        await arg.Channel.SendMessageAsync(user, false, null);
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[" + date + "][BOT]: " + user);
                        CLog.LogWrite("[" + date + "][BOT]: " + user);
                    }
                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - rank Command: " + e.ToString());
            }
            //--------------------------------------------

            //!t10 command
            try
            {

                if (m.Content != null && m.Content == "!t10")
                {
                    string[] rUserPointsLines = File.ReadAllLines(user_Points);
                    string rUserPints = File.ReadAllText(user_Points);
                    List<string> pL = new List<string>();
                    foreach (var line in rUserPointsLines)
                    {
                        string[] t = line.Split('|');
                        pL.Add(t[1] + "|" + t[0]);
                    }

                    if (!m.Content.Contains(" "))
                    {
                        pL.Sort((a, b) => b.CompareTo(a));
                        string outs = string.Empty;
                        int count = 0;
                        foreach (var item in pL.ToArray())
                        {

                            string[] t = item.Split('|');
                            count++;
                            if (count <= 10 && t[0] != "0")
                            {
                                outs += "****" + t[1] + "**** has ****" + t[0] + "****" + Environment.NewLine;
                            }
                        }
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[" + date + "][BOT] Top 10 Yanni points list: " + Environment.NewLine + outs);
                        await arg.Channel.SendMessageAsync("**__Top 10 Yanni points list:__** " + Environment.NewLine + Environment.NewLine + outs, false, null);
                        CLog.LogWrite("[" + date + "][BOT] Top 10 Yanni points list: " + Environment.NewLine + outs);
                    }

                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - rank Command: " + e.ToString());
            }
            //--------------------------------------------
            #endregion

            #region 8Ball by CodingWithScott
            //!8ball command
            try
            {

                if (m.Content != null && m.Content.StartsWith("!8ball"))
                {
                    if (m.Content.Contains("?"))
                    {

                        List<string> randomM = new List<string>();
                        if (File.Exists(ballAnswer))
                        {
                            bool c = false;
                            string[] rand_list = File.ReadAllLines(ballAnswer);
                            foreach (var line in rand_list)
                            {
                                if (line.Length > 0)
                                {
                                    randomM.Add(line);
                                }
                                else
                                {
                                    if (c == false)
                                    {
                                        logWrite("[" + date + "][BOT] 8Ball - Answers files is empty! You need to add something");
                                        c = true;
                                    }
                                }
                            }
                            int index = r.Next(randomM.Count);
                            string rand = randomM[index];
                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            logWrite("[" + date + "][BOT] Scott says: " + rand);
                            await arg.Channel.SendMessageAsync("****Scott**** says: " + rand, false, null);
                            CLog.LogWrite("[" + date + "][BOT] Scott says: " + rand);
                        }
                    }
                    else
                    {
                        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        logWrite("[" + date + "][BOT] Your question must contain ? for Scott to answer!");
                        await arg.Channel.SendMessageAsync("Your question must contain ? for ****Scott**** to answer!", false, null);
                        CLog.LogWrite("[" + date + "][BOT] Your question must contain ? for Scott to answer!");
                    }

                }
            }
            catch (Exception e)
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                CLog.LogWriteError("[" + date + "]Error - !8ball Command: " + e.ToString());
            }
            //--------------------------------------------


            #endregion

            #region Secret Key Game by Scott
            if (sGame == "1")
            {
                //movement commands
                try
                {
                    if (channelID != "")
                    {
                        ulong id = 0;
                        
                        if (sChannel == "1")
                        {
                             id = Convert.ToUInt64(channelID);
                        }
                        else
                        {
                             id = arg.Channel.Id;
                        }
                  

                        if (m.Content != null && m.Content == "!e")
                        {
                            if (arg.Channel.ToString() == _client.GetChannel(id).ToString())
                            {
                                GameDisplay = scott_game_engine.Game_Play(m.Author.Username, "east", PLAYER_DATA);
                                if (!GameDisplay.Contains("Win"))
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite(GameDisplay);
                                    await arg.Channel.SendMessageAsync(GameDisplay, false, null);
                                }
                                else
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT] Congratulation " + m.Author.Username + " you have won the game!");
                                    await arg.Channel.SendMessageAsync("Congratulation ****" + m.Author.Username + "****, you have found the Secret Key! Therefore I give you a mistic coin.", false, null);
                                }
                            }
                            else
                            {

                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT] To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!");
                                await arg.Channel.SendMessageAsync("To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!", false, null);

                            }

                        }
                        else if (m.Content != null && m.Content == "!w")
                        {
                            if (arg.Channel.ToString() == _client.GetChannel(id).ToString())
                            {
                                GameDisplay = scott_game_engine.Game_Play(m.Author.Username, "west", PLAYER_DATA);
                                if (!GameDisplay.Contains("Win"))
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite(GameDisplay);
                                    await arg.Channel.SendMessageAsync(GameDisplay, false, null);
                                }
                                else
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT] Congratulation " + m.Author.Username + " you have won the game!");
                                    await arg.Channel.SendMessageAsync("Congratulation ****" + m.Author.Username + "****, you have found the Secret Key! Therefore I give you a mistic coin.", false, null);
                                }
                            }
                            else
                            {

                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT] To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!");
                                await arg.Channel.SendMessageAsync("To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!", false, null);

                            }
                        }
                        else if (m.Content != null && m.Content == "!n")
                        {
                            if (arg.Channel.ToString() == _client.GetChannel(id).ToString())
                            {
                                GameDisplay = scott_game_engine.Game_Play(m.Author.Username, "north", PLAYER_DATA);
                                if (!GameDisplay.Contains("Win"))
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite(GameDisplay);
                                    await arg.Channel.SendMessageAsync(GameDisplay, false, null);
                                }
                                else
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT] Congratulation " + m.Author.Username + " you have won the game!");
                                    await arg.Channel.SendMessageAsync("Congratulation ****" + m.Author.Username + "****, you have found the Secret Key! Therefore I give you a mistic coin.", false, null);
                                }
                            }
                            else
                            {

                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT] To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!");
                                await arg.Channel.SendMessageAsync("To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!", false, null);

                            }

                        }
                        else if (m.Content != null && m.Content == "!s")
                        {
                            if (arg.Channel.ToString() == _client.GetChannel(id).ToString())
                            {
                                GameDisplay = scott_game_engine.Game_Play(m.Author.Username, "south", PLAYER_DATA);
                                if (!GameDisplay.Contains("Win"))
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite(GameDisplay);
                                    await arg.Channel.SendMessageAsync(GameDisplay, false, null);
                                }
                                else
                                {
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT] Congratulation " + m.Author.Username + " you have won the game!");
                                    await arg.Channel.SendMessageAsync("Congratulation ****" + m.Author.Username + "****, you have found the Secret Key! Therefore I give you a mistic coin.", false, null);
                                }
                            }
                            else
                            {

                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT] To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!");
                                await arg.Channel.SendMessageAsync("To play The Secret Key Game you need to go at " + _client.GetChannel(id).ToString() + " channel!", false, null);

                            }
                        }
                    }

                }

                catch (Exception ex)
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    logWrite("[" + date + "] Game East error: " + ex.ToString());

                }

                //--------------------------------

                //!s10 command
                try
                {

                    if (m.Content != null && m.Content == "!s10")
                    {
                        string[] rUserSP = File.ReadAllLines(PLAYER_DATA);
                        List<string> pL = new List<string>();
                        foreach (var line in rUserSP)
                        {
                            string[] t = line.Split('|');
                            t[2] = t[2].Trim();
                            pL.Add(t[2] + "|" + t[0]);
                        }

                        if (!m.Content.Contains(" "))
                        {
                            pL.Sort((a, b) => b.CompareTo(a));
                            string outs = string.Empty;
                            int count = 0;
                            foreach (var item in pL.ToArray())
                            {

                                string[] t = item.Split('|');
                                count++;
                                if (count <= 10 && t[0] != "0")
                                {
                                    outs += "****" + t[1] + "**** has ****" + t[0] + "**** coins" + Environment.NewLine;
                                }
                            }
                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            logWrite("[" + date + "][BOT] Top 10 Secret Key Game coins list: " + Environment.NewLine + outs);
                            await arg.Channel.SendMessageAsync("**__Top 10 Secret Key Game coins list:__** " + Environment.NewLine + Environment.NewLine + outs, false, null);
                            CLog.LogWrite("[" + date + "][BOT] Top 10 Secret Key Game coins list: " + Environment.NewLine + outs);
                        }

                    }
                }
                catch (Exception e)
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    CLog.LogWriteError("[" + date + "]Error - secret_Grank Command: " + e.ToString());
                }
                //--------------------------------------------



                //!s rank Secret Game command
                try
                {

                    if (m.Content != null && m.Content.StartsWith("!c"))
                    {
                        string[] rUserCoinsLines = File.ReadAllLines(PLAYER_DATA);
                        string rUserCoins = File.ReadAllText(PLAYER_DATA);
                        if (m.Content.Contains(" "))
                        {
                            string[] mu = m.Content.Split(' ');
                            if (mu[0] == "!c")
                            {
                                if (mu[1] != null)
                                {
                                    if (mu[1].Contains("<"))
                                    {
                                        string b = string.Empty;

                                        for (int i = 0; i < mu[1].Length; i++)
                                        {
                                            if (Char.IsDigit(mu[1][i]))
                                                b += mu[1][i];
                                        }
                                        try
                                        {
                                            string eUser = _client.GetUser(Convert.ToUInt64(b)).ToString();
                                            string[] u = eUser.Split('#');
                                            string mUser = u[0];


                                            string user = string.Empty;

                                            foreach (var line in rUserCoinsLines)
                                            {

                                                if (line.Contains(mUser))
                                                {
                                                    string[] rank = line.Split('|');

                                                    user = "****" + rank[0] + "****, has ****" + rank[2] + "**** coins!";
                                                }

                                            }

                                            if (!rUserCoins.Contains(mUser))
                                            {
                                                user = "****" + mUser + "****, has ****0**** Coins!";
                                            }

                                            await arg.Channel.SendMessageAsync(user, false, null);
                                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                            logWrite("[" + date + "][BOT]: " + user);
                                            CLog.LogWrite("[" + date + "][BOT]: " + user);

                                        }
                                        catch (Exception ex)
                                        {
                                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                            CLog.LogWriteError("[" + date + "]Error - Game rank Command: " + ex.ToString());
                                        }
                                    }
                                }
                                else
                                {

                                    string user = string.Empty;

                                    foreach (var line in rUserCoinsLines)
                                    {

                                        if (line.Contains(m.Author.Username))
                                        {
                                            string[] rank = line.Split('|');

                                            user = "****" + rank[0] + "****, you have ****" + rank[2] + "**** coins!";
                                        }


                                    }
                                    if (!rUserCoins.Contains(m.Author.Username))
                                    {
                                        user = "****" + m.Author.Username + "****, has ****0**** coins!";
                                    }

                                    await arg.Channel.SendMessageAsync(user, false, null);
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                    logWrite("[" + date + "][BOT]: " + user);
                                    CLog.LogWrite("[" + date + "][BOT]: " + user);
                                }
                            }
                            else
                            {
                                await arg.Channel.SendMessageAsync("The command for Secret Key Game rank coins for user must contain !c", false, null);
                                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                                logWrite("[" + date + "][BOT]: The command for Secret Key Game rank coins for user must contain !c");
                                CLog.LogWrite("[" + date + "][BOT]: The command for Secret Key Game rank coins for user must contain !c");
                            }
                        }
                        else
                        {

                            string user = string.Empty;

                            foreach (var line in rUserCoinsLines)
                            {

                                if (line.Contains(m.Author.Username))
                                {
                                    string[] rank = line.Split('|');

                                    user = "****" + rank[0] + "****, you have ****" + rank[2] + "**** coins!";
                                }

                            }


                            if (!rUserCoins.Contains(m.Author.Username))
                            {
                                user = "****" + m.Author.Username + "****, has ****0**** coins!";
                            }
                            await arg.Channel.SendMessageAsync(user, false, null);
                            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                            logWrite("[" + date + "][BOT]: " + user);
                            CLog.LogWrite("[" + date + "][BOT]: " + user);
                        }
                    }
                }
                catch (Exception e)
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    CLog.LogWriteError("[" + date + "]Error - Game rank Command: " + e.ToString());
                }
                //--------------------------------------------
            }
            #endregion
        }

        /// <summary>
        /// discrod.net log system
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Task Log(LogMessage msg)
        {
            //WebSocket connection was closed 
            if (msg.ToString().Contains("Server requested a reconnect"))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "][BOT]INFO: Server requested a reconnect");
                CLog.LogWrite("[" + date + "][BOT]INFO: Server requested a reconnect");

            }else if (msg.ToString().Contains("Disconnected"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/red_dot.png"));
                    startBotBTN.Content = "START";
                });
            }else if (msg.ToString().Contains("Ready"))
            {
                Thread.Sleep(50);
                string[] cUser = _client.CurrentUser.ToString().Split('#');
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Connect as " + cUser[0]);
                CLog.LogWrite("[" + date + "][BOT] Connect as " + cUser[0]);
                this.Dispatcher.Invoke(() =>
                {
                    statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/green_dot.png"));
                    startBotBTN.Content = "STOP";
                });
            }
            else if (msg.ToString().Contains("WebSocket connection was closed"))
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "][BOT]INFO: The remote party closed the WebSocket connection without completing the close handshake.");
                CLog.LogWrite("[" + date + "][BOT]INFO: The remote party closed the WebSocket connection without completing the close handshake.");
            }
            else
            {
                logWrite(msg.ToString());
                CLog.LogWrite(msg.ToString());
            }



            return Task.CompletedTask;
        }
        /// <summary>
        /// Load variables from registry and other
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegVarLoad(object sender, EventArgs e)
        {
            menuStatus = Reg.regKey_Read(keyName, "menuStatus");
            apiKey = Reg.regKey_Read(keyName, "WeatherAPIKey");
            wStart = Reg.regKey_Read(keyName, "wStart");
            sGame = Reg.regKey_Read(keyName, "sGame");
            channelID = Reg.regKey_Read(keyName, "channelID");
            sChannel = Reg.regKey_Read(keyName, "sChannel");
            try
            {

                d_Token = Encryption._decryptData(Reg.regKey_Read(keyName, "d_Token"));
            }
            catch (Exception ex)
            {
                CLog.LogWriteError("Settigns - decrypt Discord Token: " + ex.ToString());
            }


            //check if internet is up and reconnect the bot
            if (Network.inetCK())
            {
                statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/green_dot.png"));
            }
            else
            {
                statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/red_dot.png"));
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Internet is down! Bot will be alive after internet connection is up!");
                CLog.LogWrite("[" + date + "] Internet is down! Bot will be alive after internet connection is up!");

            }
            //------------------------------------------
        }

        /// <summary>
        /// Log writer
        /// </summary>
        /// <param name="data"></param>
        private void logWrite(string data)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(data))
                {
                    //outs += data + Environment.NewLine;
                    logViewRTB.Document.Blocks.Add(new Paragraph(new Run(data)));
                    logViewRTB.ScrollToEnd();
                }
            });

        }


        /// <summary>
        /// Drag window on mouse click left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        /// <summary>
        /// Close wpf form button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeBTN_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Application.Current.Shutdown();//close the app
        }

        /// <summary>
        /// Output string from a rich text box
        /// </summary>
        /// <param name="rtb"></param>
        /// <returns></returns>
        private string ConvertRichTextBoxContentsToString(System.Windows.Controls.RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart,
            rtb.Document.ContentEnd);
            return textRange.Text;
        }

        /// <summary>
        /// Openning settings form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsBTN_Click(object sender, RoutedEventArgs e)
        {
            settings st = new settings();
            st.Show();
        }

        /// <summary>
        /// Bot start/stop button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startBTN_Click(object sender, RoutedEventArgs e)
        {
            botActivate();
        }
        /// <summary>
        /// Bring to live the bot
        /// </summary>
        private void botActivate()
        {

            if (Network.inetCK())
            {
                if (_client == null)
                {

                    worker = new BackgroundWorker();
                    worker.DoWork += BotConnect;
                    worker.RunWorkerAsync();
                    dispatcherTimer.Start();
                }
                else
                {

                    _client.StopAsync();
                    this.Dispatcher.Invoke(() =>
                    {
                        _client = null;
                        statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/green_dot.png"));
                        logViewRTB.Document.Blocks.Clear();
                        startBotBTN.Content = "START";
                        dispatcherTimer.Stop();
                    });

                }
            }
            else
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Internet is down! Try again later");
                CLog.LogWrite("[" + date + "] Internet is down! Try again later");
            }
        }

        /// <summary>
        /// left slide menu open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            btnOpenMenu.Visibility = Visibility.Collapsed;
            btnCloseMenu.Visibility = Visibility.Visible;
            startBotBTN.Visibility = Visibility.Visible;
            logViewRTB.Margin = new Thickness(199, 50, 0, 0);
            Reg.regKey_WriteSubkey(keyName, "menuStatus", "1");

        }

        /// <summary>
        /// left slide menu close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            logViewRTB.Margin = new Thickness(50, 50, 0, 0);
            btnOpenMenu.Visibility = Visibility.Visible;
            btnCloseMenu.Visibility = Visibility.Collapsed;
            startBotBTN.Visibility = Visibility.Hidden;
            Reg.regKey_WriteSubkey(keyName, "menuStatus", "0");

        }


        /// <summary>
        /// Minimizr button(label)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// About window open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutBTN_Click(object sender, RoutedEventArgs e)
        {
            aB = new about();
            aB.ShowDialog();
        }
        

        /// <summary>
        /// opening settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sT = new settings();
            sT.ShowDialog();
        }

        //closing all windows
        private void Window_Closed(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();

        }

        /// start bot from icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statIMG_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            botActivate();
        }

        /// <summary>
        /// weather(celsius) api check and return the output parssed
        /// </summary>
        /// <param name="CityName"></param>
        /// <returns></returns>
        private string weatherForecastMetric(string CityName)
        {

            string _date = DateTime.Now.ToString("yyyy_MM_dd");
            string date2 = string.Empty;
            string errFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log\errors\" + _date + "_log.txt";
            string outs = string.Empty;
            try
            {

                if (apiKey.Length > 0) // we check the lenght
                {

                    string html = @"https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric";

                    HttpResponseMessage response = clientH.GetAsync(string.Format(html, CityName, Encryption._decryptData(apiKey))).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    string l = "";
                    string line = "";
                    //parssing the oudtput
                    responseBody = responseBody.Replace(",", Environment.NewLine);
                    responseBody = responseBody.Replace("\"", "");
                    responseBody = responseBody.Replace("}", "");
                    responseBody = responseBody.Replace("{", "");
                    responseBody = responseBody.Replace("wind:", "");
                    responseBody = responseBody.Replace("main:", "");
                    //---------------------------------
                    using (var sr = new StringReader(responseBody))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {

                            //we check only for what we need, like: temp, feel, humidity, wind speed
                            if (line.Contains("temp") || line.Contains("feel") || line.Contains("humidity") || line.Contains("speed"))
                            {
                                l += line + Environment.NewLine;
                            }
                        }
                    }
                    outs = l;
                    //renaming output parts
                    outs = outs.Replace("temp:", " Temperature: ");
                    outs = outs.Replace("feels_like:", " Feels Like: ");
                    outs = outs.Replace("temp_min:", " Minim Temperature: ");
                    outs = outs.Replace("temp_max:", " Maxim Temperature: ");
                    outs = outs.Replace("humidity:", " Humidity: ");
                    outs = outs.Replace("speed:", " Wind Speed: ");
                    //---------------------------------
                }
                else
                {
                    //we print the issue on the log viewer console
                    logWrite("No openweathermap.org API Key saved! Please check" + Environment.NewLine);
                    CLog.LogWrite("C: No openweathermap.org API Key saved! Please check" + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                //In case of error we output this in console.
                outs = "Please check city name!";

                //save the entire error to file
                date2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                if (File.Exists(errFile))
                {
                    string rErrorFile = File.ReadAllText(errFile);

                    if (!rErrorFile.Contains("[" + date2 + "] Weather error: "))
                    {
                        CLog.LogWriteError("[" + date2 + "] Weather error: " + e.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    File.WriteAllText(errFile, "");
                    string rErrorFile = File.ReadAllText(errFile);

                    if (!rErrorFile.Contains("[" + date2 + "] Weather error: "))
                    {
                        CLog.LogWriteError("[" + date2 + "] Weather error: " + e.ToString() + Environment.NewLine);
                    }
                }
                //--------------------------------

            }

            //print the final weather forecast
            return outs;

        }

        /// <summary>
        /// weather(imperial) api check and return the output parssed
        /// </summary>
        /// <param name="CityName"></param>
        /// <returns></returns>
        private string weatherForecastImperial(string CityName)
        {

            string _date = DateTime.Now.ToString("yyyy_MM_dd");
            string date2 = string.Empty;
            string errFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\log\errors\" + _date + "_log.txt";
            string outs = string.Empty;
            try
            {

                if (apiKey.Length > 0) // we check the lenght
                {

                    string html = @"https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=imperial";


                    HttpResponseMessage response = clientH.GetAsync(string.Format(html, CityName, Encryption._decryptData(apiKey))).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    string l = "";
                    string line = "";
                    //parssing the oudtput
                    responseBody = responseBody.Replace(",", Environment.NewLine);
                    responseBody = responseBody.Replace("\"", "");
                    responseBody = responseBody.Replace("}", "");
                    responseBody = responseBody.Replace("{", "");
                    responseBody = responseBody.Replace("wind:", "");
                    responseBody = responseBody.Replace("main:", "");
                    //---------------------------------
                    using (var sr = new StringReader(responseBody))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {

                            //we check only for what we need, like: temp, feel, humidity, wind speed
                            if (line.Contains("temp") || line.Contains("feel") || line.Contains("humidity") || line.Contains("speed"))
                            {
                                l += line + Environment.NewLine;
                            }
                        }
                    }
                    outs = l;
                    //renaming output parts
                    outs = outs.Replace("temp:", " Temperature: ");
                    outs = outs.Replace("feels_like:", " Feels Like: ");
                    outs = outs.Replace("temp_min:", " Minim Temperature: ");
                    outs = outs.Replace("temp_max:", " Maxim Temperature: ");
                    outs = outs.Replace("humidity:", " Humidity: ");
                    outs = outs.Replace("speed:", " Wind Speed: ");
                    //---------------------------------
                }
                else
                {
                    //we print the issue on the log viewer console
                    logWrite("No openweathermap.org API Key saved! Please check" + Environment.NewLine);
                    CLog.LogWrite("F: No openweathermap.org API Key saved! Please check" + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                //In case of error we output this in console.
                outs = "Please check city name!";

                //save the entire error to file
                date2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                if (File.Exists(errFile))
                {
                    string rErrorFile = File.ReadAllText(errFile);

                    if (!rErrorFile.Contains("[" + date2 + "] Weather error: "))
                    {
                        CLog.LogWriteError("[" + date2 + "] Weather error: " + e.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    File.WriteAllText(errFile, "");
                    string rErrorFile = File.ReadAllText(errFile);

                    if (!rErrorFile.Contains("[" + date2 + "] Weather error: "))
                    {
                        CLog.LogWriteError("[" + date2 + "] Weather error: " + e.ToString() + Environment.NewLine);
                    }
                }
                //--------------------------------

            }

            //print the final weather forecast
            return outs;

        }

        /// <summary>
        /// Open Yanni Points system management form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YanniPoints_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            yM = new yanni_management();
            yM.Show();
        }

        /*
        Future work
        /// <summary>
        /// Message function for send on discord/log and store log
        /// </summary>
        /// <param name="MessageSent">Message to be sent on Discord<./param>
        /// <param name="LogText">Message to be displayed on bot and stored on log files.</param>
        private async void MessageSend(string MessageSent, string LogText)
        {
            try
            {

                SocketMessage arg = null;
                var message = arg as SocketUserMessage;
                await arg.Channel.SendMessageAsync(MessageSent, false, null);
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "][BOT]: " + LogText);
                CLog.LogWrite("[" + date + "][BOT]: " + LogText);
            }
            catch (Exception e)
            {
                CLog.LogWriteError("MessageSend error: " + e.ToString());
            }
        }

        */

        private void wStart_Tick(object sender, EventArgs e)
        {


            if (Network.inetCK())
            {
                if (_client == null)
                {

                    worker = new BackgroundWorker();
                    worker.DoWork += BotConnect;
                    worker.RunWorkerAsync();
                    dispatcherTimer.Start();
                    wStartTimer.Stop();
                }
                else
                {

                    _client.StopAsync();
                    this.Dispatcher.Invoke(() =>
                    {
                        _client = null;
                        statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/green_dot.png"));
                        logViewRTB.Document.Blocks.Clear();
                        startBotBTN.Content = "START";
                        dispatcherTimer.Stop();
                    });

                }
            }
            else
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Internet is down! Try again later");
                CLog.LogWrite("[" + date + "] Internet is down! Try again later");
            }

        }

        /// <summary>
        /// Open Secret Key Game Management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecretKeyGame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sKG = new secretkey_game_manage();
            sKG.ShowDialog();
        }
    }
}
