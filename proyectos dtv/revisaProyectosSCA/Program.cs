using System;
using System.Windows.Forms;

namespace revisaProyectosSCA
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                Application.Run(new Form3(args[0]));
            }
            else
            {
                Application.Run(new Form3());
            }
            //test

            //Application.Run(new Form1());
        }
    }
}
