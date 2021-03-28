using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hector
{
    /// <summary>
    /// Interaction logic for yanni_management.xaml
    /// </summary>
    public partial class yanni_management : Window
    {

        //Decalare variables
        readonly static string user_Points = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\data\user_points.txt";
        private static string oldPoints=string.Empty;
        //----------------------
        public yanni_management()
        {
            InitializeComponent();
            //loading user and potins from external file in listview
            string[] rUserPoints = File.ReadAllLines(user_Points);

            foreach (var line in rUserPoints)
            {
               
                if (line.Length > 0 && line!="test|0")
                {
                    string[] s = line.Split('|');
                    pointsList.Items.Add(new { User = s[0], Points = s[1] });
                
                }
            }
            //-----------------------------------------------------
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
        /// Populate textboxes when an item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pointsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pointsList.SelectedItem != null)
            {
                string[] item = pointsList.SelectedItem.ToString().Split(',');
                string u=item[0].Replace("{ User = ","");
                string p = item[1].Replace(" Points = ", "");
                p = p.Replace(" }", "");
                userNameTXT.Text = u;
                pointsTXT.Text = p;
                oldPoints = u + "|" + p;
            }

        }


        /// <summary>
        /// Button for updateing points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateAddBTN_Click(object sender, RoutedEventArgs e)
        {
            string rPointsList = File.ReadAllText(user_Points);
 
            if (userNameTXT.Text.Length > 0 && pointsTXT.Text.Length > 0)
            {
               
                try
                {
                    int n = Convert.ToInt32(pointsTXT.Text);
                    if (n >= 0)
                    {
                        string newPoints = userNameTXT.Text + "|" + pointsTXT.Text;
                        rPointsList = rPointsList.Replace(oldPoints, newPoints);
                        File.WriteAllText(user_Points, rPointsList);
                        oldPoints = newPoints;


                        //reloading user and potins from external file in listview
                        pointsList.Items.Clear();
                        string[] rUserPoints = File.ReadAllLines(user_Points);

                        foreach (var line in rUserPoints)
                        {

                            if (line.Length > 0 && line != "test|0")
                            {
                                string[] s = line.Split('|');
                                pointsList.Items.Add(new { User = s[0], Points = s[1] });

                            }
                        }
                        //-----------------------------------------------------
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("You cannot save negative numbers!");
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Only numbers are allowed in points field!");
                }
            }

            else
            {
                System.Windows.Forms.MessageBox.Show("You must select a user from the list for update!");
            }
        }
    }

}
