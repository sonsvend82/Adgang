using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adgang
{
    class ListeGenerator // Brukes for å populere listen i GUI og finne tilbake til valgte elementer for presentering av kort
    {
        
        public int KortID { get; set; }
        public string Navn { get; set; }
        public int PinKode { get; set; }
        public bool Aktivt { get; set; }
        public ListeGenerator(int kortid, string navn, int pinkode, bool aktivt)
        {
            this.KortID = kortid;
            this.Navn = navn;
            this.PinKode = pinkode;
            this.Aktivt = aktivt;
        }
    
    }
}
