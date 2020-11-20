using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using Microsoft.Win32;
using System.Windows.Threading;

namespace lecteurSousTitre
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        

        
        public MainWindow()
        {
            InitializeComponent();
            PauseTextBox.Text = "0";

            

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


        }

      

        void timer_Tick(object sender, EventArgs e)
        {
            if (VideoControl.Source != null)
            {
                if (VideoControl.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", VideoControl.Position.ToString(@"mm\:ss"), VideoControl.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }



        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            
            VideoControl.Pause();
            
            PauseTextBox.Text = "0";

        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PauseTextBox.Text = "1";
            
            VideoControl.Play();

            


        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            PauseTextBox.Text = "2";
            VideoControl.Stop();
            
        }

        private void ChoixVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            openDlg.InitialDirectory = @"c:\";

            openDlg.ShowDialog();

            MediaPathTextBox.Text = openDlg.FileName;
        }

        private void ChoixSrt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.InitialDirectory = @"c:\";

            openDlg.ShowDialog();

            SrtTextBox.Text = openDlg.FileName;
        }

        private void ValiderFichier_Click(object sender, RoutedEventArgs e)
        {

            if (MediaPathTextBox.Text.Length <= 0)

            {

                MessageBox.Show("Choisissez une vidéo");

                return;

            }
            if (SrtTextBox.Text.Length <= 0)

            {

                MessageBox.Show("Choisissez un fichier de sous-titre");

                return;

            }

            PauseTextBox.Text = "1";
            VideoControl.Source = new Uri(MediaPathTextBox.Text);
            VideoControl.Play();
            string CheminSrt = SrtTextBox.Text;
            Srt srt = new Srt(CheminSrt);
            srt.Lecture(soustitre, PauseTextBox, VideoControl, AvanceRapide, Pla, Pla2);

        }

        private void diminuer_Click(object sender, RoutedEventArgs e)
        {
            if (soustitre.FontSize >= 3)
            {
                soustitre.FontSize -= 1;
            }
        }

        private void augmenter_Click(object sender, RoutedEventArgs e)
        {
            if (soustitre.FontSize <= 10)
            {
                soustitre.FontSize += 1;
            }
        }


        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            VideoControl.Volume = (double)volumeSlider.Value;
        }


    }
}
