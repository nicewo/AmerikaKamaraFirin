using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// textBox1.xaml etkileşim mantığı
    /// </summary>
    public partial class textBox1 : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public textBox1()
        {
            InitializeComponent();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public double ValueSize // Sadece Value Boyutunu Ayarlamak için.
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

        private double textSize;
        public double TextSize // Tüm Textlerin Boyutunu Ayarlamak için.
        {
            get { return textSize; }
            set
            {
                if (Math.Abs(textSize - value) > 0.001)
                {
                    textSize = value;
                    OnPropertyChanged(nameof(TextSize));

                    deger.FontSize = textSize;
                }
            }
        }
    }
}
