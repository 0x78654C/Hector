using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Window
    {

        //Declare global variables
        private static string keyName = "Hector";
        private static string d_Token;
        private static string apiKey;
        private static string wStart;
        private const string keyStart = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static string cmdPrefix;
        private static readonly Regex _regex = new Regex("[^!-*.-]+");
        //----------------------------


        public settings()
        {
            InitializeComponent();
            //read registry values
            wStart = Reg.regKey_Read(keyName, "wStart");
            apiKey = Reg.regKey_Read(keyName, "WeatherAPIKey");
            cmdPrefix = Reg.regKey_Read(keyName, "cmdPrefix");
            cmdPrefixXT.Text = cmdPrefix;
            weatherTXT.Password = Encryption._decryptData(apiKey);
            try
            {

                d_Token = Encryption._decryptData(Reg.regKey_Read(keyName, "d_Token"));
                d_TokenTXT.Password = d_Token;
            }
            catch (Exception e)
            {
                CLog.LogWriteError("Settigns - decrypt oAuth Key: " + e.ToString());
            }
            //------------------------------

            //set reboot checkbox based on registry value
            if (wStart == "1")
            {
                startWinCKB.Content = "Open on Windows Startup: ON";
                startWinCKB.IsChecked = true;
            }
            else
            {
                startWinCKB.Content = "Open on Windows Startup: OFF";
                startWinCKB.IsChecked = false;
            }
            //------------------------------

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
        /// Save settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            //store Discord bot Token in regsitry
            if (d_TokenTXT.Password != "")
            {
                Reg.regKey_WriteSubkey(keyName, "d_Token", Encryption._encryptData(d_TokenTXT.Password));

            }
            else
            {
                MessageBox.Show("Please add the Discord Token!");
            }
            //--------------------------------

            //sotre weather API key in regkey
            if (weatherTXT.Password != "")
            {
                Reg.regKey_WriteSubkey(keyName, "WeatherAPIKey", Encryption._encryptData(weatherTXT.Password));
            }
            else
            {
                Reg.regKey_WriteSubkey(keyName, "WeatherAPIKey", weatherTXT.Password);
            }
            //--------------------------------

            //store command prefix in registry
            if (cmdPrefixXT.Text != "")
            {
                Reg.regKey_WriteSubkey(keyName, "cmdPrefix", cmdPrefixXT.Text);
            }
            else
            {
                Reg.regKey_WriteSubkey(keyName, "cmdPrefix","!");
            }
            //--------------------------------

            MessageBox.Show("Your settings are saved!");
            this.Close();
        }

        /// <summary>
        /// Write 1 in registry for enable open on reboot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startWinCKB_Checked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "wStart", "1");
            Reg.regKey_WriteSubkey(keyStart, "Hector", System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            startWinCKB.Content = "Open on Windows Startup: ON";
        }


        /// <summary>
        /// Write 0 in registry for dislabe open on reboot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startWinCKB_Unchecked(object sender, RoutedEventArgs e)
        {
            Reg.regKey_WriteSubkey(keyName, "wStart", "0");
            Reg.regKey_DeleteSubkey(keyStart, "Hector");
            startWinCKB.Content = "Open on Windows Startup: OFF";
       
        }
        /// <summary>
        /// Acceptin only custom characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdPrefixTXT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
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
