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

        public ReceteDuzenle(Recete recete)
        {
            InitializeComponent();
           for (int i = 0; i < recete.Adimlar.Count; i++)
                recete.Adimlar[i].AdimNo = i + 1;
            _recete = recete;
            txtReceteAdi.Text = recete.Adi;
            lstAdimlar.ItemsSource = _recete.Adimlar;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _recete.Adi = txtReceteAdi.Text;

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
            MessageBox.Show("Reçete başarıyla kaydedildi.");
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SilAdim_Click(object sender, RoutedEventArgs e)
        {

        }
        private void EkleAdim_Click(object sender, RoutedEventArgs e)
        {

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
