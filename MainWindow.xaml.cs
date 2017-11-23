/****************************************************************************************************************
 * Kandidatnr: 408                                                                                              *
 * Dato: 01.06.15                                                                                               *
 * Kommentarer:                                                                                                 *
 *                                                                                                              *
 * Kunne flyttet mer kode til egne metoder/klasser men gikk tom for tid                                         *        
 *                                                                                                              *
 * ListView i utlevert kode er byttet med DataGrid for enklere column sortering                                 *
 *                                                                                                              *
 * Oppgaven mangler mye av tidsaspektene for alarm, kode inntasting etc.                                        *
 * Det er lagt inn noe dummy kode for alarm/tidtaking men disse fungerer ikke så de er kommentert ut            *
 *                                                                                                              *
 *                                                                                                              *
 * Presenter kort fungerer ved at man velger kort fra listen før man trykker på knappen,                        *
 * blinking av lyset starter ikke før man har tastet inn koden på 4 tegn                                        *
 *                                                                                                              *
 * *************************************************************************************************************/




using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Adgang
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Deklarerer variabler som brukes

        string input = "";  // Brukes for kode inntasting
        int tastetkode = 0;  // Brukes for kode inntasting
        int pinTeller = 0;  // Brukes for kode inntasting
        Kort aktivtKort;  // Brukes for kode inntasting
        Kort[] database = new Kort[10]; // Databasen for Kortene
        int databaseBrukt = 0;  // Antall brukte plasser i databasen (for utvidelse dersom det er flere en 10 kort totalt og populering av DataGrid listen)
        SolidColorBrush green = new SolidColorBrush(Colors.Green);  // Definerer en grønnfarge for kortleser blinking
        SolidColorBrush red = new SolidColorBrush(Colors.Red); // Definerer en rødfarge for kortleser blinking
        Stopwatch timer = new Stopwatch(); // Klokke for kode inntasting, brukes ikke
        bool isClosed = true; // Boolean for om døren er åpen eller lukket, brukes ikke men settes ved å trykke på åpne/lukke knappene
        Stopwatch alarm = new Stopwatch(); // Klokke for alarm dersom døren er åpen for lenge, brukes ikke
        public MainWindow()
        {
            InitializeComponent();
            // Legger til to kort i databasen
            database[0] = new Kort(0, "Jensen", 1234, true);
            databaseBrukt++;
            database[1] = new Kort(1, "Olsen", 2345, false);
            databaseBrukt++;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e) // Kjøres når datagrid er lastet, populerer listen
        {
            var items = new List<ListeGenerator>();
            for (int i = 0; i < databaseBrukt; i++)
            {
                items.Add(new ListeGenerator(database[i].KortID, database[i].Navn, database[i].PinKode, database[i].Aktivt));
            }
            listKort.ItemsSource = items;
        }

        private async void lampeBlinkGreen() // Blinking med lysene, settes til grønn når ferdig
        {
            for (int i = 0; i < 4; i++ )
            {
                
                if (i % 2 == 0)
                {
                    rectangleLampe.Fill = green;
                }
                else
                {
                    rectangleLampe.Fill = red;
                }
                await Task.Delay(500);
            }
            rectangleLampe.Fill = green;
        }

        private async void lampeBlinkRed() // Blinking med lysene, settes til rød når ferdig
        {
            for (int i = 0; i < 4; i++)
            {

                if (i % 2 == 0)
                {
                    rectangleLampe.Fill = green;
                }
                else
                {
                    rectangleLampe.Fill = red;
                }
                await Task.Delay(500);
            }
            rectangleLampe.Fill = red;
        }

        /* Ikke ferdig, 10 sekunder å taste inn koden (10 nye sekunder per kode trykk)
        private void nedTeller()
        {
            if (timer.IsRunning == false)
            {
                timer.Start();
            }
            else
            {
                timer.Restart();
            }
            if (timer.ElapsedMilliseconds > 10000)  // Dersom tider overskrider 10 sekunder nullstilles alt
            {                                       
                input = "";
                pinTeller = 0;
                tastetkode = 0;
                rectangleLampe.Fill = red;
                aktivtKort = null;
            }
        }
        */

        private void buttonAvslutt_Click(object sender, RoutedEventArgs e) // Avslutter programmet
        {
            this.Close();
        }

        void DataWindow_Closing(object sender, CancelEventArgs e) // Kjøres når programmet avsluttes
        {
            MessageBoxResult result = MessageBox.Show("Er du sikker på at du vil avslutte?", "Bekreft", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) // Knapp 1 til 0 er samme funksjon
        {
            if (aktivtKort != null) // Sjekker at det er presentert et kort
            {
                // nedTeller();
                input += 1; // Legger til et 1 tall på input string
                pinTeller++; // Teller at det er tastet inn et nytt pin siffer
                if (pinTeller == 4) // Dersom det er tastet inn 4 siffer, sjekker om pin koden stemmer med det presenterte kortet og blinker rødt/grønt
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {   // Dersom det legges inn logging av feil koder for max antall forsøk, reset denne counteren så de ikke blir stengt ute på neste dør
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else // Gjerne legge inn varsling/logging her for antall feil tastinger etc.
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4) // Dersom det er kommet en feil og mer en 4 siffer er blitt tastet inn
                {
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 2;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 3;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 4;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 5;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red; 
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 6;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 7;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 8;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 9;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void button0_Click(object sender, RoutedEventArgs e)
        {
            if (aktivtKort != null)
            {
                // nedTeller();
                input += 0;
                pinTeller++;
                if (pinTeller == 4)
                {
                    tastetkode = Convert.ToInt32(input);
                    if (tastetkode == aktivtKort.PinKode)
                    {
                        lampeBlinkGreen();
                        rectangleLampe.Fill = green;
                    }
                    else
                    {
                        lampeBlinkRed();
                        rectangleLampe.Fill = red;
                    }
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    aktivtKort = null;
                }
                else if (pinTeller > 4)
                {
                    lampeBlinkRed();
                    rectangleLampe.Fill = red;
                    input = "";
                    pinTeller = 0;
                    tastetkode = 0;
                    rectangleLampe.Fill = red;
                    aktivtKort = null;
                }
            }
        }

        private void buttonStar_Click(object sender, RoutedEventArgs e) // Tilbakestiller kode inntasting, men ikke kort
        {
            input = "";
            pinTeller = 0;
            tastetkode = 0;
            rectangleLampe.Fill = red;
        }



        private void buttonKort_Click(object sender, RoutedEventArgs e) // Finner det valgte kortet i databasen og setter det som aktivtKort
        {
            ListeGenerator valgtnavn = (ListeGenerator)listKort.SelectedItem;
            aktivtKort = database[valgtnavn.KortID];
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e) // Tilbakestiller kode/aktivt kort
        {
            input = "";
            pinTeller = 0;
            tastetkode = 0;
            rectangleLampe.Fill = red;
            aktivtKort = null;
        }

        private void buttonApen_Click(object sender, RoutedEventArgs e) // Apner døren
        {
            isClosed = false;
            // alarmCount(true);
        }

        private void alarmCount(bool startstop)  // Nedtelling for alarm, ikke implementert
        {
            if (startstop == true)
            {
                alarm.Start();
            }
            else
            {
                alarm.Reset();
            }

            if (alarm.ElapsedMilliseconds >= 60000)
            {
                alarmLampe.Fill = red; // Setter alarmen til rød dersom døren har stått åpen i 60 sekunder
            }
        }

        private void buttonLukket_Click(object sender, RoutedEventArgs e) // Lukker døren
        {
            isClosed = true;
            // alarmCount(false);
        }
    }
}
