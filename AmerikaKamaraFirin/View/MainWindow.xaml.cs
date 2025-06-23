using AmerikaKamaraFirin.View;
using Sharp7;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _activePage;
        private DispatcherTimer UItimer;
        private Timer ThreadTimer;

#if DEBUG
        bool debug = true;
#else
        bool debug = false;
#endif

        bool ilkacilis = true;

        private static string lastErrorMessage = string.Empty;  





        public string ActivePage
        {
            get => _activePage;
            set
            {
                _activePage = value;
                OnPropertyChanged(nameof(ActivePage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Binding için
            Loaded += MainWindow_Loaded;
            if (!debug)
                this.WindowState = WindowState.Maximized;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {


            UItimer = new DispatcherTimer();
            UItimer.Interval = TimeSpan.FromMilliseconds(700);
            UItimer.Tick += UItimerTick;
            UItimer.Start();
            ActivePage = "btnMainPage";

            if (File.Exists("lang.txt"))
            {
                string currentLang = File.ReadAllText("lang.txt").Trim();
                foreach (ComboBoxItem item in cmbLanguage.Items)
                {
                    if ((item.Tag as string) == currentLang)
                    {
                        cmbLanguage.SelectedItem = item;
                        break;
                    }
                }
            }

            ThreadTimer = new Timer(ThreadTimerTick, null, 0, 600);
            ilkacilis = false;
            Plc.plcoku = true;

        }


        private void ThreadTimerTick(object? state)
        {
            PlcFunction();
        }
        private void UItimerTick(object? sender, EventArgs e)
        {
            ErrorControl();
            if (ActivePage == "btnAlarms")
            {
                txbError.Visibility = Visibility.Hidden;
            }
            var currentPage = Pages.Content as dynamic;
            if (currentPage != null && currentPage?.GetType().GetMethod("TimerAction") != null)
            {
                currentPage?.TimerAction();
            }

            if (Plc.r_butonTime > 0)
            {
                lbl_geriSayim.Visibility = Visibility.Visible;
                lbl_geriSayim.Content = ((Plc.r_butonBasmaTime - Plc.r_butonTime) / 1000).ToString();
            }
            else
            {
                lbl_geriSayim.Visibility = Visibility.Hidden;
            }


            if(((Plc.r_butonBasmaTime - Plc.r_butonTime) / 1000) < 0 || ((Plc.r_butonBasmaTime - Plc.r_butonTime) / 1000) > 10)
            {
                lbl_geriSayim.Visibility = Visibility.Hidden;
            }
        }


        private void PlcFunction()
        {
            if (!Config.Plc.Connected || !Globals.plcConnected)
                Plc.PlcConnect();
            else
                Plc.PlcCycle();
        }


        public static void UpdateStatus(string message, bool error = false, string title = "Bir Hatayla Karşılaşıldı!")
        {
            title = AmerikaKamaraFirin.Resources.bir_hatayla_karsilasildi;
            if (lastErrorMessage != message)
            {
                if (error)
                {
                    Globals.UpdateStatus(message, error, title);
                }
                lastErrorMessage = message;
            }
        }
        private void ErrorControl()
        {
            if (Globals.HataIcerigi != "")
            {
                txbError.Title = Globals.HataBasligi;
                txbError.Visibility = Visibility.Visible;
            }
            else
            {
                txbError.Visibility = Visibility.Hidden;
            }
        }


        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
        private void btnNavbar_Click(object sender, RoutedEventArgs e)
        {
            // btnLaminasyon btnSunger btnAstar  btnKumas  btnDogSarma  btnTemizlik  btnRecete  btnKalibrasyon
            Button btn = (Button)sender;
            if (btn.Name == "btnMainPage")
            {
                Pages.Navigate(new MainPage());
                ActivePage = "btnMainPage";
            }
            else if (btn.Name == "btnAlarms")
            {
                Pages.Navigate(new Alarms());
                ActivePage = "btnAlarms";
            }
            else if (btn.Name == "btnRecipe")
            {
                Pages.Navigate(new Recipe());
                ActivePage = "btnRecipe";
            }
            else if (btn.Name == "btnSettings")
            {
                Pages.Navigate(new Settings());
                ActivePage = "btnSettings";
            }
            else
            {
                Pages.Navigate(new MainPage());
                ActivePage = "btnMainPage";
            }
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void txbError_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Pages.Navigate(new Alarms());
            ActivePage = "btnAlarms";
        }
        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ilkacilis) return;
            if (cmbLanguage.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string cultureCode)
            {
                // Dili kaydet
                File.WriteAllText("lang.txt", cultureCode);

                // Uygulamayı yeniden başlat
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                System.Diagnostics.Process.Start(exePath);
                Application.Current.Shutdown();
            }
        }
    }
}