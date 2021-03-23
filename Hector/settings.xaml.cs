﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //----------------------------


        public settings()
        {
            InitializeComponent();
            //read registry values
            apiKey = Reg.regKey_Read(keyName, "WeatherAPIKey");
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

            MessageBox.Show("Your settings are saved!");
            this.Close();
        }
    }
}
