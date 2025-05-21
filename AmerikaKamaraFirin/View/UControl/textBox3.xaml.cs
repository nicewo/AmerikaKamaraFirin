using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// textBox3.xaml etkileşim mantığı
    /// </summary>
    public partial class textBox3 : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public textBox3()
        {
            InitializeComponent();
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
    }
}
