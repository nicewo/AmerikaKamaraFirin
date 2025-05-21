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

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Message.xaml etkileşim mantığı
    /// </summary>
    public partial class Message : Window
    {
        public Message(string message)
        {
            InitializeComponent();
            MessageText.Text = message; // TextBlock veya Label ile bağlayabilirsiniz

        }

        public async Task ShowWithTimeout(int milliseconds)
        {
            Show();
            await Task.Delay(milliseconds);
            Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
