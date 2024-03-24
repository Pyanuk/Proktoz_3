using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;

namespace Pleer
{
    public partial class MainWindow : Window
    {
        public List<string> listeningHistory = new List<string>();
        
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private ObservableCollection<string> playlist = new ObservableCollection<string>();
        private Random random = new Random();
        private DispatcherTimer timer = new DispatcherTimer();
        

        public MainWindow()
        {
            InitializeComponent();
            playList.ItemsSource = playlist;

            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

        }


        private void ListeningHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            ListeningHistoryWindow historyWindow = new ListeningHistoryWindow(listeningHistory); 
            historyWindow.ShowDialog();
        }



        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            SliderTrack.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SliderTrack.Value = mediaPlayer.Position.TotalSeconds;
        }

        private void AddTracks_OnClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            dialog.Title = "Выберите папку с музыкой";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);

                foreach (string file in files)
                {
                    if (file.EndsWith(".mp3") || file.EndsWith(".wav") || file.EndsWith(".m4a"))
                    {
                        
                        playlist.Add(file); 
                    }
                }

                PlayAudio_OnClick(sender, e);

            }

        }

        private void PlayAudio_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (mediaPlayer.Source == null)
            {
                if (playlist.Count > 0)
                {
                    mediaPlayer.Open(new Uri(playlist[0]));
                    mediaPlayer.Play();
                    timer.Start();
                }
            }
            else
            {
                if (mediaPlayer.Position == mediaPlayer.NaturalDuration.TimeSpan)
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Play();
                    timer.Start();
                }
                else if (mediaPlayer.Position == TimeSpan.Zero || mediaPlayer.Position == mediaPlayer.NaturalDuration.TimeSpan)
                {
                    mediaPlayer.Play();
                    timer.Start();
                }
                else
                {
                    mediaPlayer.Stop();

                }

            }
            
        }

        private void NextTrack_OnClick(object sender, RoutedEventArgs e)
        {
            int currentIndex = playlist.IndexOf(mediaPlayer.Source.LocalPath);
            if (currentIndex < playlist.Count - 1)
            {
                mediaPlayer.Open(new Uri(playlist[currentIndex + 1]));
                mediaPlayer.Play();
            }
        }

        private void PreviousTrack_OnClick(object sender, RoutedEventArgs e)
        {
            int currentIndex = playlist.IndexOf(mediaPlayer.Source.LocalPath);
            if (currentIndex > 0)
            {
                mediaPlayer.Open(new Uri(playlist[currentIndex - 1]));
                mediaPlayer.Play();
            }
        }

        private void RepeatTrack_OnClick(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();
        }



        private void RandomTracks_OnClick(object sender, RoutedEventArgs e)
        {
            int randomIndex = random.Next(playlist.Count);
            mediaPlayer.Open(new Uri(playlist[randomIndex]));
            mediaPlayer.Play();
        }

        private void SliderTrack_OnPreviewMouseUp(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(SliderTrack.Value);
            mediaPlayer.Play();
        }


        private void SliderVolume_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = SliderVolume.Value / 100;
        }

        private void PlayList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playList.SelectedItem != null)
            {
                mediaPlayer.Open(new Uri(playList.SelectedItem.ToString()));
                mediaPlayer.Play();
            }

        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {              
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mediaPlayer.Source.LocalPath);                
                listeningHistory.Add(mediaPlayer.Source.LocalPath);
            }

            int currentIndex = playlist.IndexOf(mediaPlayer.Source.LocalPath);
            if (currentIndex < playlist.Count - 1)
            {
                mediaPlayer.Open(new Uri(playlist[currentIndex + 1]));
                mediaPlayer.Play();
            }

           
        }


    }
}
