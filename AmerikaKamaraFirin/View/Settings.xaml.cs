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

        private void txb_frekans_TextChanged(object sender, TextChangedEventArgs e)
        {
            Plc.plcoku = true;
            int Plc_Writew = Plc.PlcWriteRead();
            if (Plc_Writew != 0)
            {

                MainWindow.UpdateStatus($"PLC Okuma hatası: {Plc_Writew} - {Config.Plc.ErrorText(Plc_Writew)}", true);

            }

            ushort deger = ushort.TryParse(txb_frekans.Text, out var val) ? val : (ushort)0;
            S7.SetWordAt(Plc.writereadBuffer,48, deger);
            S7.SetBitAt(Plc.writereadBuffer, 50, 0, true);
            int Plc_Write = Plc.PlcWrite();
            if (Plc_Write != 0)
            {

                MainWindow.UpdateStatus($"PLC Okuma hatası: {Plc_Write} - {Config.Plc.ErrorText(Plc_Write)}", true);

            }
            Plc.plcoku = false;
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
    }
}
