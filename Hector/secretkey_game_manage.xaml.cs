using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Core;
namespace Hector
{
    /// <summary>
    /// Interaction logic for secretkey_game_manage.xaml
    /// </summary>
    public partial class secretkey_game_manage : Window
    {
        //Declare variables
        readonly static string user_data = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\uData.txt";
        private static string sGame;
        private static string channelID;
        private static string keyName = "Hector";
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static string sChannel;
        //-------------------------

        public secretkey_game_manage()
        {
            InitializeComponent();
            //read data from registry
            sGame = Reg.regKey_Read(keyName, "sGame");
            channelID = Reg.regKey_Read(keyName, "channelID");
            sChannel = Reg.regKey_Read(keyName, "sChannel");
            //---------------------------------------------------

            //activation game control read from registry
            if (sGame == "1")
            {
                activateGameCKB.Content = "Activate Game: ON";
                activateGameCKB.IsChecked = true;
            }
            else
            {
                activateGameCKB.Content = "Activate Game: OFF";
                activateGameCKB.IsChecked = false;
            }
            //--------------------------------------------------

            //activation of custom channel send for game
            if (sChannel == "1")
            {
                channelCKB.IsChecked = true;
                channelCKB.Content = "Use specific channel for game only: ON";

            }
            else
            {
                channelCKB.IsChecked = false;
                channelCKB.Content = "Use specific channel for game only: OFF";
                discordChannelTXT.IsEnabled = false;
                updateAddBTN.IsEnabled = false;
            }
            //--------------------------------------------------

            //loading user data from external file in listview
            string[] rUserPoints = File.ReadAllLines(user_data);

            foreach (var line in rUserPoints)
            {

                if (line.Length > 0 && line != "test|12|0")
                {
                    string[] s = line.Split('|');
                    coinsList.Items.Add(new { User = s[0], Position = s[1], Coins=s[2] });

                }
            }
            //-----------------------------------------------------

            //load channel ID in textbox
            discordChannelTXT.Text = channelID;
            //-----------------------------------------------------
        }

        /// <summary>
        /// check for regex match
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        /// <summary>
        /// Drag window with mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Close lable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Minimize lable button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniMizeLBL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Store in registry the channel ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateAddBTN_Click(object sender, RoutedEventArgs e)
        {
            if (discordChannelTXT.Text != "")
            {
                Reg.regKey_WriteSubkey(keyName, "channelID", discordChannelTXT.Text);
                MessageBox.Show("Channel ID is saved!");
            }
            else
            {
                MessageBox.Show("You must enter your channel ID!");
            }
        }

        /// <summary>
        /// Reset user Coins 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetPointsBTN_Click(object sender, RoutedEventArgs e)
        {
            List<string> uData = new List<string>();
            string[] u_dataList = File.ReadAllLines(user_data);
            foreach(var line in u_dataList)
            {
                string[] user = line.Split('|');
                uData.Add(user[0] + "|" + user[1] + "|0");
            }
            File.WriteAllText(user_data, string.Join(Environment.NewLine, uData));


            //loading user data from external file in listview
            string[] rUserPoints = File.ReadAllLines(user_data);
            coinsList.Items.Clear();
            foreach (var line in rUserPoints)
            {

                if (line.Length > 0 && line != "test|12|0")
                {
                    string[] s = line.Split('|');
                    coinsList.Items.Add(new { User = s[0], Position = s[1], Coins = s[2] });

                }
            }
            //-----------------------------------------------------
            MessageBox.Show("All players coins are reseted!");
        }
        /// <summary>
        /// Reset user position to middle of map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetPositionBTN_Click(object sender, RoutedEventArgs e)
        {

            List<string> uData = new List<string>();
            string[] u_dataList = File.ReadAllLines(user_data);
            foreach (var line in u_dataList)
            {
                string[] user = line.Split('|');
                uData.Add(user[0] + "|12|" + user[2]);
            }
            File.WriteAllText(user_data, string.Join(Environment.NewLine, uData));


            //loading user data from external file in listview
            string[] rUserPoints = File.ReadAllLines(user_data);
            coinsList.Items.Clear();
            foreach (var line in rUserPoints)
            {

                if (line.Length > 0 && line != "test|12|0")
                {
                    string[] s = line.Split('|');
                    coinsList.Items.Add(new { User = s[0], Position = s[1], Coins = s[2] });

                }
            }
            //-----------------------------------------------------

            MessageBox.Show("All players position are reseted!");
        }

        /// <summary>
        /// Store variable in registry for activate game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void activeteGameCKB_Checked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "sGame", "1");
            activateGameCKB.Content="Activate Game: ON";
        }

        /// <summary>
        /// Store variable in registry for deactivate game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void activeteGameCKB_Unchecked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "sGame", "0");
            activateGameCKB.Content = "Activate Game: OFF";
        }

        /// <summary>
        /// Store variable in regsitry for deactivate custom channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customChannelCKB_Unchecked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "sChannel", "0");
            channelCKB.Content = "Use specific channel for game only: OFF";
            discordChannelTXT.IsEnabled = false;
            updateAddBTN.IsEnabled = false;
        }

        /// <summary>
        /// Store variable in regsitry for activate custom channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customChannelCKB_Checked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "sChannel", "1");
            channelCKB.Content = "Use specific channel for game only: ON";
            discordChannelTXT.IsEnabled = true;
            updateAddBTN.IsEnabled = true;
        }

        /// <summary>
        /// Only digit add in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void discordChannelTXT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Prevent pasting letterts 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
