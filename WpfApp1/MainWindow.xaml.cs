using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<FileInfo> files = new ObservableCollection<FileInfo>();
        public string FileName { get; set; }
        public string Url { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            lbOutput.ItemsSource = files;
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbOutput.SelectedItem != null)
                {
                    var selected = lbOutput.SelectedItem as FileInfo;
                    if (selected.IsPause)
                    {
                        (lbOutput.SelectedItem as FileInfo).ResetEvent.Reset();
                    }
                    else
                    {
                        (lbOutput.SelectedItem as FileInfo).ResetEvent.Set();
                    }
                    return;
                }
                MessageBox.Show("Please, select an item!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbOutput.SelectedItem == null)
                {
                    MessageBox.Show("Select item!");
                }
                else
                {
                    (lbOutput.SelectedItem as FileInfo).client.CancelAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            await Handler();
        }


        Task Handler()
        {
            WebClient client = new WebClient();
            FileInfo info = new FileInfo()
            {
                FolderName = tbPath.Text,
                Progress = 0,
                client = client
            };
            client.DownloadProgressChanged += (s, el) => { info.Progress = el.ProgressPercentage; };
            client.DownloadFileAsync(new Uri(tbPath.Text), $"test{new Random().Next(1, 100)}.txt"); // change late
            files.Add(info);
            return Task.CompletedTask;
        }
        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show("Canceled!");
            else
                MessageBox.Show("File downloaded succesfully!");
        }

    }
}
