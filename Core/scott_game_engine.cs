using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Core
{
     
    public class scott_game_engine
    {
        //Declare variables for game
        //initial map
        private static string[] DEFAULT_MAP = { "a-1", "a-2", "a-3", "a-4", "a-5",
                                 "b-1", "b-2", "b-3", "b-4", "b-5",
                                 "c-1", "c-2", "c-3", "c-4", "c-5",
                                 "d-1", "d-2", "d-3", "d-4", "d-5",
                                 "e-1", "e-2", "e-3", "e-4", "e-5" };

        //initial map 2 for reload 
        private static string[] DEFAULT_MAP2 = { "a-1", "a-2", "a-3", "a-4", "a-5",
                                 "b-1", "b-2", "b-3", "b-4", "b-5",
                                 "c-1", "c-2", "c-3", "c-4", "c-5",
                                 "d-1", "d-2", "d-3", "d-4", "d-5",
                                 "e-1", "e-2", "e-3", "e-4", "e-5" };

        private static string[] NEW_MAP = null;
        private static int pos = 12;

        private static string[] lettersList = { "a", "b", "c", "d", "e" };
        private static string letterPos = string.Empty;
        private static string PlayerPos = string.Empty;
        private static string newLetter = string.Empty;
        private static string numPos = string.Empty;
        private static string PLAYER_DATA = Application.StartupPath + @"\uData.txt";//player username|player position/key position
        private static bool win = false;
        private static Random r = new Random();
        //---------------------------------------------

        /// <summary>
        /// Secret Key spawn random generator for 
        /// </summary>
        /// <param name="arr">Import array for random object generate</param>
        private static void SetSecretKey(string[] arr)
        {
            if (win == false)
            {
                bool c = false;
                List<string> randomM = new List<string>();

                foreach (var line in arr)
                {
                    if (line.Length > 0 && !line.Contains("c-3"))
                    {
                        randomM.Add(line);
                    }

                }
                int index = r.Next(randomM.Count);
                string rand = randomM[index];
                string num = rand.Split('-')[1];

                NEW_MAP = arr.Select(s => s.Replace(rand, "S-" + num)).ToArray();
                win = true;
            }
        }

        /// <summary>
        /// Secret Key Game play
        /// </summary>
        /// <param name="UserName">Disocrd Username</param>
        /// <param name="OutMap">Textbox for output the Data and map</param>
        /// <param name="Command">Coordonates for player movement</param>
        /// <param name="UserData">User data stored afer play</param>
        public static string Game_Play(string UserName, string Command, string UserData)
        {
            string OutMap = string.Empty;
            try
            {
                SetSecretKey(DEFAULT_MAP);

                DEFAULT_MAP = NEW_MAP;
                string[] UserDataLines = File.ReadAllLines(UserData);
                string data = string.Empty;
                int points = 0;
                List<string> uD = new List<string>();
                foreach (var line in UserDataLines)
                {
                    uD.Add(line);
                    if (line.Length > 0 && line.StartsWith(UserName))
                    {

                        string iPos = line.Split('|')[1];
                        if (line.Split('|').Count() == 3)
                        {
                            points = Convert.ToInt32(line.Split('|')[2]);
                        }

                        pos = Convert.ToInt32(iPos);
                        data = line;
                    }
                    else
                    {
                        pos = 12;
                    }
                }

                letterPos = DEFAULT_MAP[pos].Split('-')[0];

                //Move y++
                if (Command == "east")
                {
                    pos++;
                    if (pos < 0)
                    {
                        pos = 0;
                    }
                    if (pos > 24)
                    {
                        pos = 24;
                    }
                    if (pos <= 24 && DEFAULT_MAP[pos].Contains(letterPos) || DEFAULT_MAP[pos].Contains("S"))
                    {

                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(DEFAULT_MAP[pos]));
                        PlayerPos = DEFAULT_MAP[index];
                        if (PlayerPos.Contains("S"))
                        {
                            pos = 12;
                            OutMap = "Win";
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }

                    }
                    else
                    {
                        pos--;
                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(DEFAULT_MAP[pos]));
                        PlayerPos = DEFAULT_MAP[index];
                        if (PlayerPos.Contains("S"))
                        {
                            pos = 12;
                            OutMap = "Win";
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }
                    }

                }
                //move y--
                if (Command == "west")
                {
                    pos--;
                    if (pos < 0)
                    {
                        pos = 0;
                    }
                    if (pos > 24)
                    {
                        pos = 24;
                    }


                    if (pos >= 0 && DEFAULT_MAP[pos].Contains(letterPos) || DEFAULT_MAP[pos].Contains("S"))
                    {

                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(DEFAULT_MAP[pos]));
                        PlayerPos = DEFAULT_MAP[index];
                        if (PlayerPos.Contains("S"))
                        {
                            pos = 12;
                            OutMap = "Win";
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }
                    }
                    else
                    {
                        pos++;
                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(DEFAULT_MAP[pos]));
                        PlayerPos = DEFAULT_MAP[index];
                        if (PlayerPos.Contains("S"))
                        {
                            pos = 12;
                            OutMap = "Win";
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }
                    }
                }

                //move x++
                if (Command == "north")
                {
                    if (pos < 0)
                    {
                        pos = 0;
                    }
                    if (pos > 24)
                    {
                        pos = 24;
                    }
                    letterPos = DEFAULT_MAP[pos].Split('-')[0];

                    numPos = DEFAULT_MAP[pos].Split('-')[1];
                    int nIndex = Array.FindIndex(lettersList, row => row.Contains(letterPos));
                    int lPos = 0;
                    if (nIndex > 0)
                    {
                       lPos = nIndex - 1;
                    }
                    if (lPos >= 0)
                    {

                        newLetter = lettersList[lPos];
                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(newLetter + "-" + numPos));
                        pos = index;
                        if (pos != -1)
                        {
                            PlayerPos = DEFAULT_MAP[index];
                        }
                        else
                        {
                            pos = 12;
                            OutMap = "Win" + Environment.NewLine;
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }

                    }


                }


                //move x--
                if (Command == "south")
                {
                    if (pos < 0)
                    {
                        pos = 0;
                    }
                    if (pos > 24)
                    {
                        pos = 24;
                    }


                    letterPos = DEFAULT_MAP[pos].Split('-')[0];

                    numPos = DEFAULT_MAP[pos].Split('-')[1];
                    int nIndex = Array.FindIndex(lettersList, row => row.Contains(letterPos));
                    int lPos = 0;
                    if (nIndex == 4)
                    {
                        lPos = 4;
                    }
                    else
                    {
                        lPos = nIndex + 1;
                    }
                  
                    if (lPos <= 4)
                    {

                        newLetter = lettersList[lPos];
                        int index = Array.FindIndex(DEFAULT_MAP, row => row.Contains(newLetter + "-" + numPos));
                        pos = index;
                        if (pos != -1)
                        {
                            PlayerPos = DEFAULT_MAP[index];
                        }
                        else
                        {
                            pos = 12;
                            OutMap = "Win" + Environment.NewLine;
                            win = false;
                            DEFAULT_MAP = DEFAULT_MAP2;
                            points++;
                        }

                    }

                }

                if (!OutMap.Contains("Win"))
                {
                    OutMap = "";
                    OutMap += "Game Map" + Environment.NewLine;
                    OutMap += "-------------------------" + Environment.NewLine;
                    OutMap += DEFAULT_MAP2[0] + "    " + DEFAULT_MAP2[1] + "    " + DEFAULT_MAP2[2] + "    " + DEFAULT_MAP2[3] + "    " + DEFAULT_MAP2[4] + Environment.NewLine + "    " + Environment.NewLine;
                    OutMap += DEFAULT_MAP2[5] + "    " + DEFAULT_MAP2[6] + "    " + DEFAULT_MAP2[7] + "    " + DEFAULT_MAP2[8] + "    " + DEFAULT_MAP2[9] + Environment.NewLine + "    " + Environment.NewLine;
                    OutMap += DEFAULT_MAP2[10] + "    " + DEFAULT_MAP2[11] + "    " + DEFAULT_MAP2[12] + "    " + DEFAULT_MAP2[13] + "    " + DEFAULT_MAP2[14] + Environment.NewLine + "    " + Environment.NewLine;
                    OutMap += DEFAULT_MAP2[15] + "    " + DEFAULT_MAP2[16] + "    " + DEFAULT_MAP2[17] + "    " + DEFAULT_MAP2[18] + "    " + DEFAULT_MAP2[19] + Environment.NewLine + "    " + Environment.NewLine;
                    OutMap += DEFAULT_MAP2[20] + "    " + DEFAULT_MAP2[21] + "    " + DEFAULT_MAP2[22] + "    " + DEFAULT_MAP2[23] + "    " + DEFAULT_MAP2[24] + Environment.NewLine + "    " + Environment.NewLine;
                    OutMap += "-------------------------" + Environment.NewLine;
                    OutMap = OutMap.Replace(PlayerPos, " ***X*** ");
                    OutMap += "Player Name: ***" + UserName +"***"+ Environment.NewLine;
                    OutMap += "-------------------------" + Environment.NewLine;
                    foreach (var item in DEFAULT_MAP2)
                    {
                        if (OutMap.Contains(item))
                        {
                            OutMap = OutMap.Replace(item, " O ");
                        }
                    }

                    uD.Remove(data);
                    uD.Add(UserName + "|" + pos.ToString() + "|" + points);
                    File.WriteAllText(UserData, string.Join(Environment.NewLine, uD));
                    pos = 12;

                }
                else
                {

                    uD.Clear();
                    foreach (var item in UserDataLines)
                    {
                        if (item.StartsWith(UserName))
                        {
                            string[] user = item.Split('|');
                            uD.Add(user[0] + "|12|" + points);
                        }else
                        {
                            string[] user = item.Split('|');
                            uD.Add(user[0] + "|12|" + user[2]);
                        }
                    }
                    File.WriteAllText(UserData, string.Join(Environment.NewLine, uD));
                    pos = 12;
                }


                return OutMap;

            }catch(Exception ex)
            {
                throw new Exception( ex.ToString());
            }
        }
    }
}
