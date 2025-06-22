using Sharp7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Path = System.IO.Path;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Main.xaml etkileşim mantığı
    /// </summary>
    public partial class MainPage : Page
    {
        public static string RecipesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");

        public MainPage()
        {
            InitializeComponent();
            LoadRecipe();
            CreateTrendCanvas();

        }
        public void TimerAction()
        {

            if (Plc.r_firinDurum)
            {
                string statuMachine = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.calisiyor + "(" + AmerikaKamaraFirin.Resources.adim + ":" + Plc.r_step + ")";
                MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                string statuMachine2 = "";
                ElapsedTime.Visibility = Visibility.Visible;

                if (seciliRecete != null)
                {
                    statuMachine2 = "    " + AmerikaKamaraFirin.Resources.hedef_sicaklik + " : " + seciliRecete.Adimlar[Plc.r_step].HedefSicaklik1;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.sure + " : " + seciliRecete.Adimlar[Plc.r_step].SureDakika;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.baca_aciklik + " : " + seciliRecete.Adimlar[Plc.r_step].BacaAciklik;
                }

                MachineStatu.Content = statuMachine;
                MachineStatu2.Content = statuMachine2;

            }
            else
            {
                MachineStatu.Content = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.durduruldu;
                MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                ElapsedTime.Visibility = Visibility.Hidden;
            }

            ElapsedTime.Content = AmerikaKamaraFirin.Resources.gecenZaman + " : " + Plc.r_total_elapsed_time + " " + AmerikaKamaraFirin.Resources.dk;

            if (Plc.r_solKapiAssada && Plc.r_solKapiKapali)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solkapaliassa.png"));
            }
            else if (Plc.r_solKapiAssada && Plc.r_solKapiAcik)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solacikassa.png"));
            }
            else if (Plc.r_solKapiYukarda && Plc.r_solKapiKapali)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solkapaliyukari.png"));
            }
            else if (Plc.r_solKapiYukarda && Plc.r_solKapiAcik)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solacikyukari.png"));
            }

            if (Plc.r_sagKapiAssada && Plc.r_sagKapiKapali)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagkapaliassa.png"));
            }
            else if (Plc.r_sagKapiAssada && Plc.r_sagKapiAcik)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagacikassa.png"));
            }
            else if (Plc.r_sagKapiYukarda && Plc.r_sagKapiKapali)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagkapaliyukari.png"));
            }
            else if (Plc.r_sagKapiYukarda && Plc.r_sagKapiAcik)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagacikyukari.png"));
            }



            if(Plc.r_KapiMinTempError)
            {

            }


            if (!Plc.r_veriGeldi && !Plc.plcyaz)
            {

                Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
                Plc.plcoku = false;

                if (Plc.r_step < 0) Plc.r_step = 0;
                if (Plc.r_step > 100) Plc.r_step = 0;

                Plc.w_setTemp1 = seciliRecete.Adimlar[Plc.r_step].HedefSicaklik1;
                Plc.w_setTemp2 = seciliRecete.Adimlar[Plc.r_step].HedefSicaklik2;
                Plc.w_setTime = seciliRecete.Adimlar[Plc.r_step].SureDakika;
                Plc.w_damper1 = seciliRecete.Adimlar[Plc.r_step].BacaAciklik;

                S7.SetDIntAt(Plc.writeBuffer, 2, Plc.w_setTemp1);
                S7.SetDIntAt(Plc.writeBuffer, 10, Plc.w_setTime);
                S7.SetDIntAt(Plc.writeBuffer, 6, Plc.w_setTemp2);
                S7.SetDIntAt(Plc.writeBuffer, 22, Plc.w_damper1);
                S7.SetBitAt(Plc.writeBuffer, 42, 2,true);
                S7.SetBitAt(Plc.writeBuffer, 42, 3,true);
                S7.SetDIntAt(Plc.writeBuffer, 38, 10);

                Plc.plcyaz = true;

            }


            if (Plc.w_sagdoorclose) sagLeftRight.RightButtonBackground = Brushes.Orange;
            else sagLeftRight.RightButtonBackground = Brushes.Green;
            if (Plc.w_sagdooropen) sagLeftRight.LeftButtonBackground = Brushes.Orange;
            else sagLeftRight.LeftButtonBackground = Brushes.Green;

            if (Plc.w_sagdoorup) sagUpDown.RightButtonBackground = Brushes.Orange;
            else sagUpDown.RightButtonBackground = Brushes.Green;
            if (Plc.w_sagdoordown) sagUpDown.LeftButtonBackground = Brushes.Orange;
            else sagUpDown.LeftButtonBackground = Brushes.Green;




            if (Plc.w_soldoorclose) solLeftRight.RightButtonBackground = Brushes.Orange;
            else solLeftRight.RightButtonBackground = Brushes.Green;
            if (Plc.w_soldooropen) solLeftRight.LeftButtonBackground = Brushes.Orange;
            else solLeftRight.LeftButtonBackground = Brushes.Green;

            if (Plc.w_soldoorup) solUpDown.RightButtonBackground = Brushes.Orange;
            else solUpDown.RightButtonBackground = Brushes.Green;
            if (Plc.w_soldoordown) solUpDown.LeftButtonBackground = Brushes.Orange;
            else solUpDown.LeftButtonBackground = Brushes.Green;



            if(Plc.r_sagKapiKapali) sagLeftRight.RightButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiAcik) sagLeftRight.LeftButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiYukarda) sagUpDown.RightButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiAssada) sagUpDown.LeftButtonBackground = Brushes.Gray;

            if (Plc.r_solKapiKapali) solLeftRight.RightButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiAcik) solLeftRight.LeftButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiYukarda) solUpDown.RightButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiAssada) solUpDown.LeftButtonBackground = Brushes.Gray;
        }



        public void LoadRecipe()
        {
            if (!Directory.Exists(RecipesFolder))
                return;

            var files = Directory.GetFiles(RecipesFolder, "*.json")
                                 .OrderBy(f => f) // alfabetik sıralama
                                 .ToList();

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var recete = JsonSerializer.Deserialize<Recete>(json);

                    if (recete != null)
                        comboBox.Items.Add(recete);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{AmerikaKamaraFirin.Resources.recete_okunurken_hata_olustu}:\n{ex.Message}");
                }
            }
            comboBox.SelectedIndex = 0;
        }
        Recete seciliRecete = null;
        public void CreateTrendCanvas()
        {
            trendGraph.Children.Clear();

            seciliRecete = comboBox.SelectedItem as Recete;
            if (seciliRecete == null || seciliRecete.Adimlar == null || seciliRecete.Adimlar.Count < 1)
                return;

            var polyline = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };

            double maxSicaklik = seciliRecete.Adimlar.Max(a => a.HedefSicaklik1);
            double minSicaklik = seciliRecete.Adimlar.Min(a => a.HedefSicaklik1);
            double range = maxSicaklik - minSicaklik;
            if (range == 0) range = 1;

            double totalSure = seciliRecete.Adimlar.Sum(a => a.SureDakika);

            double xScale = trendGraph.Width / totalSure;
            double yScale = trendGraph.Height / maxSicaklik;

            double now = 0;
            polyline.Points.Add(new Point(0, trendGraph.Height)); // ilk nokta alt köşe

            foreach (var adim in seciliRecete.Adimlar)
            {
                now += adim.SureDakika * xScale;
                double y = trendGraph.Height - (adim.HedefSicaklik1 * yScale);
                polyline.Points.Add(new Point(now, y));
            }

            trendGraph.Children.Add(polyline); // yeni çizim





            var polyline2 = new Polyline
            {
                Stroke = Brushes.LightSkyBlue,
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };

            double maxSicaklik2 = seciliRecete.Adimlar.Max(a => a.HedefSicaklik2);
            double minSicaklik2 = seciliRecete.Adimlar.Min(a => a.HedefSicaklik2);
            double range2 = maxSicaklik2 - minSicaklik2;
            if (range2 == 0) range2 = 1;

            double totalSure2 = seciliRecete.Adimlar.Sum(a => a.SureDakika);

            double xScale2 = trendGraph.Width / totalSure2;
            double yScale2 = trendGraph.Height / maxSicaklik2;

            double now2 = 0;
            polyline2.Points.Add(new Point(0, trendGraph.Height));

            foreach (var adim in seciliRecete.Adimlar)
            {
                now2 += adim.SureDakika * xScale;
                double y = trendGraph.Height - (adim.HedefSicaklik2 * yScale2);
                polyline2.Points.Add(new Point(now2, y));
            }

            trendGraph.Children.Add(polyline2);

        }

        private void solUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 2, true);
            S7.SetBitAt(Plc.writeBuffer, 0, 3, false);

            Plc.plcyaz = true;
        }

        private void solUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 2, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 3, true);
            Plc.plcyaz = true;
        }

        private void solLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 0, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 1, true);

            Plc.plcyaz = true;
        }

        private void solLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;


            S7.SetBitAt(Plc.writeBuffer, 0, 0, true);
            S7.SetBitAt(Plc.writeBuffer, 0, 1, false);

            Plc.plcyaz = true;
        }

        private void sagUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 7, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 6, true);

            Plc.plcyaz = true;
        }

        private void sagUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 6, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 7, true);

            Plc.plcyaz = true;
        }

        private void sagLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 4, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 5, true);

            Plc.plcyaz = true;
        }

        private void sagLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;


            S7.SetBitAt(Plc.writeBuffer, 0, 5, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 4, true);

            Plc.plcyaz = true;
        }

        private void btnReceteSec_Click(object sender, RoutedEventArgs e)
        {
            CreateTrendCanvas();
        }
    }
}
