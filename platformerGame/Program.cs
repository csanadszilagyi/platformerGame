using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace platformerGame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                cSfmlApp gameApp = new cSfmlApp();
                gameApp.Run();
            }
            catch (Exception e)
            {
                StreamWriter sw = new StreamWriter("error_log.txt");
                sw.WriteLine("Error created at " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")); //yyyy/MM/dd HH:mm:ss:
                sw.WriteLine(e.Message);
                sw.WriteLine(e.Data.ToString());

                sw.WriteLine(e.StackTrace);
                sw.Close();
            }
        }
    }
}
