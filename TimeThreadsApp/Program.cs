using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace Projekt_główny
{

    class Mieszkanie
    {
        public String miasto;
        public String dzielnica;
        public double powierzchnia;
        public int cena_za_metr;

        public Mieszkanie(string miasto, string dzielnica, double powierzchnia, int cena_za_metr)
        {
            this.miasto = miasto;
            this.dzielnica = dzielnica;
            this.powierzchnia = powierzchnia;
            this.cena_za_metr = cena_za_metr;
        }

        public String pokaz()
        {

            return "Miasto: " + this.miasto + " Dzielnica: " + string.Format("{0,13:D}", this.dzielnica) + " Powierzchnia: " + string.Format("{0,7:N2} [m2]", this.powierzchnia) + " Cena: " + string.Format("{0,7:N2} [zł]", this.cena_za_metr);
        }

    }

    class Fabryka_mieszkan
    {
        private static Random rand = new Random();

        public static Mieszkanie generuj_mieszkanie(String[] dzielnice, double min_powierzchnia, double max_powierzchnia, int min_cena_za_metr, int max_cena_za_metr)
        {


            Mieszkanie m = new Mieszkanie("Krakow",
                                          dzielnice[(int)rand.Next(0, dzielnice.Length)],
                                          rand.NextDouble() * ((double)max_powierzchnia - (double)min_powierzchnia) + (double)min_powierzchnia,
                                          rand.Next(min_cena_za_metr, max_cena_za_metr));

            Console.WriteLine("wątek nr. {0} dane: {1}", string.Format("{0,2:D}", Thread.CurrentThread.ManagedThreadId), m.pokaz());
            System.Threading.Thread.Sleep(100);
            return m;
        }
    }


    class Generuj
    {

        public static List<Mieszkanie> generuj_bez_rownoleglosci(string[] dzielnice_lista,
                                                                 double min_powierzchnia,
                                                                 double max_powierzchnia,
                                                                 int min_cena_za_metr,
                                                                 int max_cena_za_metr,
                                                                 int ilosc_razy = 1)
        {

            List<Mieszkanie> mieszkania_lista = new List<Mieszkanie>();
            for (int i = 0; i < ilosc_razy; i++)
            {
                mieszkania_lista.Add(Fabryka_mieszkan.generuj_mieszkanie(dzielnice_lista,
                                                                         min_powierzchnia,
                                                                         max_powierzchnia,
                                                                         min_cena_za_metr,
                                                                         max_cena_za_metr));
            }
            return mieszkania_lista;
        }



        public static List<Mieszkanie> generuj_rownolegle_auto(string[] dzielnice_lista,
                                                          double min_powierzchnia,
                                                          double max_powierzchnia,
                                                          int min_cena_za_metr,
                                                          int max_cena_za_metr,
                                                          int ilosc_razy = 1,
                                                          int ilosc_watkow = 1)
        {

            List<Mieszkanie> mieszkania_lista2 = new List<Mieszkanie>();

            Parallel.For(0, ilosc_razy, i
               =>
            {

                mieszkania_lista2.Add(Fabryka_mieszkan.generuj_mieszkanie(dzielnice_lista,
                                                                    min_powierzchnia,
                                                                    max_powierzchnia,
                                                                    min_cena_za_metr,
                                                                    max_cena_za_metr));

            });

            return mieszkania_lista2;


        }

        public static List<Mieszkanie> generuj_rownolegle_manual(string[] dzielnice_lista,
                                                        double min_powierzchnia,
                                                        double max_powierzchnia,
                                                        int min_cena_za_metr,
                                                        int max_cena_za_metr,
                                                        int ilosc_razy = 1,
                                                        int ilosc_watkow = 1)
        {

            List<Mieszkanie> mieszkania_lista3 = new List<Mieszkanie>();


            Parallel.For(0, ilosc_razy, new ParallelOptions { MaxDegreeOfParallelism = ilosc_watkow },
                    (i, state)
               =>
                    {

                        mieszkania_lista3.Add(Fabryka_mieszkan.generuj_mieszkanie(dzielnice_lista,
                                                                            min_powierzchnia,
                                                                            max_powierzchnia,
                                                                            min_cena_za_metr,
                                                                            max_cena_za_metr));

                    });



            Console.WriteLine("\nIlosc uzytych watków: {0}", ilosc_watkow);
            return mieszkania_lista3;


        }
    }

    class Program
    {


        static void Main(string[] args)
        {
            string[] dzielnice_lista = { "Stare miasto", "Nowa Huta", "Podgórze", "Zwierzyniec", "Dębniki", "Bronowice", "Krowodrza", "Ruczaj", "Czyżyny", "Grzegórzki", "Prądnik Biały" };
            float min_powierzchnia = 40;
            float max_powierzchnia = 100;
            int min_cena_za_metr = 5000;
            int max_cena_za_metr = 15000;


            void menu()
            {

                Console.WriteLine("1. Metoda równoległa automatyczna i metoda sekwencyjna");
                Console.WriteLine("2. Metoda równoległa manualna i metoda sekwencyjna");
                Console.WriteLine("3. Zakończ");

                int caseSwitch;
                Console.Write("\nWybierz opcję: ");
                caseSwitch = Convert.ToInt32(Console.ReadLine());

                Console.Clear();

                switch (caseSwitch)
                {
                    case 1:
                        Console.WriteLine("Metoda równoległa automatyczna i metoda sekwencyjna:");

                        Console.WriteLine("\nPodaj liczbę generowań: ");
                        int liczba_generowań_auto = Convert.ToInt32(Console.ReadLine());

                        Console.Clear();

                        Console.WriteLine("\nMetoda równoległa: \n ");

                        Stopwatch sw1_auto;
                        sw1_auto = Stopwatch.StartNew();
                        Generuj.generuj_rownolegle_auto(dzielnice_lista,
                                                   min_powierzchnia,
                                                   max_powierzchnia,
                                                   min_cena_za_metr,
                                                   max_cena_za_metr,
                                                   liczba_generowań_auto);

                        sw1_auto.Stop();
                        Console.WriteLine("\nCZAS: {0} [Milliseconds]", sw1_auto.ElapsedMilliseconds);

                        Console.WriteLine("\nMetoda bez równoległości:");

                        Stopwatch sw2_auto;
                        sw2_auto = Stopwatch.StartNew();
                        Generuj.generuj_bez_rownoleglosci(dzielnice_lista,
                                                          min_powierzchnia,
                                                          max_powierzchnia,
                                                          min_cena_za_metr,
                                                          max_cena_za_metr,
                                                          liczba_generowań_auto);

                        sw2_auto.Stop();
                        Console.WriteLine("\nCZAS: {0} [Milliseconds]", sw2_auto.ElapsedMilliseconds);

                        Console.WriteLine("\nPodsumowanie:");
                        Console.WriteLine("\nCzas metody równoległej: {0} [Milliseconds]", sw1_auto.ElapsedMilliseconds);
                        Console.WriteLine("\nCzas metody sekwencyjnej: {0} [Milliseconds]", sw2_auto.ElapsedMilliseconds);

                        Console.WriteLine("\nKliknij dowolny klawisz aby wrócić do menu");
                        Console.ReadKey();
                        Console.Clear();
                        menu();


                        break;
                    case 2:
                        Console.WriteLine("Metoda równoległa manualna i metoda sekwencyjna:");

                        Console.WriteLine("\nPodaj liczbę generowań: ");
                        int liczba_generowań_manual = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("\nPodaj na ilu wątkach ma pracować program: ");
                        int liczba_wątków_manual = Convert.ToInt32(Console.ReadLine());

                        Console.Clear();

                        Console.WriteLine("\nMetoda równoległa: \n ");

                        Stopwatch sw1_manual;
                        sw1_manual = Stopwatch.StartNew();
                        Generuj.generuj_rownolegle_manual(dzielnice_lista,
                                                   min_powierzchnia,
                                                   max_powierzchnia,
                                                   min_cena_za_metr,
                                                   max_cena_za_metr,
                                                   liczba_generowań_manual, liczba_wątków_manual);

                        sw1_manual.Stop();
                        Console.WriteLine("\nCZAS: {0}", sw1_manual.ElapsedMilliseconds);


                        Console.WriteLine("\nMetoda bez równoległści: \n");

                        Stopwatch sw2_manual;
                        sw2_manual = Stopwatch.StartNew();
                        Generuj.generuj_bez_rownoleglosci(dzielnice_lista,
                                                          min_powierzchnia,
                                                          max_powierzchnia,
                                                          min_cena_za_metr,
                                                          max_cena_za_metr,
                                                          liczba_generowań_manual);

                        sw2_manual.Stop();
                        Console.WriteLine("\nCZAS: {0}", sw2_manual.ElapsedMilliseconds);

                        Console.WriteLine("\nPodsumowanie:");
                        Console.WriteLine("\nCzas metody równoległej: {0} [Milliseconds], na {1} wątkach", sw1_manual.ElapsedMilliseconds, liczba_wątków_manual);
                        Console.WriteLine("\nCzas metody sekwencyjnej: {0} [Milliseconds]", sw2_manual.ElapsedMilliseconds);


                        Console.WriteLine("\nKliknij dowolny klawisz aby wrócić do menu");
                        Console.ReadKey();
                        Console.Clear();
                        menu();



                        break;
                    case 3:
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }



            }


            menu();

        }
    }

}
