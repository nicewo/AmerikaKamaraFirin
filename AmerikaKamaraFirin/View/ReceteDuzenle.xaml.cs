using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// ReceteDuzenle.xaml etkileşim mantığı
    /// </summary>
    public partial class ReceteDuzenle : Window
    {
        private Recete _recete;
        public Recete eskirecete;
        public ReceteDuzenle(Recete recete)
        {
            InitializeComponent();
           for (int i = 0; i < recete.Adimlar.Count; i++)
                recete.Adimlar[i].AdimNo = i + 1;
            _recete = recete;
            txtReceteAdi.Text = recete.Adi;
            lstAdimlar.ItemsSource = _recete.Adimlar;
            eskirecete = recete;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _recete.Adi = txtReceteAdi.Text;

            if(_recete.Adi != eskirecete.Adi)
            {
                // Reçete adı değiştiğinde eski dosyayı sil
                string eskiDosyaAdi = Globals.RemoveTurkishAndSpecialChars(eskirecete.Adi);
                string eskiKlasor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
                string eskiYol = Path.Combine(eskiKlasor, eskiDosyaAdi + ".json");
                if (File.Exists(eskiYol))
                {
                    File.Delete(eskiYol);
                }
            }

            // Güvenli dosya adı oluştur
            string dosyaAdi = Globals.RemoveTurkishAndSpecialChars(_recete.Adi);
            string klasor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
            Directory.CreateDirectory(klasor);
            string yol = Path.Combine(klasor, dosyaAdi + ".json");

            // JSON serialize ve kaydet
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            File.WriteAllText(yol, JsonSerializer.Serialize(_recete, options));
            MessageBox.Show(AmerikaKamaraFirin.Resources.recete_basariyla_kaydedildi);
            Recipe.StaticWrapPanelInstance.Children.Clear(); // Reçeteleri yeniden yükle
            Recipe.LoadRecipes();
            this.Close();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SilAdim_Click(object sender, RoutedEventArgs e)
        {
            // Hangi butona tıklandığını al
            var button = sender as Button;
            if (button == null) return;

            // Bu butonun ait olduğu ListViewItem'ı bul
            var dataContext = button.DataContext as Adim;
            if (dataContext == null) return;

            // İlgili adımı listeden çıkar
            _recete.Adimlar.Remove(dataContext);

            // Adım numaralarını yeniden düzenle
            for (int i = 0; i < _recete.Adimlar.Count; i++)
                _recete.Adimlar[i].AdimNo = i + 1;

            // ListView'ı güncelle
            lstAdimlar.ItemsSource = null;
            lstAdimlar.ItemsSource = _recete.Adimlar;
        }
        private void EkleAdim_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtYeniSicaklik1.Text, out int sicaklik1) &&
                int.TryParse(txtYeniSicaklik2.Text, out int sicaklik2) &&
                int.TryParse(txtYeniSure.Text, out int sure) && 
                int.TryParse(txtYeniBaca.Text, out int baca))
            {
                // Yeni adım nesnesi oluştur
                var yeniAdim = new Adim
                {
                    AdimNo = _recete.Adimlar.Count + 1,
                    HedefSicaklik1 = sicaklik1,
                    SureDakika = sure,
                    BacaAciklik = baca
                };

                // Listeye ekle
                _recete.Adimlar.Add(yeniAdim);

                // ListView'i güncelle
                lstAdimlar.ItemsSource = null;
                lstAdimlar.ItemsSource = _recete.Adimlar;

                // Giriş kutularını temizle
                txtYeniSicaklik1.Text = "";
                txtYeniSicaklik2.Text = "";
                txtYeniSure.Text = "";
            }
            else
            {
                MessageBox.Show("Geçerli bir sıcaklık ve süre giriniz.");
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

    }
}
