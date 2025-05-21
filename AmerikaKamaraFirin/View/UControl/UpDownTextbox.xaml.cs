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
    /// UpDownTextbox.xaml etkileşim mantığı
    /// </summary>
    public partial class UpDownTextbox : UserControl
    {
        public event EventHandler ArrowLeftClicked;
        public event EventHandler ArrowRightClicked;
        public event EventHandler NumPadClicked;
        public event PropertyChangedEventHandler PropertyChanged;

        // Sağ ok için event

        public UpDownTextbox()
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
        private void Numpad_Click(object sender, MouseButtonEventArgs e)
        {
            NumPadClicked?.Invoke(this, EventArgs.Empty); // Event'i tetikle
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

                    baslik.Text = title;
                }
            }
        }

        private double titleSize;
        public double TitleSize
        {
            get { return titleSize; }
            set
            {
                if (Math.Abs(titleSize - value) > 0.001)
                {
                    titleSize = value;
                    OnPropertyChanged(nameof(TitleSize));

                    baslik.FontSize = titleSize;
                }
            }
        }

        private string value;
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));

                    deger.Text = this.value;
                }
            }
        }

        private double valueSize;
        public double ValueSize
        {
            get { return valueSize; }
            set
            {
                if (Math.Abs(valueSize - value) > 0.001)
                {
                    valueSize = value;
                    OnPropertyChanged(nameof(ValueSize));

                    deger.FontSize = valueSize;
                }
            }
        }

        private string unit;
        public string Unit
        {
            get { return unit; }
            set
            {
                if (unit != value)
                {
                    unit = value;
                    OnPropertyChanged(nameof(Unit));

                    birim.Text = unit;
                }
            }
        }

        private double unitSize;
        public double UnitSize
        {
            get { return unitSize; }
            set
            {
                if (Math.Abs(unitSize - value) > 0.001)
                {
                    unitSize = value;
                    OnPropertyChanged(nameof(UnitSize));

                    birim.FontSize = unitSize;
                }
            }
        }

        private double textSize;
        public double TextSize
        {
            get { return textSize; }
            set
            {
                if (Math.Abs(textSize - value) > 0.001)
                {
                    textSize = value;
                    OnPropertyChanged(nameof(TextSize));

                    baslik.FontSize = textSize;
                    deger.FontSize = textSize;
                    birim.FontSize = textSize;
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
