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
            txb_frekans.Text = S7.GetWordAt(Plc.writereadBuffer, 48).ToString();

            txb_frekans.TextChanged += txb_frekans_TextChanged;
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
            S7.SetBitAt(Plc.writeBuffer, 50, 0, true);


            Plc.plcyaz = true;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txb)
            {
                num = new Numpad(txb.Text);
                if (num.ShowDialog() == true)
                {
                    txb.Text = num.GirilenMetin;
                }
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox txb)
            {
                num = new Numpad(txb.Text);
                if (num.ShowDialog() == true)
                {
                    txb.Text = num.GirilenMetin;
                }
            }
        }


    }
}
