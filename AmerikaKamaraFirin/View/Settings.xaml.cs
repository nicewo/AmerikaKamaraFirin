using Sharp7;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Settings.xaml etkileşim mantığı
    /// </summary>
    public partial class Settings : Page
    {
        Numpad num;

        public Settings()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_minTemp.Visibility = Visibility.Hidden;
            txb_minTemp.Visibility = Visibility.Hidden;
            lbl_tempFark.Visibility = Visibility.Hidden;
            txb_tempFark.Visibility = Visibility.Hidden;

            txb_frekans.Text = Plc.w_surucuFrekans.ToString();
            txb_minTemp.Text = Plc.w_minTemp.ToString();
            txb_tempFark.Text = Plc.w_tcfarkhata.ToString();

            txb_frekans.TextChanged += txb_frekans_TextChanged;
            txb_minTemp.TextChanged += txb_minTemp_TextChanged;
            txb_tempFark.TextChanged += txb_tempFark_TextChanged;
        }

        public void TimerAction()
        {



        }

        private void txb_frekans_TextChanged(object sender, TextChangedEventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;



            ushort deger = ushort.TryParse(txb_frekans.Text, out var val) ? val : (ushort)0;
            S7.SetWordAt(Plc.writeBuffer, 48, deger);

            MainPage.SetSetting("Frekans", deger);

            Plc.plcyaz = true;
        }
        private void txb_minTemp_TextChanged(object sender, TextChangedEventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;



            ushort deger = ushort.TryParse(txb_minTemp.Text, out var val) ? val : (ushort)0;
            S7.SetDIntAt(Plc.writeBuffer, 52, deger);

            MainPage.SetSetting("MinTemp", deger);

            Plc.plcyaz = true;
        }
        private void txb_tempFark_TextChanged(object sender, TextChangedEventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;



            ushort deger = ushort.TryParse(txb_tempFark.Text, out var val) ? val : (ushort)0;
            S7.SetDIntAt(Plc.writeBuffer, 38, deger);

            MainPage.SetSetting("TempFark", deger);

            Plc.plcyaz = true;
        }

        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox txb)
            {
                int max = 1000;
                if (txb.Name == "txb_frekans") max = 60;
                if (txb.Name == "txb_minTemp") max = 900;
                if (txb.Name == "txb_tempFark") max = 200;
                num = new Numpad(txb.Text,0,max);
                if (num.ShowDialog() == true)
                {
                    txb.Text = num.GirilenMetin;
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
                int max = 9999;
                num = new Numpad("", 0, max);
                if (num.ShowDialog() == true)
                {
                    if(num.GirilenMetin == "1453")
                    {
                        lbl_minTemp.Visibility = Visibility.Visible;
                        txb_minTemp.Visibility = Visibility.Visible;
                        lbl_tempFark.Visibility = Visibility.Visible;
                        txb_tempFark.Visibility= Visibility.Visible;
                    }
                }
            
        }
    }
}
