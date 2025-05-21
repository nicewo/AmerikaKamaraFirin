using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Loading.xaml etkileşim mantığı
    /// </summary>
    public partial class Loading : Window
    {
#if DEBUG
        bool debug = true;
#else
        bool debug = false;
#endif


        int DelayCarpan = 1;

        public Loading()
        {
            InitializeComponent();
            StartLoading();
            if (!debug)
                this.WindowState = WindowState.Maximized;

        }
        private void StartLoading()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            Config.LoadConfig();
            // PLC Bağlantısı
            UpdateStatus("PLC'lere Bağlanılıyor...");
            int tryConnect = 0;
            Config.PlcStatu = 1;
            while (Config.PlcStatu != 0 && tryConnect < Globals.ConnectTryCount)
            {
                Config.PlcStatu = Config.Plc.ConnectTo(Config.PlcIP, 0, 0);
                Task.Delay(500).Wait();
                tryConnect++;
            }
            if (Config.PlcStatu != 0) UpdateStatus($"Fırın PLC Bağlanamadı: {Globals.PlcError(Config.PlcStatu)}", true, "Program Açılırken Bazı Hatalar Oluştu!");
            tryConnect = 0;

                UpdateStatus("Reçeteler kontrol ediliyor...");
                Task.Delay(3 * DelayCarpan).Wait();
                CheckAndCreateRecipe();
            // Arayüz Yükleniyor
            UpdateStatus("Arayüz Yükleniyor...");
            Task.Delay(1 * DelayCarpan).Wait(); // Arayüz yükleme işlemi burada yapılır

        }
        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            // Ana sayfaya geçiş
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow mainPage = new MainWindow();
                mainPage.Show();
                this.Close();
            });
        }
        private void CheckAndCreateRecipe()
        {
            try
            {
                string klasorYolu = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
                if (!Directory.Exists(klasorYolu))
                {
                    Directory.CreateDirectory(klasorYolu);
                    UpdateStatus("Reçeteler klasörü oluşturuldu.");
                }

                var receteDosyalari = Directory.GetFiles(klasorYolu, "*.json");
                if (receteDosyalari.Length == 0)
                {
                    // Örnek reçete oluştur
                    string receteAdi = "Sample Recipe";
                    var ornekRecete = new Recete
                    {
                        Adi = receteAdi,
                        Adimlar = new List<Adim>()
                    };

                    for (int i = 0; i < 10; i++)
                    {
                        ornekRecete.Adimlar.Add(new Adim
                        {
                            HedefSicaklik = 100 + (i * 20), // 100, 120, ..., 280
                            SureDakika = 5 + i              // 5, 6, ..., 14 dakika
                        });
                    }

                    string dosyaAdi = Globals.RemoveTurkishAndSpecialChars(receteAdi) + ".json";
                    string dosyaYolu = Path.Combine(klasorYolu, dosyaAdi);

                    string json = JsonSerializer.Serialize(ornekRecete, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dosyaYolu, json);

                    UpdateStatus("Örnek reçete oluşturuldu.");
                }
                else
                {
                    UpdateStatus("Reçeteler kontrol edildi.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Reçete klasörü kontrolünde hata: {ex.Message}", true, "Reçete Oluşturma Hatası");
            }
        }
        private void UpdateStatus(string message, bool error = false, string title = "Bir Hatayla Karşılaşıldı!")
        {
            // Uygulama içindeki TextBlock'a mesajı yaz
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtLoaded.Text = message;

                if (error)
                {
                    txtLoaded.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    txtLoaded.Foreground = new SolidColorBrush(Colors.White);
                }
            });
            if (error)
            {
                Globals.UpdateStatus(message, error, title);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
