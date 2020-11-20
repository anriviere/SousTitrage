using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace lecteurSousTitre
{


    class Srt
    {
        private List<string> debut = new List<string>();

        private List<string> fin = new List<string>();
        private List<List<string>> texte = new List<List<string>>();
        private TimeSpan count = new TimeSpan(0);
        
        
        public Srt(string path) 
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string l = "";
                int nLigne = 1;
                while ((l = sr.ReadLine()) != null)
                {
                    
                    if (l.Length == 0)
                    {
                        nLigne = 0;
                    }

                    if (nLigne == 2)
                    {
                        //ajouter 1er partie à début et 2eme partie à fin
                        string[] temps = l.Split(' ');
                        //temps[0] = début;
                        debut.Add(temps[0]);
                        //temps[2] = fin;
                        fin.Add(temps[2]);
                    }

                    if (nLigne == 3)
                    {
                       //add ligne a liste
                        List<string> ligne = new List<string>();
                        ligne.Add(l);
                        texte.Add(ligne);
                    }

                    if (nLigne > 3)
                    {
                        //ajoute à la suite du dernier index de la liste
                        int i = texte.Count;

                        texte[i - 1].Add(l);
                        
                    }
                   
                    nLigne++;
                    //Console.WriteLine(l);
                }

            }

        }

        public async Task Lecture(TextBlock sousTitre, TextBox EnPause, MediaElement video, ContentControl avanceRapide, ContentControl debug, ContentControl debug2)
        {
            
            int l = debut.Count;
            int i = 0;
            
            while (i < l)
            {
                //Attend le début du sous-titre
                Task attente = AttenteEntreSousTitre(i);
                await attente;

                //Mise en pause de la vidéo
                Task pause = ReprisePause(EnPause, video, debut, i);
                await pause;

                //Si on arrete la vidéo (définitivement = retour au début)
                if (EnPause.Text == "2")
                {
                    Task stop = Pause(EnPause);
                    await stop;
                    count = new TimeSpan(0);
                    i = 0;
                    continue;
                }

                //affiche le sous-titre
                AfficheStr(texte[i], sousTitre);  
                
                //Calcule le temps d'affichage du sous-titre
                Task TempsSrt = TempsAffichageStr(i);
                await TempsSrt;

                //Mise en pause de la vidéo
                Task pause2 = ReprisePause(EnPause, video, fin, i);
                await pause2;

                //Si on arrete la vidéo (définitivement = retour au début)
                if (EnPause.Text == "2")
                {
                    Task stop = Pause(EnPause);
                    await stop;
                    count = new TimeSpan(0);
                    i = 0;
                    continue;
                }
                sousTitre.Text = " ";

                
                i++;
                
               
            }


        }

        //Calcule le temps de décalage entre la vidéo et les sous-titre après une pause, et les réajuste
        public async Task ReprisePause(TextBox EnPause, MediaElement video, List<string> position, int i)
        {
            if (EnPause.Text == "0")
            {
                Task pause1 = Pause(EnPause);
                await pause1;

                TimeSpan RattrapeVideo2 = TransformHeure(position[i]) - video.Position;
                //debug.Content = TransformHeure(fin[i]).ToString();
                //debug2.Content = video.Position.ToString();
                await Task.Delay(RattrapeVideo2);

            }
        }


        //affiche les sous-titres
        public void AfficheStr(List<string> str, TextBlock sousTitre)
        {
            
            sousTitre.Text = str[0];
            if(str.Count > 1)
            {
                sousTitre.Text = sousTitre.Text + Environment.NewLine + str[1];
            }

        }
        
        //Met en pause les sous-titre jusqu'a la reprise
        public async Task Pause(TextBox EnPause)
        {
            if (EnPause.Text == "0" || EnPause.Text == "2")
            {
                await Task.Delay(1);
                Task u = Pause(EnPause);
                await u;
  
            }           
        }
        

        //attente entre chaque sous-titre
        public async Task AttenteEntreSousTitre(int i)
        {
            TimeSpan heure = this.TransformHeure(debut[i]);
            TimeSpan attente = heure - count;
            await Task.Delay(attente);
            count = heure;
        }


        //temps d'affichage d'un sous-titre
        public async Task TempsAffichageStr(int i)
        {
            TimeSpan heure = this.TransformHeure(fin[i]);
            TimeSpan attente = heure - count;
            await Task.Delay(attente);
            count = heure;

            
        }


        //transforme une string en TimeSpan
        public TimeSpan TransformHeure(string time)
        {
            string DateString = time.Replace(",", ".");
            DateTime DateFromString = DateTime.Parse(DateString);
            DateTime test = DateTime.Today;
            TimeSpan Intervalle = DateFromString - test;
            return Intervalle;
        }

       

    }
}
