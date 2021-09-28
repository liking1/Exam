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
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<FileInfo> files = new ObservableCollection<FileInfo>();
        public string FileName { get; set; }
        public string Url { get; set; }
        int number = 0;

        public MainWindow()
        {
            InitializeComponent();
            number = new Random().Next(1, 100);
            lbOutput.ItemsSource = files;
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnCancel.IsEnabled == false)
                {
                    btnPause.Content = "Pause";
                    (sender as Button).Background = new SolidColorBrush(Colors.Gray);
                }
                if (lbOutput.SelectedItem != null)
                {
                    var selected = lbOutput.SelectedItem as FileInfo;
                    if (selected.IsPause)
                    {
                        (lbOutput.SelectedItem as FileInfo).ResetEvent.Set();
                        selected.IsPause = false;
                    }
                    else
                    {
                        (lbOutput.SelectedItem as FileInfo).ResetEvent.Reset();
                        selected.IsPause = true;
                    }
                    if (btnPause.Content.ToString() == "Pause")
                    {
                        btnPause.Content = "Resume";
                        (sender as Button).Background = new SolidColorBrush(Colors.LightGreen);
                    }
                    else
                    {
                        btnPause.Content = "Pause";
                        (sender as Button).Background = new SolidColorBrush(Colors.Gray);
                    }
                }
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
                    btnPause.Content = "Pause";
                    btnPause.Background = new SolidColorBrush(Colors.Gray);
                    btnPause.IsEnabled = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (tbPath.Text == string.Empty || tbTo.Text == string.Empty)
            {
                return;
            }
            WebClient client = new WebClient();
            FileInfo info = new FileInfo()
            {
                FileName = tbPath.Text,
                FolderName = tbTo.Text,
                Progress = 0,
                client = client
            };
            client.DownloadProgressChanged += (s, el) => { info.Progress = el.ProgressPercentage; };
            DownloadWithStream(info);
            files.Add(info);
            btnPause.IsEnabled = true;
        }
        private string GetFileExtension1(string fileName)
        {
            string extension = "";
            char[] arr = fileName.ToCharArray();
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '.'
            )
                {
                    index = i;
                }
            }
            for (int x = index + 1; x < arr.Length; x++)
            {
                extension = extension + arr[x];
            }
            return extension;
        }
        public Task DownloadWithStream(FileInfo info)
        {
            return Task.Run(() =>
            {
                using (Stream stream = info.client.OpenRead(info.FileName))
                {
                    info.MaxProgress = Convert.ToInt64(info.client.ResponseHeaders["Content-Length"]) / 1024 / 1024;
            
                    using (FileStream fs = File.Create($@"{info.FolderName}/{Path.GetFileName(info.FileName)}"))
                    {
                        int len = 0;
                        do
                        {
                            info.ResetEvent.WaitOne();
                            byte[] buff = new byte[1024];
                            len = stream.Read(buff, 0, buff.Length);
                            fs.Write(buff, 0, len);
                            info.Progress += len / (double)1024 / (double)1024;
                        }
                        while (len > 0);
                    }
                }
            });
            //using (Stream stream = info.client.OpenRead(info.FileName))
            //{
            //    info.MaxProgress = Convert.ToInt64(info.client.ResponseHeaders["Content-Length"]) / 1024 / 1024;
            //
            //    using (FileStream fs = File.Create($@"{info.FolderName}/{Path.GetFileName(info.FileName)}"))
            //    {
            //        int len = 0;
            //        do
            //        {
            //            byte[] buff = new byte[1024];
            //            len = stream.Read(buff, 0, buff.Length);
            //            fs.Write(buff, 0, len);
            //            info.Progress += len / (double)1024 / (double)1024;
            //        }
            //        while (len > 0);
            //    }
            //}
            //long currentPosition = File.Exists(info.FileName) ? new FileInfo(info.FolderName): 0;
        }

        private string GetFileExtension(string fileName)
        {
            string extension = "";
            char[] arr = fileName.ToCharArray();
            int index = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '.'
            )
                {
                    index = i;
                }
            }
            for (int x = index + 1; x < arr.Length; x++)
            {
                extension = extension + arr[x];
            }
            return extension;
        }

        //Task Handler()
        //{
        //    WebClient client = new WebClient();
        //    FileInfo info = new FileInfo()
        //    {
        //        FileName = tbPath.Text,
        //        FolderName = tbTo.Text,
        //        Progress = 0,
        //        client = client
        //    };
        //    client.DownloadProgressChanged += (s, el) => { info.Progress = el.ProgressPercentage; };
        //    client.DownloadFileAsync(
        //       new Uri(tbPath.Text),
        //       tbTo.Text);
        //    //Console.WriteLine("File loaded");
        //    files.Add(info);
        //    //client.DownloadProgressChanged += (s, el) => { info.Progress = el.ProgressPercentage; };
        //    //client.DownloadFileAsync(new Uri(tbPath.Text), tbTo.Text/*$"File{number}." + GetFileExtension(tbPath.Text)*/);
        //    //files.Add(info);
        //    //return Task.CompletedTask;
        //}

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show("Canceled!");
            else
                MessageBox.Show("File has been downloaded succesfully!");
        }
        private void openFiles_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                tbTo.Text = dialog.FileName;
            }
        }
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            var filePickerDialog = new OpenFileDialog();
            if (filePickerDialog.ShowDialog() == true)
            {
                string fileToDelete = filePickerDialog.FileName;
                File.Delete(fileToDelete);
                if (!File.Exists(fileToDelete))
                    files.Remove((FileInfo)lbOutput.SelectedItem);
            }
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
                Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            files.Clear();
        }
    }
}
