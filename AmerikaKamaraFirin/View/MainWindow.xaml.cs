using AmerikaKamaraFirin.View;
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
        private void ThreadTimerTick(object? state)
        {
            //           Plc.ReadPlc();
        }

        private void UItimerTick(object? sender, EventArgs e)
        {
            ErrorControl();

            var currentPage = Pages.Content as dynamic;
            if (currentPage != null && currentPage?.GetType().GetMethod("TimerAction") != null)
            {
                currentPage?.TimerAction();
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            ThreadTimer = new Timer(ThreadTimerTick, null, 0, 300);

            UItimer = new DispatcherTimer();
            UItimer.Interval = TimeSpan.FromMilliseconds(500);
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

            ilkacilis = false;
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
            else if (btn.Name == "btnManual")
            {
                Pages.Navigate(new Manual());
                ActivePage = "btnManual";
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
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void txbError_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErrorPage errorPage = new ErrorPage();
            errorPage.ShowDialog();
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