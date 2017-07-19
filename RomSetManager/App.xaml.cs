using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RomSetManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var e = (Exception) args.ExceptionObject;


                MessageBox.Show("Unhandled Exception: " + e.Message + "; " + e.StackTrace);
            };
        }
    }

}