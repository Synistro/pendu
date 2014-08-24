using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace Pendu
{
    class Program
    {
        #region Variables
        public enum Difficulte { Facile, Normal, Difficile }
        public static string prenom;
        public static string mot;
        public static Dictionary<char, bool> lettresJouees = new Dictionary<char, bool>();
        public static int pointsVie;
        public static int rejouer;
        public static int score;
        public static Difficulte difficulte;
        #endregion

        static void Main()
        {
            Console.Title = Message.TitreConsole;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Message.Langue);
            Console.WriteLine(Message.Fr);
            Console.WriteLine(Message.En);
            var demandeLangue = Console.ReadKey(false);
            while (demandeLangue.KeyChar != '1' && demandeLangue.KeyChar != '2')
            {
                Console.WriteLine(Message.ErreurNombre);
                demandeLangue = Console.ReadKey(false);
            }
            if (demandeLangue.KeyChar == '1')
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr");
            }
            if (demandeLangue.KeyChar == '2')
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }

            Console.Clear();
            Console.Title = Message.TitreConsole;
            Console.WriteLine(Message.MessageBienvenue, Properties.Settings.Default.MeilleurScore);
            Console.WriteLine(Message.DemandePrenom);

            //ColorConsole.WriteLine("Bonjour et bienvenue ! \nQuel est votre prénom ?", ConsoleColor);

            prenom = Console.ReadLine();
            rejouer = 1;
            while (rejouer == 1)
            {
                NouvellePartie();
                Console.WriteLine(Message.DemandeRejouer);
                Console.WriteLine(Message.Rejouer1);
                Console.WriteLine(Message.Rejouer2);
                var demandeRejouer = Console.ReadKey(false);
                while (demandeRejouer.KeyChar != '1' && demandeRejouer.KeyChar != '2')
                {
                    Console.WriteLine(Message.ErreurNombre);
                    demandeRejouer = Console.ReadKey(false);
                }
                if (demandeRejouer.KeyChar == '2')
                    rejouer = 0;
            }
        }

        static void NouvellePartie()
        {
            Console.Clear();
            score = 0;
            lettresJouees.Clear();
            difficulte = DemanderDifficulte();
            Console.Clear();
            Console.WriteLine(Message.DispDifficulte, prenom, ObtenirLibellePourNiveau(difficulte));
            mot = ChoisirMotAleatoirement();
            while (ConstruireMotPourAffichage().Contains("_") && pointsVie > 0)
            {
                DemandeLettreJoueur();
            }
            if (pointsVie == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Message.FinFalse, prenom, mot);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Message.FinTrue, prenom, mot, score, score > 1 || score < -1 ? "s" : "");
                Console.ForegroundColor = ConsoleColor.White;
                if (score <= Properties.Settings.Default.MeilleurScore)
                {
                    Console.WriteLine(Message.DispMeilleurScore, Properties.Settings.Default.MeilleurScore);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(Message.NouveauMeilleurScore, Properties.Settings.Default.MeilleurScore);
                    Properties.Settings.Default.MeilleurScore = score;
                    Properties.Settings.Default.Save();
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        static Difficulte DemanderDifficulte()
        {
            Console.WriteLine(Message.DiffChoix, prenom);
            Console.WriteLine(Message.Diff1Desc);
            Console.WriteLine(Message.Diff2Desc);
            Console.WriteLine(Message.Diff3Desc);
            var niveau = Console.ReadKey(false);
            while (niveau.KeyChar != '1' && niveau.KeyChar != '2' && niveau.KeyChar != '3')
            {
                Console.WriteLine(Message.ErreurNombre);
                niveau = Console.ReadKey(false);
            }
            switch (niveau.KeyChar)
            {
                case '1':
                    pointsVie = 15;
                    return Difficulte.Facile;
                case '2':
                    pointsVie = 10;
                    return Difficulte.Normal;
                case '3':
                    pointsVie = 5;
                    return Difficulte.Difficile;
            }
            return Difficulte.Facile;
        }

        static string ChoisirMotAleatoirement()
        {
            var lignes = File.ReadAllLines(@"mots.txt");
            var rand = new Random();
            return lignes[rand.Next(lignes.Length)];
        }

        static string ConstruireMotPourAffichage()
        {
            var motPourAffichage = "";
            var enumerator = mot.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var lettreTrouvees = lettresJouees.Where(lj => lj.Value).Select(lj => lj.Key).ToList();
                if (lettreTrouvees.Contains(enumerator.Current))
                    motPourAffichage += enumerator.Current;
                else
                    motPourAffichage += "_";
                motPourAffichage += " ";
            }
            return motPourAffichage;
        }

        static void DemandeLettreJoueur()
        {
            // ESANTIRULO
            Console.WriteLine(Message.DispMot, ConstruireMotPourAffichage());
            Console.WriteLine(Message.DemandeLettre);
            var lettre = Convert.ToChar(Console.ReadKey().KeyChar.ToString().ToUpper());
            Console.Clear();
            if (!char.IsLetter(lettre))
            {
                Console.WriteLine(Message.DemandeLettre);
                return;
            }
            if (lettresJouees.ContainsKey(lettre))
            {
                Console.WriteLine(Message.LettreJouee);
                return;
            }
            var verifLettre = mot.IndexOf(lettre);
            var emplacementLettre = mot.IndexOf(lettre) + 1;
            if (verifLettre == -1)
            {
                Console.Beep(100, 200);
                pointsVie--;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Message.LettreFalse, lettre, pointsVie, pointsVie > 1 ? "s" : "");
                lettresJouees.Add(lettre, false);
                MettreAJourScore(false);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.Beep(800, 200);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(Message.LettreTrue, lettre, emplacementLettre);
                lettresJouees.Add(lettre, true);
                ConstruireMotPourAffichage();
                MettreAJourScore(true);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void MettreAJourScore(bool result)
        {
            if (result)
            {
                if (difficulte == Difficulte.Facile)
                    score += 8;
                else if (difficulte == Difficulte.Normal)
                    score += 10;
                else if (difficulte == Difficulte.Difficile)
                    score += 13;

            }
            else
            {
                if (difficulte == Difficulte.Facile)
                    score -= 3;
                else if (difficulte == Difficulte.Normal)
                    score -= 5;
                else if (difficulte == Difficulte.Difficile)
                    score -= 7;
            }
        }

        static string ObtenirLibellePourNiveau(Difficulte difficulte)
        {
            switch (difficulte)
            {
                case Difficulte.Facile:
                    return Message.Diff1;
                case Difficulte.Normal:
                    return Message.Diff2;
                case Difficulte.Difficile:
                    return Message.Diff3;
            }
            return Message.Diff1;
        }

        //public class DifficulteInfo
        //{
        //    private Difficulte niveau;
        //    public Difficulte Niveau
        //    {
        //        get { return niveau; }
        //        set { niveau = value; }
        //    }

        //    public string Label
        //    {
        //        get
        //        {
        //            if (niveau == Difficulte.Facile) return Message.DispDifficulte;
        //        }
        //    }

        //}
    }
}
