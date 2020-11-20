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



            string mydocpath =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamReader sr = new StreamReader(path))
            {
                string l = "";
                int nLigne = 1;
                while ((l = sr.ReadLine()) != null)
                {
                    //int nLigne = 1;

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
                        //string newValeur = texte[i - 1] + "&" + l;

                        //texte[i - 1] = newValeur;
                    }
                    //ajouter à texte




                    nLigne++;
                    //Console.WriteLine(l);
                }

            }




            //lecture du fichier

        }

        public async Task Lecture(TextBlock sousTitre, TextBox EnPause, MediaElement video, ContentControl avanceRapide, ContentControl debug, ContentControl debug2)
        {
            
            int l = debut.Count;
            int i = 0;
            
            while (i < l)
            {
     
                Task attente = AttenteEntreSousTitre(i);

                

                //while (!attente.IsCompleted)
                //{
                    

                //}

                await attente;

                if (EnPause.Text == "0")
                {
                    Task pause = Pause(EnPause);
                    await pause;

                    
                    TimeSpan RattrapeVideo = TransformHeure(debut[i]) - video.Position;
                    debug.Content = TransformHeure(debut[i]).ToString();
                    debug2.Content = video.Position.ToString();
                    await Task.Delay(RattrapeVideo);
                    

                }
                if (EnPause.Text == "2")
                {
                    Task stop = Pause(EnPause);
                    await stop;


                    count = new TimeSpan(0);
                    i = 0;
                    continue;


                }


                // blbl = true
                // récupéerer bon i

                //remettre str au bon moment

                AfficheStr(texte[i], sousTitre);

                //if (avanceRapide.Content == "1")
                //{

                //    int j = 0;

                //    TimeSpan where = video.Position;
                //    debug.Content = where;
                //    while (where > TransformHeure(debut[j]))
                //    {
                //        j++;
                //        avanceRapide.Content = "3";
                //    }

                //    if (video.Position > TransformHeure(fin[j - 1]))
                //    {
                //        i = j - 1;
                //        count = video.Position;
                //        avanceRapide.Content = "0";
                //    }

                //    if (video.Position < TransformHeure(fin[j - 2]))
                //    {
                //        i = j - 2;
                //        count = video.Position;
                //        avanceRapide.Content = "0";
                //        continue;
                //    }


                //}






                
                Task TempsSrt = TempsAffichageStr(i);

                await TempsSrt;

                if (EnPause.Text == "0")
                {
                    Task pause1 = Pause(EnPause);
                    await pause1;


                    //if (video.Position < TransformHeure(fin[i]))
                    //{


                    TimeSpan RattrapeVideo2 = TransformHeure(fin[i]) - video.Position;
                    debug.Content = TransformHeure(fin[i]).ToString();
                    debug2.Content = video.Position.ToString();
                    await Task.Delay(RattrapeVideo2);
                    //}

                }

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

        public void AfficheStr(List<string> str, TextBlock sousTitre)
        {
            
            sousTitre.Text = str[0];
            if(str.Count > 1)
            {
                sousTitre.Text = sousTitre.Text + Environment.NewLine + str[1];
            }

            
        }
        
        public async Task Pause(TextBox EnPause)
        {
            if (EnPause.Text == "0" || EnPause.Text == "2")
            {

                await Task.Delay(1);
                Task u = Pause(EnPause);
                await u;

               
            }

           
        }
        

     
        public async Task AttenteEntreSousTitre(int i)
        {
            TimeSpan heure = this.TransformHeure(debut[i]);
            TimeSpan attente = heure - count;
            await Task.Delay(attente);
            count = heure;
        }

        public async Task TempsAffichageStr(int i)
        {
            TimeSpan heure = this.TransformHeure(fin[i]);
            TimeSpan attente = heure - count;
            await Task.Delay(attente);
            count = heure;

            
        }


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
