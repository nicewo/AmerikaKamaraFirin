using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// durumBilgisi.xaml etkileşim mantığı
    /// </summary>
    public partial class durumBilgisi : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public durumBilgisi()
        {
            InitializeComponent();
        }

        public void UpdatePLCStatus(bool status)
        {
            if (status)
            {
                statusLabel.Content = AmerikaKamaraFirin.Resources.buffer_acik;
                statusLabel.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                statusLabel.Content = AmerikaKamaraFirin.Resources.buffer_kapali;
                statusLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));

                    icerik.Text = title;
                }
            }
        }
    }
}
