using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;

namespace AmerikaKamaraFirin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // config.json ya da başka bir yerden oku (örnek için varsayılan tr-TR)
            string language = "en-EN";

            if (File.Exists("lang.txt"))
            {
                language = File.ReadAllText("lang.txt");
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        }
    }
}
