using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private CommandHandler _handler;
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

        //Declare mutex variable for startup instance check
        Mutex myMutex;
        //---------------------------------

        //declare date variable
        private static string date;
        //---------------------------------
        //declare the bot forms variables
        settings sT;
        about aB;
        //--------------------------------
        //declare timer for load icons and variables read value
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //--------------------------------

        private static string menuStatus = "0";
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

            //we check if regisrty variables exists and if not we create
            if (Reg.regKey_Read(keyName, "menuStatus") == "")
            {
                Reg.regKey_CreateKey(keyName, "menuStatus", "0");
            }

            if (Reg.regKey_Read(keyName, "d_Token") == "")
            {
                Reg.regKey_CreateKey(keyName, "d_Token", "0");
            }
            //--------------------------------

            //read variables values from registry
            menuStatus = Reg.regKey_Read(keyName, "menuStatus");
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

            //check internet connection
            if (!Network.inetCK())
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                logWrite("[" + date + "] Internet is down! Try again later");
                CLog.LogWrite("[" + date + "] Internet is down! Try again later");
            }
            //----------------------------------
        }


        private void BotConnect(object sender, DoWorkEventArgs e)
        {
            this.MainAsync().GetAwaiter().GetResult();

        }
        public async Task MainAsync()
        {

            try
            {
                _client = new DiscordSocketClient();

                _client.Log += Log;

                await _client.LoginAsync(TokenType.Bot, d_Token);
                _client.UserJoined += client_UserJoined;
                _client.MessageReceived += client_MessageReceived;

                await _client.StartAsync();

                //load connection icon
                this.Dispatcher.Invoke(() =>
                {
                    statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/green_dot.png"));
                    startBotBTN.Content = "STOP";
                });


                //---------------------------------

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
                CLog.LogWrite("[" + date + "] User Joined Error: "+e.ToString());
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

            try
            {

                if (m.Content != null && m.Content.StartsWith("!hector"))
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

        }

        private Task Log(LogMessage msg)
        {
            logWrite(msg.ToString());
            CLog.LogWrite(msg.ToString());
            if (msg.ToString().Contains("Disconnected"))
            {
                this.Dispatcher.Invoke(() =>
                {
                    statIMG.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/red_dot.png"));
                    startBotBTN.Content = "START";
                });
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
                worker = new BackgroundWorker();
                worker.DoWork += BotConnect;
                worker.RunWorkerAsync();
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
                    dispatcherTimer.Start();
                    worker = new BackgroundWorker();
                    worker.DoWork += BotConnect;
                    worker.RunWorkerAsync();
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

    }
}
