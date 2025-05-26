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
            UpdateStatus(AmerikaKamaraFirin.Resources.plclere_baglaniliyor);
            int tryConnect = 0;
            Config.PlcStatu = 1;
            while (Config.PlcStatu != 0 && tryConnect < Globals.ConnectTryCount)
            {
                Config.PlcStatu = Config.Plc.ConnectTo(Config.PlcIP, 0, 0);
                Task.Delay(500).Wait();
                tryConnect++;
            }
            if (Config.PlcStatu != 0) UpdateStatus($"{AmerikaKamaraFirin.Resources.fırın_PLC_Baglanamadi}: {Globals.PlcError(Config.PlcStatu)}", true, AmerikaKamaraFirin.Resources.program_acilirken_bazi_hatalar_olustu);
            tryConnect = 0;

                UpdateStatus(AmerikaKamaraFirin.Resources.receteler_kontrol_ediliyor);
                Task.Delay(3 * DelayCarpan).Wait();
                CheckAndCreateRecipe();
            // Arayüz Yükleniyor
            UpdateStatus(AmerikaKamaraFirin.Resources.arayuz_yukleniyor);
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
                    UpdateStatus(AmerikaKamaraFirin.Resources.receteler_klasoru_olusturuldu);
                }

                var receteDosyalari = Directory.GetFiles(klasorYolu, "*.json");
                if (receteDosyalari.Length == 0)
                {
                    // Örnek reçete oluştur
                    string receteAdi = AmerikaKamaraFirin.Resources.sample_recipe;
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

                    UpdateStatus(AmerikaKamaraFirin.Resources.ornek_recete_olusturuldu);
                }
                else
                {
                    UpdateStatus(AmerikaKamaraFirin.Resources.receteler_kontrol_edildi);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"{AmerikaKamaraFirin.Resources.recete_klasörü_kontrolünde_hata}: {ex.Message}", true, AmerikaKamaraFirin.Resources.recete_olusturma_hatasi);
            }
        }
        private void UpdateStatus(string message, bool error = false, string title = "Bir Hatayla Karşılaşıldı!")
        {
            title = AmerikaKamaraFirin.Resources.bir_hatayla_karsilasildi;
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
