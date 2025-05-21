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
        public int HedefSicaklik { get; set; }
        public int SureDakika { get; set; }

        [JsonIgnore] // kaydetmesin
        public int AdimNo { get; set; }
    }
}
