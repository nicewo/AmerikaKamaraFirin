using Sharp7;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace AmerikaKamaraFirin
{
    public class Config
    {
        //PLC Tanımlama
        public static S7Client
            Plc = new();
        public static int
            PlcStatu = 0;
        public static string
            PlcIP = "192.168.0.1";

        public static void LoadConfig()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml");

            // Eğer Config.xml yoksa, oluştur
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
            }

            // Config.xml dosyasını oku ve eksik değerleri tamamla
            XDocument config = XDocument.Load(configPath);

            // PLC Ayarları
            PlcIP = GetOrAddValue(config, "PlcIP", PlcIP);


            // Config.xml güncellenmişse kaydet
            config.Save(configPath);

        }
        private static string GetOrAddValue(XDocument config, string key, string? defaultValue)
        {
            XElement? element = config.Root?.Element(key);
            if (element == null || string.IsNullOrEmpty(element.Value))
            {
                // Değer yoksa, varsayılanı ekle
                element = new XElement(key, defaultValue);
                config.Root?.Add(element);
            }

            return element.Value;
        }

        private static void CreateDefaultConfig(string path)
        {
            XDocument defaultConfig = new XDocument(
                new XElement("Config",
                    new XElement("PlcIP", PlcIP)
                )
            );

            defaultConfig.Save(path);
        }
    }
}
