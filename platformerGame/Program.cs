using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using platformerGame.App;

namespace platformerGame
{
    class Program
    {
        static void Main(string[] args)
        {
            SfmlApp gameApp = null;
            
            try
            {
                gameApp = new SfmlApp();
                gameApp.Run();
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter("ErrorLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                sw.WriteLine("Error created at " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")); //yyyy/MM/dd HH:mm:ss:
                sw.WriteLine(ex.Message);
                sw.WriteLine(ex.Data.ToString());

                sw.WriteLine(ex.StackTrace);
                sw.Close();

                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.Data.ToString());
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                while (true) { }

            }
            
        }
    }
}
