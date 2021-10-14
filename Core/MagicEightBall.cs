using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
   /// <summary>
   /// Magic 8Ball Game
   /// </summary>
    
    public class MagicEightBall
    {
       static Random r = new Random();
        /// <summary>
        /// Magic Eight Ball Game. 
        /// </summary>
        /// <param name="AnswerFile">Add the path for the file with answer that will be displayed</param>
        /// <returns>The answer from Magic 8 Ball Game</returns>
        public static string GetAnswer(string AnswerFile)
        {
            List<string> randomM = new List<string>();
            string AnswerOutput = string.Empty;
            if (File.Exists(AnswerFile))
            {
                bool c = false;
                string[] rand_list = File.ReadAllLines(AnswerFile);
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
                            return "Answers files is empty! You need to add something";
                            c = true;
                            break;
                        }
                    }
                }
                int index = r.Next(randomM.Count);
                string rand = randomM[index];
                AnswerOutput = rand;
            }
            return AnswerOutput;
        }
    }
}
