using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NOVisionImageViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var cultureinfo = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = cultureinfo;
            Thread.CurrentThread.CurrentUICulture = cultureinfo;
        }
    }
}
