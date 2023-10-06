
using System;
using System.Windows.Forms;

namespace SurveillanceCSharp
{
    static class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var frm = new FrmSurveillance
            {
                Args = args
            };
            Application.Run(frm);
        }
    }
}
