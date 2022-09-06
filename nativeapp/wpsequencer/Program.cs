using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wpsequencer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // I need to put my mits on this Form1 instance:
            Form1 mainForm = new Form1();
            // Static class CTOR does not run until I reference something in it!
            StdioJsonHandler.FirstReferenceToStartMe(mainForm);
            // Static class CTOR does not run until I reference something in it!
            MySqlHandler.FirstReferenceToStartMe(mainForm);
            // also from me:
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            // With all that done, start infinite loopL
            Application.Run(mainForm);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            // Ignore any errors that might occur
            try
            {
                StdioJsonHandler.FormSaysAppExiting();
                MySqlHandler.FormSaysAppExiting();
            }
            catch
            {
            }
        }

    }
}
