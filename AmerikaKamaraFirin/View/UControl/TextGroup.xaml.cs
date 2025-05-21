using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace AmerikaKamaraFirin.View.UControl
{
    public class SubtitleItem
    {
        public string Baslik { get; set; }
        public string Deger { get; set; }
        public string Birim { get; set; }
        public Brush Renk { get; set; }
    }

    public partial class TextGroup : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TextGroup()
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
                    anaBaslik.Text = title;
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
                    anaBaslik.FontSize = titleSize;
                }
            }
        }

        private string degerler;
        public string Value
        {
            get { return degerler; }
            set
            {
                if (degerler != value)
                {
                    degerler = value;
                    OnPropertyChanged(nameof(Value));
                    UpdateSubtitles();
                }
            }
        }

        private string renkler;
        public string Color
        {
            get { return renkler; }
            set
            {
                if (renkler != value)
                {
                    renkler = value;
                    OnPropertyChanged(nameof(Color));
                    UpdateSubtitles();
                }
            }
        }

        private double degerlerSize;
        public double ValueSize
        {
            get { return degerlerSize; }
            set
            {
                if (Math.Abs(degerlerSize - value) > 0.001)
                {
                    degerlerSize = value;
                    OnPropertyChanged(nameof(ValueSize));
                }
            }
        }

        private string birimler;
        public string Unit
        {
            get { return birimler; }
            set
            {
                if (birimler != value)
                {
                    birimler = value;
                    OnPropertyChanged(nameof(Unit));
                    UpdateSubtitles();
                }
            }
        }

        private double birimlerSize;
        public double UnitSize
        {
            get { return birimlerSize; }
            set
            {
                if (Math.Abs(birimlerSize - value) > 0.001)
                {
                    birimlerSize = value;
                    OnPropertyChanged(nameof(UnitSize));
                }
            }
        }

        private string basliklar;
        public string Subtitles
        {
            get { return basliklar; }
            set
            {
                if (basliklar != value)
                {
                    basliklar = value;
                    OnPropertyChanged(nameof(Subtitles));
                    UpdateSubtitles();
                }
            }
        }

        private void UpdateSubtitles()
        {
            var items = new List<SubtitleItem>();

            if (!string.IsNullOrWhiteSpace(basliklar) &&
                !string.IsNullOrWhiteSpace(degerler) &&
                !string.IsNullOrWhiteSpace(birimler))
            {
                var basliklarArray = basliklar.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var degerlerArray = degerler.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var birimlerArray = birimler.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var renklerArray = renkler.Split(';', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < Math.Min(basliklarArray.Length,
                                             Math.Min(degerlerArray.Length, birimlerArray.Length)); i++)
                {
                    Brush brush;
                    if (i < renklerArray.Length && !string.IsNullOrWhiteSpace(renklerArray[i]))
                    {
                        BrushConverter converter = new BrushConverter();
                        brush = (Brush)converter.ConvertFromString(renklerArray[i]);
                    }
                    else
                    {
                        brush = Brushes.White;
                    }

                    items.Add(new SubtitleItem
                    {
                        Baslik = basliklarArray[i],
                        Deger = degerlerArray[i],
                        Birim = birimlerArray[i],
                        Renk = brush
                    });
                }
            }

            AltBasliklar.ItemsSource = items;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSubtitles();
        }
    }
}
