using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader
{
    public class FileInfo : INotifyPropertyChanged
    {
        public WebClient client { get; set; } // for cancel in listbox
        public ManualResetEvent ResetEvent { get; set; } = new ManualResetEvent(true);
        public string FileName { get; set; }
        public string FolderName { get; set; }
        private double progress;


        private double maxProgress;
        public double MaxProgress
        {
            get { return maxProgress; }
            set
            {
                maxProgress = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }
        public bool IsPause { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
