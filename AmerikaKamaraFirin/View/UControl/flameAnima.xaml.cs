using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AmerikaKamaraFirin.View.UControl
{
    /// <summary>
    /// flame.xaml etkileşim mantığı
    /// </summary>
    public partial class flameAnima : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public flameAnima()
        {
            InitializeComponent();
            StartAnimation();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool run;
        public bool Run
        {
            get { return run; }
            set
            {
                if (run != value)
                {
                    run = value;
                    OnPropertyChanged(nameof(Run));

                    if (run)
                        StartAnimation();
                    else
                        StopAnimation();
                }
            }
        }

        // Animasyonu Başlat
        private void StartAnimation()
        {
            var storyboard = (Storyboard)FindResource("MoveStoryboard");
            storyboard.Begin(this, true); // Tekrar başlatılabilir şekilde başlat
        }

        // Animasyonu Durdur
        private void StopAnimation()
        {
            var storyboard = (Storyboard)FindResource("MoveStoryboard");
            storyboard.Pause(this); // Duraklat
        }
    }
}
