using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Path = System.IO.Path;
using AmerikaKamaraFirin;

namespace AmerikaKamaraFirin.View
{
    /// <summary>
    /// Recipe.xaml etkileşim mantığı
    /// </summary>
    public partial class Recipe : Page
    {
        private string RecipesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recipes");

        public Recipe()
        {
            InitializeComponent();
            LoadRecipes();
        }

        private void LoadRecipes()
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
                        RecipeWrapPanel.Children.Add(CreateRecipeCard(recete));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Reçete okunurken hata oluştu:\n{ex.Message}");
                }
            }
        }

        private Border CreateRecipeCard(Recete recete)
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });

            // Reçete adı
            var textblock = new TextBlock
            {
                Text = recete.Adi,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };
            grid.Children.Add(textblock);

            // Grafik
            var grafik = CreateTrendCanvas(recete);
            Grid.SetRow(grafik, 1);
            grid.Children.Add(grafik);

            var border = new Border
            {
                Width = 250,
                Height = 150,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
                CornerRadius = new CornerRadius(12),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand, // Hand imleç
                Child = grid
            };

            var buton = new Button
            {
                Content = "Delete",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.Red,
                Margin = new Thickness(8, 0, 8, 0),
                Width = 234,
            };
            grid.Children.Add(buton);
            Grid.SetRow(buton, 2);

            // Mouse tıklama olayı
            grafik.MouseLeftButtonUp += (s, e) =>
            {
                var duzenlePencere = new ReceteDuzenle(recete);
                duzenlePencere.ShowDialog(); // modal aç
            };

            textblock.MouseLeftButtonUp += (s, e) =>
            {
                var duzenlePencere = new ReceteDuzenle(recete);
                duzenlePencere.ShowDialog(); // modal aç
            };

            buton.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the recipe \"{recete.Adi}\"?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    ReceteSil(recete);
                }
            };

            return border;

        }

        private Canvas CreateTrendCanvas(Recete recete)
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

            double maxSicaklik = recete.Adimlar.Max(a => a.HedefSicaklik);
            double minSicaklik = recete.Adimlar.Min(a => a.HedefSicaklik);
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
                double y = recete.Adimlar[i].HedefSicaklik * range;
                y = canvas.Height - y; // Y eksenini ters çeviriyoruz
                polyline.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(polyline);
            return canvas;
        }

        private void ReceteEkle(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Reçete Ekle tıklandı");
        }

        private void ReceteSil(Recete recete)
        {
            string dosyaAdi = Globals.RemoveTurkishAndSpecialChars(recete.Adi);
            string yol = Path.Combine(RecipesFolder, dosyaAdi + ".json");
            if (File.Exists(yol))
            {
                File.Delete(yol);
                MessageBox.Show("Reçete başarıyla silindi.");
                RecipeWrapPanel.Children.Clear(); // Reçeteleri yeniden yükle
                LoadRecipes();
            }
            else
            {
                MessageBox.Show("Reçete bulunamadı.");
            }
        }
    }
}
