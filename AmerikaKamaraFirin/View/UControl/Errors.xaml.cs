using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// Errors.xaml etkileşim mantığı
    /// </summary>
    public partial class Errors : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Errors()
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

        private bool error;
        public bool IsError
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    OnPropertyChanged(nameof(IsError));

                    // kontrol widthHeight 60 - 90*250   border borderbrush   FirstGrid   SecondGrid
                    if (error)
                    {
                        border.Width = 250;
                        border.Height = 90;
                        border.BorderBrush = new SolidColorBrush(Colors.Red);
                        FirstGrid.Visibility = Visibility.Visible;
                        //SecondGrid.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        border.Width = 60;
                        border.Height = 60;
                        border.BorderBrush = new SolidColorBrush(Colors.Green);
                        FirstGrid.Visibility = Visibility.Hidden;
                        //SecondGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
