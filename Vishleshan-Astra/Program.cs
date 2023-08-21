using System;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace Vishleshan_Astra
{
    class Program
    {
        #region code to maximize console window
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;
        #endregion

        static void Main(string[] args)
        {
            #region code to maximize console window
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
            #endregion

            AnsiConsole.Write(new FigletText($"Initiating adhyayan at {DateTime.Now}").LeftJustified().Color(Color.Green));
            AdhyayanManager.GetActivityToPerform(args);
            AdhyayanManager.PerformTask();
            System.Threading.Thread.Sleep(5000);
            Environment.Exit(0);
        }        
    }
}
