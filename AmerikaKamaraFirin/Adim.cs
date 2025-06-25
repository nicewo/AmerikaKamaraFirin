using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmerikaKamaraFirin
{
    public class Adim
    {
        public int HedefSicaklik1 { get; set; }
        public int HedefSicaklik2 { get; set; }
        public int SureDakika { get; set; }
        public int BacaAciklik1 { get; set; }
        public int BacaAciklik2 { get; set; }

        [JsonIgnore] // kaydetmesin
        public int AdimNo { get; set; }
    }
}
