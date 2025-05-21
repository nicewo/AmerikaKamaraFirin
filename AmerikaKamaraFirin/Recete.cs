using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmerikaKamaraFirin
{
    public class Recete
    {
        public string Adi { get; set; }
        public List<Adim> Adimlar { get; set; } = new List<Adim>();
    }
}
