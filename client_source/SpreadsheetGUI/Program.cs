using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace SS
{
    /// <summary>
    /// Keeps track of how many windows are running.
    /// </summary>
    class MultiApplicationContext : ApplicationContext
    {
        // number of open windows.
        private int windowCount = 0;

        private static MultiApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        private MultiApplicationContext()
        {
        }

        /// <summary>
        /// Returns the MultiApplicationContext.
        /// </summary>
        public static MultiApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new MultiApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form.
        /// </summary>
        public void RunForm(Form form)
        {
            windowCount++;
            form.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };
            form.Show();
        }

    }


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
            MultiApplicationContext appContext = MultiApplicationContext.getAppContext();
            appContext.RunForm(new Window());
            Application.Run(appContext);
        }
    }
}


