using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// loginBtn.xaml etkileşim mantığı
    /// </summary>
    public partial class loginBtn : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public loginBtn()
        {
            InitializeComponent();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Color allColor;
        public Color AllColor
        {
            get { return allColor; }
            set
            {
                if (allColor != value)
                {
                    allColor = value;
                    OnPropertyChanged(nameof(AllColor));

                    arrowBrush.Color = allColor;
                    doorBrush.Color = allColor;
                }
            }
        }

        private Color doorColor;
        public Color DoorColor
        {
            get { return doorColor; }
            set
            {
                if (doorColor != value)
                {
                    doorColor = value;
                    OnPropertyChanged(nameof(DoorColor));

                    doorBrush.Color = doorColor;
                }
            }
        }

        private Color arrowColor;
        public Color ArrowColor
        {
            get { return arrowColor; }
            set
            {
                if (arrowColor != value)
                {
                    arrowColor = value;
                    OnPropertyChanged(nameof(ArrowColor));

                    arrowBrush.Color = arrowColor;
                }
            }
        }

        private Brush textColor;
        public Brush TextColor
        {
            get { return textColor; }
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    OnPropertyChanged(nameof(TextColor));

                    title.Foreground = textColor;
                }
            }
        }

        private object text;
        public object Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));

                    title.Content = text;
                    if (text?.ToString() != AmerikaKamaraFirin.Resources.giris_yapiniz)
                    {
                        door.Visibility = Visibility.Hidden;
                        arrow.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        door.Visibility = Visibility.Visible;
                        arrow.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
