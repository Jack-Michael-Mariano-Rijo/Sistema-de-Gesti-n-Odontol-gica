using System;
using System.Windows.Forms;

namespace Proyecto_final
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new User());
        }
    }
}