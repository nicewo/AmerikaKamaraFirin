using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// ErrorPage.xaml etkileşim mantığı
    /// </summary>
    public partial class ErrorPage : Window
    {
        public ErrorPage()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string text in Globals.HataIcerigi.Split('/'))
            {
                textBox.AppendText(text);
                textBox.AppendText("\r\n");
            }
        }

        private void textBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var msg = new Message("Hata İçeriği Günlüğe Kaydedildi.");
            await msg.ShowWithTimeout(1000);
            Globals.HataIcerigi = "";
        }
    }
}
