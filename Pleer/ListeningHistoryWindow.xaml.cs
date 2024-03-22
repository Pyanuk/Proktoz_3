using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Pleer
{
    public partial class ListeningHistoryWindow : Window
    {
        public ListeningHistoryWindow(List<string> listeningHistory)
        {
            InitializeComponent();
            HistoryListBox.ItemsSource = listeningHistory; 
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}