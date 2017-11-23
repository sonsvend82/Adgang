using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adgang
{
    class Kort // Klasse for adgangskort
    {
        private int kortID;

        public int KortID
        {
            get { return kortID; }
            set { kortID = value; }
        }
        private string navn;

        public string Navn
        {
            get { return navn; }
            set { navn = value; }
        }
        private int pinKode;

        public int PinKode
        {
            get { return pinKode; }
            set { pinKode = value; }
        }
        private bool aktivt;

        public bool Aktivt
        {
            get { return aktivt; }
            set { aktivt = value; }
        }

        public Kort(int _kortID, string _navn, int _pinKode, bool _aktivt)
        {
            kortID = _kortID;
            navn = _navn;
            pinKode = _pinKode;
            aktivt = _aktivt;
        }
    }
}
