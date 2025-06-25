using Sharp7;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Alarms.xaml etkileşim mantığı
    /// </summary>
    public partial class Alarms : Page
    {
        public Alarms()
        {
            InitializeComponent();
        }
        public void TimerAction()
        {
            if(Globals.HataIcerigi == "")
            {
                lstLog0.Items.Clear();
            } else
            {
                lstLog0.Items.Clear();
                foreach (string text in Globals.HataIcerigi.Split('/'))
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        lstLog0.Items.Add(text.Trim());
                    }
                }
            }
        }
        private void LoadGeçmişAlarmlar()
        {
            lstLog.Items.Clear();
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "status_log.txt");

            if (!File.Exists(logPath))
            {
                lstLog.Items.Add(AmerikaKamaraFirin.Resources.logyok);
                return;
            }

            var lines = File.ReadAllLines(logPath);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    lstLog.Items.Add(line.Trim());
                }
            }
            lstLog.SelectedItem = lstLog.Items.Count - 1;

        }

        private async void ResetAlarm_Click(object sender, RoutedEventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 42, 4, true);

            Plc.plcyaz = true;


            var msg = new Message(AmerikaKamaraFirin.Resources.hata_icerigi_gunluge_kaydedildi);
            await msg.ShowWithTimeout(1000);
            Globals.HataIcerigi = "";
        }

        private void alarmTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (alarmTabs.SelectedItem == GecmisAlarmTab)
            {
                LoadGeçmişAlarmlar();
            }
        }

    }
}
