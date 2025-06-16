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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// LeftRightTextbox.xaml etkileşim mantığı
    /// </summary>
    public partial class LeftRightTextbox : UserControl
    {
        public event EventHandler ArrowLeftClicked;
        public event EventHandler ArrowRightClicked;
        public event EventHandler NumPadClicked;
        public event PropertyChangedEventHandler PropertyChanged;

        // Sağ ok için event

        public LeftRightTextbox()
        {
            InitializeComponent();
        }

        // Sol oka tıklanma olayı
        private void ArrowLeft_Click(object sender, MouseButtonEventArgs e)
        {
            ArrowLeftClicked?.Invoke(this, EventArgs.Empty); // Event'i tetikle
        }

        // Sağ oka tıklanma olayı
        private void ArrowRight_Click(object sender, MouseButtonEventArgs e)
        {
            ArrowRightClicked?.Invoke(this, EventArgs.Empty); // Event'i tetikle
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
