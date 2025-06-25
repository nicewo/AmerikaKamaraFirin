using AmerikaKamaraFirin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Recipe.xaml etkileşim mantığı
    /// </summary>
    public partial class Recipe : Page
    {
        public static string RecipesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
        public static WrapPanel StaticWrapPanelInstance;
        public static int cardWidth = 230;
        public Recipe()
        {
            InitializeComponent();
            StaticWrapPanelInstance = RecipeWrapPanel;
            cardWidth = (int)RecipeWrapPanel.ItemWidth - 80;
            LoadRecipes();
        }

        public static void LoadRecipes()
        {
            if (!Directory.Exists(RecipesFolder))
                return;

            var files = Directory.GetFiles(RecipesFolder, "*.json")
                                 .OrderBy(f => f) // alfabetik sıralama
                                 .ToList();
            newRecipePanel(); // Yeni reçete paneli ekle
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var recete = JsonSerializer.Deserialize<Recete>(json);

                    if (recete != null)
                        StaticWrapPanelInstance.Children.Add(CreateRecipeCard(recete));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{AmerikaKamaraFirin.Resources.recete_okunurken_hata_olustu}:\n{ex.Message}");
                }
            }
        }

        public static Border CreateRecipeCard(Recete recete)
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

            // Reçete adı
            string adi = recete.Adi;
            if (adi.Length > 25)
                adi = adi.Substring(0, 22) + "...";

            var textblock = new TextBlock
            {
                Text = adi,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8),
                IsHitTestVisible = false
            }; grid.Children.Add(textblock);

            // Grafik
            var grafik = CreateTrendCanvas(recete);
            Grid.SetRow(grafik, 1);
            grafik.IsHitTestVisible = false; // olaylar grid'e geçsin
            grid.Children.Add(grafik);

            int toplamsüre = recete.Adimlar.Sum(a => a.SureDakika);
            string toplamsüreStr = (toplamsüre / 60) + ":" + (toplamsüre % 60);
            var buton = new TextBlock
            {
                Text = AmerikaKamaraFirin.Resources.toplam_süre + "  " + toplamsüreStr,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(8, 0, 8, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 150,
                IsHitTestVisible = false
            };
            grid.Children.Add(buton);
            Grid.SetRow(buton, 2);

            var border = new Border
            {
                Width = cardWidth,
                Height = 150,
                Margin = new Thickness(35, 10, 35, 10),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
                CornerRadius = new CornerRadius(12),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Child = grid
            };

            // Tıklama olayını sadece bir yerde tanımla
            border.MouseLeftButtonUp += (s, e) =>
            {
                var duzenlePencere = new ReceteDuzenle(recete);
                duzenlePencere.ShowDialog();
            };

            return border;
        }


        public static Canvas CreateTrendCanvas(Recete recete)
        {
            var canvas = new Canvas { Margin = new Thickness(5), Width = 240, Height = 80 };

            if (recete.Adimlar == null || recete.Adimlar.Count < 2)
                return canvas;

            var polyline = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };
            var polyline2 = new Polyline
            {
                Stroke = Brushes.LightSkyBlue,
                StrokeThickness = 3,
                StrokeLineJoin = PenLineJoin.Round
            };

            double maxSicaklik = recete.Adimlar.Max(a => a.HedefSicaklik1);
            double minSicaklik = recete.Adimlar.Min(a => a.HedefSicaklik1);
            double range = maxSicaklik - minSicaklik;
            if (range == 0) range = 1;
            double timeRange = 0;
            for (int i = 0; i < recete.Adimlar.Count; i++)
            {
                timeRange += recete.Adimlar[i].SureDakika;
            }

            range = canvas.Height / maxSicaklik;
            timeRange = canvas.Width / timeRange;

            double xStep = canvas.Width / (recete.Adimlar.Count - 1);
            double now = 0;
            polyline.Points.Add(new Point(0, canvas.Height));

            for (int i = 0; i < recete.Adimlar.Count; i++)
            {
                double xPre = recete.Adimlar[i].SureDakika * timeRange;
                now = now + xPre;
                double x = now;
                double y = recete.Adimlar[i].HedefSicaklik1 * range;
                y = canvas.Height - y; // Y eksenini ters çeviriyoruz
                polyline.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(polyline);

            double maxSicaklik2 = recete.Adimlar.Max(a => a.HedefSicaklik2);
            double minSicaklik2 = recete.Adimlar.Min(a => a.HedefSicaklik2);
            double range2 = maxSicaklik2 - minSicaklik2;
            if (range2 == 0) range2 = 1;
            double timeRange2 = 0;
            for (int i = 0; i < recete.Adimlar.Count; i++)
            {
                timeRange2 += recete.Adimlar[i].SureDakika;
            }

            range2 = canvas.Height / maxSicaklik2;
            timeRange2 = canvas.Width / timeRange2;

            double xStep2 = canvas.Width / (recete.Adimlar.Count - 1);
            double now2 = 0;
            polyline2.Points.Add(new Point(0, canvas.Height));

            for (int i = 0; i < recete.Adimlar.Count; i++)
            {
                double xPre = recete.Adimlar[i].SureDakika * timeRange2;
                now2 = now2 + xPre;
                double x = now2;
                double y = recete.Adimlar[i].HedefSicaklik2 * range2;
                y = canvas.Height - y; // Y eksenini ters çeviriyoruz
                polyline2.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(polyline2);
            return canvas;
        }
        public static void newRecipePanel()
        {
            var border = new Border
            {
                Width = cardWidth,
                Height = 150,
                Margin = new Thickness(35,10,35,10),
                Background = (Brush)new BrushConverter().ConvertFromString("#FF1E1E2E"),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Cursor = Cursors.Hand
            };

            border.MouseLeftButtonUp += ReceteEkle;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

            var textBlock = new TextBlock
            {
                Text = AmerikaKamaraFirin.Resources.new_recipe, // resx erişimi
                Margin = new Thickness(0, 0, 0, 8),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };
            grid.Children.Add(textBlock);

            var canvas = new Canvas
            {
                Width = 240,
                Height = 80,
                Margin = new Thickness(5)
            };
            Grid.SetRow(canvas, 1);

            var polyline = new Polyline
            {
                Stroke = Brushes.LightGreen,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeThickness = 3,
                Points = new PointCollection
        {
            new Point(0, 80),
            new Point(240, 80)
        }
            };

            canvas.Children.Add(polyline);
            grid.Children.Add(canvas);

            border.Child = grid;

            // Panel'e ekle (örneğin: RecipeWrapPanel)
            StaticWrapPanelInstance.Children.Add(border);
        }

        public static void ReceteEkle(object sender, MouseButtonEventArgs e)
        {
            Recete _recete = new Recete();

            string klasor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");
            string[] jsonDosyalari = Directory.GetFiles(klasor, "*.json");
            int adet = jsonDosyalari.Length;
            _recete.Adi = AmerikaKamaraFirin.Resources.new_recipe + " - " + adet;


            var rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                _recete.Adimlar.Add(new Adim
                {
                    AdimNo = i + 1,
                    HedefSicaklik1 = rnd.Next(100, 300),
                    HedefSicaklik2 = rnd.Next(100, 300),
                    SureDakika = rnd.Next(10, 60),
                    BacaAciklik1 = rnd.Next(0, 100),
                     BacaAciklik2 = rnd.Next(0, 100)
                });
            }


            // Güvenli dosya adı oluştur


            string dosyaAdi = Globals.RemoveTurkishAndSpecialChars(_recete.Adi);



            Directory.CreateDirectory(klasor);
            string yol = Path.Combine(klasor, dosyaAdi + ".json");

            // JSON serialize ve kaydet
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            File.WriteAllText(yol, JsonSerializer.Serialize(_recete, options));
            MessageBox.Show(AmerikaKamaraFirin.Resources.recete_basariyla_kaydedildi);
            StaticWrapPanelInstance.Children.Clear(); // Reçeteleri yeniden yükle
            LoadRecipes();

            var duzenlePencere = new ReceteDuzenle(_recete);
            duzenlePencere.ShowDialog();

        }

        public static void ReceteSil(Recete recete)
        {
            string dosyaAdi = Globals.RemoveTurkishAndSpecialChars(recete.Adi);
            string yol = Path.Combine(RecipesFolder, dosyaAdi + ".json");
            if (File.Exists(yol))
            {
                File.Delete(yol);
                MessageBox.Show(AmerikaKamaraFirin.Resources.recete_basariyla_silindi);
                StaticWrapPanelInstance.Children.Clear(); // Reçeteleri yeniden yükle
                LoadRecipes();
            }
            else
            {
                MessageBox.Show(AmerikaKamaraFirin.Resources.recete_bulunamadi);
            }
        }


    }
}
