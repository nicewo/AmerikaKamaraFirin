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
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Main.xaml etkileşim mantığı
    /// </summary>
    public partial class MainPage : Page
    {
        public static string RecipesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
        static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml");
        private Polyline polylineLiveTemp1 = new Polyline();
        private Polyline polylineLiveTemp2 = new Polyline();
        Numpad num;



        public MainPage()
        {
            InitializeComponent();

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txb_damper1.Text = GetSetting("Damper1").ToString();
            txb_damper2.Text = GetSetting("Damper2").ToString();

            LoadRecipe();
            CreateTrendCanvas();

            LoadLiveDataFromJson(); // geri yükleme
            DrawOldDataOnGraph();   // geçmiş grafiği çiz
        }


        public static int GetSetting(string name)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var docw = new XDocument(
                        new XElement("Settings",
                            new XElement("Damper1", 100),
                            new XElement("Damper2", 100),
                            new XElement("MinTemp", 500),
                            new XElement("Frekans", 50),
                            new XElement("TempFark", 15)
                        )
                    );
                    docw.Save(filePath);
                }

                var doc = XDocument.Load(filePath);

                // İstenen eleman eksikse varsayılanla ekle
                if (doc.Root.Element(name) == null)
                {
                    int defaultValue = name switch
                    {
                        "Damper1" => 100,
                        "Damper2" => 100,
                        "MinTemp" => 500,
                        "Frekans" => 50,
                        "TempFark" => 15,
                        _ => 0
                    };
                    doc.Root.Add(new XElement(name, defaultValue));
                    doc.Save(filePath);
                }

                var element = doc.Root.Element(name);
                return element != null && int.TryParse(element.Value, out int result) ? result : 0;
            }
            catch
            {
                return 0;
            }
        }
        public static void SetSetting(string name, int value)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var docw = new XDocument(
                        new XElement("Settings",
                            new XElement("Damper1", 100),
                            new XElement("Damper2", 100),
                            new XElement("MinTemp", 500),
                            new XElement("Frekans", 50),
                            new XElement("TempFark", 15)
                        )
                    );
                    docw.Save(filePath);
                }

                var doc = XDocument.Load(filePath);

                // Gerekli tüm default ayarların olup olmadığını kontrol et, eksikleri ekle
                var defaultSettings = new Dictionary<string, int>
        {
            { "Damper1", 100 },
            { "Damper2", 100 },
            { "MinTemp", 500 },
            { "Frekans", 50 },
            { "TempFark", 15 }
        };

                foreach (var kvp in defaultSettings)
                {
                    if (doc.Root.Element(kvp.Key) == null)
                        doc.Root.Add(new XElement(kvp.Key, kvp.Value));
                }

                // Çağrılan parametreyi ayarla (varsa güncelle, yoksa ekle)
                var element = doc.Root.Element(name);
                if (element == null)
                    doc.Root.Add(new XElement(name, value.ToString()));
                else
                    element.Value = value.ToString();

                doc.Save(filePath);
            }
            catch
            {
                // Hataları sessizce yut, istenirse loglanabilir
            }
        }



        public async void TimerAction()
        {
            if (Plc.plcokundu)
            {

                int frek = GetSetting("Frekans");
                int mtemp = GetSetting("MinTemp");
                int tfark = GetSetting("TempFark");
                bool degisti = false;
                if (Plc.plcyazokundu)
                {
                    if (Plc.w_surucuFrekans != frek) degisti = true;
                    if (Plc.w_tcfarkhata != tfark) degisti = true;
                    if (Plc.w_minTemp != mtemp) degisti = true;
                    if (degisti)
                    {
                        Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
                        Plc.plcoku = false;
                        S7.SetDIntAt(Plc.writeBuffer, 38, tfark);
                        S7.SetWordAt(Plc.writeBuffer, 48, (ushort)frek);
                        S7.SetDIntAt(Plc.writeBuffer, 52, mtemp);
                        Plc.plcyaz = true;
                    }
                }


                if (Plc.r_receteTamam && Plc.plcyazokundu)
                {

                    var msg = new Message(AmerikaKamaraFirin.Resources.receteTamam);

                    Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
                    Plc.plcoku = false;


                    S7.SetBitAt(Plc.writeBuffer, 42, 5, false);

                    Plc.plcyaz = true;
                    msg.Show();
                }
            }



            if (Plc.r_firinDurum)
            {
                string statuMachine = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.calisiyor + "(" + AmerikaKamaraFirin.Resources.adim + ":" + Plc.r_step + ")";
                MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                string statuMachine2 = "";
                ElapsedTime.Visibility = Visibility.Visible;
                ElapsedTimeStep.Visibility = Visibility.Visible;

                if (Globals.seciliRecete != null)
                {
                    statuMachine2 = "    " + AmerikaKamaraFirin.Resources.hedef_sicaklik + " : " + Globals.seciliRecete.Adimlar[Plc.r_step].HedefSicaklik1 + " - " + Globals.seciliRecete.Adimlar[Plc.r_step].HedefSicaklik2;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.sure + " : " + Globals.seciliRecete.Adimlar[Plc.r_step].SureDakika;
                    statuMachine2 = statuMachine2 + "    " + AmerikaKamaraFirin.Resources.baca_aciklik + " : " + Globals.seciliRecete.Adimlar[Plc.r_step].BacaAciklik1 + " - " + Globals.seciliRecete.Adimlar[Plc.r_step].BacaAciklik2;
                }

                MachineStatu.Content = statuMachine;
                MachineStatu2.Content = statuMachine2;


                txb_damper1.IsEnabled = false; txb_damper2.IsEnabled = false;


                lblG1Akim.Content = Plc.r_akim1Ort.ToString() + " A";
                lblG2Akim.Content = Plc.r_akim2Ort.ToString() + " A";

                comboBox.IsEnabled = false;
                btnReceteSec.IsEnabled = false;

            }
            else
            {
                txb_damper1.IsEnabled = true; txb_damper2.IsEnabled = true;
                if (Plc.r_total_elapsed_time == 0)
                {
                    MachineStatu.Content = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.durduruldu;
                    MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

                    ElapsedTime.Visibility = Visibility.Hidden;
                    ElapsedTimeStep.Visibility = Visibility.Hidden;
                    comboBox.IsEnabled = true;
                    btnReceteSec.IsEnabled = true;
                }
                else
                {
                    MachineStatu.Content = AmerikaKamaraFirin.Resources.Machine_Statu + " " + AmerikaKamaraFirin.Resources.durakladi;
                    MachineStatu.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                }

            }

            TimeSpan elapsed = TimeSpan.FromSeconds(Plc.r_total_elapsed_time);
            TimeSpan elapsedstep = TimeSpan.FromSeconds(Plc.r_total_elapsed_time);

            ElapsedTime.Content = $"{AmerikaKamaraFirin.Resources.gecenZaman} : {elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            ElapsedTimeStep.Content = $"{AmerikaKamaraFirin.Resources.recipegecenZaman} : {elapsedstep.Hours:D2}:{elapsedstep.Minutes:D2}:{elapsedstep.Seconds:D2}";

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





            if (!Plc.r_veriGeldi && !Plc.plcyaz && Plc.plcyazokundu)
            {

                Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
                Plc.plcoku = false;

                if (Plc.r_step < 0) Plc.r_step = 0;
                if (Plc.r_step > 100) Plc.r_step = 0;

                Plc.w_setTemp1 = Globals.seciliRecete.Adimlar[Plc.r_step].HedefSicaklik1;
                Plc.w_setTemp2 = Globals.seciliRecete.Adimlar[Plc.r_step].HedefSicaklik2;
                Plc.w_setTime = Globals.seciliRecete.Adimlar[Plc.r_step].SureDakika * 60;
                Plc.w_damper1 = Globals.seciliRecete.Adimlar[Plc.r_step].BacaAciklik1;
                Plc.w_damper3 = Globals.seciliRecete.Adimlar[Plc.r_step].BacaAciklik2;
                Plc.w_adimSayisi = Globals.seciliRecete.Adimlar.Count();

                S7.SetDIntAt(Plc.writeBuffer, 2, Plc.w_setTemp1);
                S7.SetDIntAt(Plc.writeBuffer, 6, Plc.w_setTime);
                S7.SetDIntAt(Plc.writeBuffer, 10, Plc.w_setTemp2);
                S7.SetDIntAt(Plc.writeBuffer, 22, Plc.w_damper1);
                S7.SetDIntAt(Plc.writeBuffer, 26, Plc.w_damper3);
                S7.SetBitAt(Plc.writeBuffer, 42, 2, true);
                S7.SetBitAt(Plc.writeBuffer, 42, 3, true);
                S7.SetDIntAt(Plc.writeBuffer, 38, 10);
                S7.SetDIntAt(Plc.writeBuffer, 64, Plc.w_adimSayisi);

                Plc.plcyaz = true;

            }

            if (Plc.plcokundu && Plc.r_firinDurum)
            {
                double toplamSure = Globals.seciliRecete?.Adimlar?.Sum(a => a.SureDakika) * 60 ?? 3600;
                Plc.UpdateLiveTempData(toplamSure); // yeni veri topla
                LiveTemp();                         // sadece son noktayı çiz
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



            if (Plc.r_sagKapiKapali) sagLeftRight.RightButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiAcik) sagLeftRight.LeftButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiYukarda) sagUpDown.RightButtonBackground = Brushes.Gray;
            if (Plc.r_sagKapiAssada) sagUpDown.LeftButtonBackground = Brushes.Gray;

            if (Plc.r_solKapiKapali) solLeftRight.RightButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiAcik) solLeftRight.LeftButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiYukarda) solUpDown.RightButtonBackground = Brushes.Gray;
            if (Plc.r_solKapiAssada) solUpDown.LeftButtonBackground = Brushes.Gray;


            SolKlepeRotate.Angle = (Plc.r_damper1 / 2) * -1;
            SagKlepeRotate.Angle = (Plc.r_damper3 / 2);
            lblDamper1.Content = Plc.r_damper1.ToString() + " %";
            lblDamper2.Content = Plc.r_damper3.ToString() + " %";




            lbl_tc1.Content = Plc.r_Tc1.ToString() + " C";
            lbl_tc2.Content = Plc.r_Tc2.ToString() + " C";
            lbl_tc1recete.Content = Plc.r_Tc1recete.ToString() + " C";
            lbl_tc2recete.Content = Plc.r_Tc2recete.ToString() + " C";


        }




        private void LiveTemp()
        {
            if (Globals.seciliRecete == null || Globals.seciliRecete.Adimlar == null || Globals.seciliRecete.Adimlar.Count == 0)
                return;

            if (Globals.LiveDataList.Count == 0) return;

            double totalSure = Globals.seciliRecete.Adimlar.Sum(a => a.SureDakika) * 60;
            double xScale = trendGraph.Width / totalSure;
            double yScale = trendGraph.Height / Globals.seciliRecete.Adimlar.Max(a => a.HedefSicaklik1);

            var last = Globals.LiveDataList.Last();
            double x = last.Time * xScale;
            if (x > trendGraph.Width) return;

            double y1 = trendGraph.Height - (last.Tc1 * yScale);
            double y2 = trendGraph.Height - (last.Tc2 * yScale);

            polylineLiveTemp1.Points.Add(new Point(x, y1));
            polylineLiveTemp2.Points.Add(new Point(x, y2));
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
            if (comboBox.Items.Count > Globals.comboIndex)
            {
                comboBox.SelectedIndex = Globals.comboIndex;
            }
            else
            {
                comboBox.SelectedIndex = 0;
            }
        }
        public void CreateTrendCanvas()
        {
            trendGraph.Children.Clear();

            Globals.seciliRecete = comboBox.SelectedItem as Recete;
            if (Globals.seciliRecete == null || Globals.seciliRecete.Adimlar == null || Globals.seciliRecete.Adimlar.Count < 1)
                return;

            var polyline = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 1,
                StrokeLineJoin = PenLineJoin.Round
            };

            double maxSicaklik = Globals.seciliRecete.Adimlar.Max(a => a.HedefSicaklik1);
            double minSicaklik = Globals.seciliRecete.Adimlar.Min(a => a.HedefSicaklik1);
            double range = maxSicaklik - minSicaklik;
            if (range == 0) range = 1;

            double totalSure = Globals.seciliRecete.Adimlar.Sum(a => a.SureDakika);

            double xScale = trendGraph.Width / totalSure;
            double yScale = trendGraph.Height / maxSicaklik;

            double now = 0;
            polyline.Points.Add(new Point(0, trendGraph.Height)); // ilk nokta alt köşe

            foreach (var adim in Globals.seciliRecete.Adimlar)
            {
                now += adim.SureDakika * xScale;
                double y = trendGraph.Height - (adim.HedefSicaklik1 * yScale);
                polyline.Points.Add(new Point(now, y));
            }

            trendGraph.Children.Add(polyline); // yeni çizim

            polylineLiveTemp1.Stroke = Brushes.Red;
            polylineLiveTemp1.StrokeThickness = 1;

            polylineLiveTemp2.Stroke = Brushes.Orange;
            polylineLiveTemp2.StrokeThickness = 1;

            trendGraph.Children.Add(polylineLiveTemp1);
            trendGraph.Children.Add(polylineLiveTemp2);




            var polyline2 = new Polyline
            {
                Stroke = Brushes.LightSkyBlue,
                StrokeThickness = 1,
                StrokeLineJoin = PenLineJoin.Round
            };

            double maxSicaklik2 = Globals.seciliRecete.Adimlar.Max(a => a.HedefSicaklik2);
            double minSicaklik2 = Globals.seciliRecete.Adimlar.Min(a => a.HedefSicaklik2);
            double range2 = maxSicaklik2 - minSicaklik2;
            if (range2 == 0) range2 = 1;

            double totalSure2 = Globals.seciliRecete.Adimlar.Sum(a => a.SureDakika);

            double xScale2 = trendGraph.Width / totalSure2;
            double yScale2 = trendGraph.Height / maxSicaklik2;

            double now2 = 0;
            polyline2.Points.Add(new Point(0, trendGraph.Height));

            foreach (var adim in Globals.seciliRecete.Adimlar)
            {
                now2 += adim.SureDakika * xScale;
                double y = trendGraph.Height - (adim.HedefSicaklik2 * yScale2);
                polyline2.Points.Add(new Point(now2, y));
            }


            trendGraph.Children.Add(polyline2);



            var polyline3 = new Polyline[Globals.seciliRecete.Adimlar.Count()];

            double xAdimSonu = 0;

            for (int i = 0; i < Globals.seciliRecete.Adimlar.Count(); i++)
            {
                if (i < Globals.seciliRecete.Adimlar.Count() - 1)
                {
                    polyline3[i] = new Polyline
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round

                    };

                    xAdimSonu += Globals.seciliRecete.Adimlar[i].SureDakika * xScale;

                    polyline3[i].Points.Add(new Point(xAdimSonu, 0));
                    polyline3[i].Points.Add(new Point(xAdimSonu, trendGraph.Height));

                    trendGraph.Children.Add(polyline3[i]);
                }
            }



        }

        private void solUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            solUpDown.LeftButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 2, true);
            S7.SetBitAt(Plc.writeBuffer, 0, 3, false);

            Plc.plcyaz = true;
        }

        private void solUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            solUpDown.RightButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 2, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 3, true);
            Plc.plcyaz = true;
        }

        private void solLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            solLeftRight.LeftButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 0, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 1, true);

            Plc.plcyaz = true;
        }

        private void solLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            solLeftRight.RightButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;


            S7.SetBitAt(Plc.writeBuffer, 0, 0, true);
            S7.SetBitAt(Plc.writeBuffer, 0, 1, false);

            Plc.plcyaz = true;
        }

        private void sagUpDown_ArrowLeftClicked(object sender, EventArgs e)
        {
            sagUpDown.LeftButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 7, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 6, true);

            Plc.plcyaz = true;
        }

        private void sagUpDown_ArrowRightClicked(object sender, EventArgs e)
        {
            sagUpDown.RightButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 6, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 7, true);

            Plc.plcyaz = true;
        }

        private void sagLeftRight_ArrowLeftClicked(object sender, EventArgs e)
        {
            sagLeftRight.LeftButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;

            S7.SetBitAt(Plc.writeBuffer, 0, 4, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 5, true);

            Plc.plcyaz = true;
        }

        private void sagLeftRight_ArrowRightClicked(object sender, EventArgs e)
        {
            sagLeftRight.RightButtonBackground = Brushes.Orange;
            Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
            Plc.plcoku = false;


            S7.SetBitAt(Plc.writeBuffer, 0, 5, false);
            S7.SetBitAt(Plc.writeBuffer, 0, 4, true);

            Plc.plcyaz = true;
        }

        private void btnReceteSec_Click(object sender, RoutedEventArgs e)
        {
            CreateTrendCanvas();
            Globals.comboIndex = comboBox.SelectedIndex;
            Globals.LiveDataList.Clear();
            Globals.lastRecordedTime = 0;
            try { File.Delete(Globals.LiveDataJsonPath); } catch { }
        }




        private void LoadLiveDataFromJson()
        {
            if (File.Exists(Globals.LiveDataJsonPath))
            {
                try
                {
                    string json = File.ReadAllText(Globals.LiveDataJsonPath);
                    var data = JsonSerializer.Deserialize<List<LiveDataPoint>>(json);
                    if (data != null)
                    {
                        Globals.LiveDataList = data;
                        Globals.lastRecordedTime = data.LastOrDefault()?.Time ?? 0;
                    }
                }
                catch { Globals.LiveDataList.Clear(); }
            }
        }
        private void DrawOldDataOnGraph()
        {
            if (Globals.seciliRecete == null || Globals.seciliRecete.Adimlar == null || Globals.seciliRecete.Adimlar.Count == 0)
                return;

            double totalSure = Globals.seciliRecete.Adimlar.Sum(a => a.SureDakika) * 60;
            double xScale = trendGraph.Width / totalSure;
            double yScale = trendGraph.Height / Globals.seciliRecete.Adimlar.Max(a => a.HedefSicaklik1);

            polylineLiveTemp1.Points.Clear();
            polylineLiveTemp2.Points.Clear();

            foreach (var data in Globals.LiveDataList)
            {
                double x = data.Time * xScale;
                if (x > trendGraph.Width) continue;

                double y1 = trendGraph.Height - (data.Tc1 * yScale);
                double y2 = trendGraph.Height - (data.Tc2 * yScale);

                polylineLiveTemp1.Points.Add(new Point(x, y1));
                polylineLiveTemp2.Points.Add(new Point(x, y2));
            }
        }
        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox txb)
            {
                num = new Numpad(txb.Text, 0, 100);
                if (num.ShowDialog() == true)
                {
                    string damper = "";
                    int plcByte = 30;
                    if (txb.Name == "txb_damper1")
                    {
                        damper = "Damper1";
                        plcByte = 30;
                    }
                    else if (txb.Name == "txb_damper2")
                    {
                        damper = "Damper2";
                        plcByte = 34;
                    }
                    Array.Copy(Plc.writereadBuffer, Plc.writeBuffer, Plc.writereadBuffer.Length);
                    Plc.plcoku = false;

                    ushort deger = ushort.TryParse(num.GirilenMetin, out var val) ? val : (ushort)0;

                    SetSetting(damper, deger);

                    S7.SetDIntAt(Plc.writeBuffer, plcByte, deger);

                    Plc.plcyaz = true;

                    txb.Text = num.GirilenMetin;
                }
            }
        }
    }
}
