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
            if (Globals.plc_firinDurum)
            {
                string statuMachine = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.calisiyor + "(" + AmerikaKamaraFirin.Resources.adim + ":" + Globals.mevcutAdim + ")";
                MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255,0,255,0));
                string statuMachine2 = "";
                ElapsedTime.Visibility = Visibility.Visible;

                if(seciliRecete != null)
                {
                    statuMachine2 = "    " + AmerikaKamaraFirin.Resources.hedef_sicaklik + " : " + seciliRecete.Adimlar[Globals.mevcutAdim].HedefSicaklik1;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.sure + " : " + seciliRecete.Adimlar[Globals.mevcutAdim].SureDakika;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.baca_aciklik + " : " + seciliRecete.Adimlar[Globals.mevcutAdim].BacaAciklik;
                }

                MachineStatu.Content = statuMachine;
                MachineStatu2.Content = statuMachine2;

            }
            else 
            {
                MachineStatu.Content = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.durduruldu;
                MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 255,0 , 0));
                ElapsedTime.Visibility = Visibility.Hidden;
            }

            ElapsedTime.Content = AmerikaKamaraFirin.Resources.gecenZaman + " : " + Globals.gecenZaman + " " + AmerikaKamaraFirin.Resources.dk;

            if (Globals.plc_solKapiAssada && Globals.plc_solKapiKapali)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solkapaliassa.png"));
            }
            else if (Globals.plc_solKapiAssada && Globals.plc_solKapiAcik)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solacikassa.png"));
            }
            else if (Globals.plc_solKapiYukarda && Globals.plc_solKapiKapali)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solkapaliyukari.png"));
            }
            else if (Globals.plc_solKapiYukarda && Globals.plc_solKapiAcik)
            {
                solkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/solacikyukari.png"));
            }

            if (Globals.plc_sagKapiAssada && Globals.plc_sagKapiKapali)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagkapaliassa.png"));
            }
            else if (Globals.plc_sagKapiAssada && Globals.plc_sagKapiAcik)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagacikassa.png"));
            }
            else if (Globals.plc_sagKapiYukarda && Globals.plc_sagKapiKapali)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagkapaliyukari.png"));
            }
            else if (Globals.plc_sagKapiYukarda && Globals.plc_sagKapiAcik)
            {
                sagkapi.Source = new BitmapImage(new Uri("pack://application:,,,/View/images/sagacikyukari.png"));
            }

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
            trendGraph.Children.Clear(); // 🔴 Önce eski çizimi temizle

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

            double now2= 0;
            polyline2.Points.Add(new Point(0, trendGraph.Height)); // ilk nokta alt köşe

            foreach (var adim in seciliRecete.Adimlar)
            {
                now2 += adim.SureDakika * xScale;
                double y = trendGraph.Height - (adim.HedefSicaklik2 * yScale2);
                polyline2.Points.Add(new Point(now2, y));
            }

            trendGraph.Children.Add(polyline2); // yeni çizim

        }

        private void solUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            Globals.plc_solKapiAssada = true;
            Globals.plc_solKapiYukarda = false;
        }

        private void solUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            Globals.plc_solKapiAssada = false;
            Globals.plc_solKapiYukarda = true;
        }

        private void solLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            Globals.plc_solKapiKapali = true;
            Globals.plc_solKapiAcik = false;
        }

        private void solLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            Globals.plc_solKapiKapali = false;
            Globals.plc_solKapiAcik = true;
        }

        private void sagUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            Globals.plc_sagKapiAssada = true;
            Globals.plc_sagKapiYukarda = false;
        }

        private void sagUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            Globals.plc_sagKapiAssada = false;
            Globals.plc_sagKapiYukarda = true;
        }

        private void sagLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            Globals.plc_sagKapiKapali = false;
            Globals.plc_sagKapiAcik = true;
        }

        private void sagLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            Globals.plc_sagKapiKapali = true;
            Globals.plc_sagKapiAcik = false;
        }

        private void btnReceteSec_Click(object sender, RoutedEventArgs e)
        {
            CreateTrendCanvas();
        }
    }
}
