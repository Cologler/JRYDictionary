using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Jasily.Windows.Controls;
using JetBrains.Annotations;

namespace JryDictionary.Controls.MediaPlayer
{
    /// <summary>
    /// MediaPlayerControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPlayerControl : INotifyPropertyChanged
    {
        private string displayName;

        public MediaPlayerControl()
        {
            this.InitializeComponent();
            this.ElementHolder = new MediaElementHolder(this.MediaElement);
            this.ElementHolder.StatusChanged += this.StatusCached_StatusChanged;
            this.DataContext = this;
        }

        public MediaElementHolder ElementHolder { get; }

        private void StatusCached_StatusChanged(object sender, MediaState e)
        {
            switch (e)
            {
                case MediaState.Play:
                    this.StopPath.Visibility = Visibility.Visible;
                    this.PlayPath.Visibility = Visibility.Collapsed;
                    break;

                default:
                    this.StopPath.Visibility = Visibility.Collapsed;
                    this.PlayPath.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            switch (this.ElementHolder.CurrentStatus)
            {
                case MediaState.Play:
                    this.ElementHolder.Stop();
                    break;

                default:
                    this.ElementHolder.Play();
                    break;
            }
        }

        public bool IsAutoPlay { get; set; }

        public void SetSource(Uri uri)
        {
            this.MediaElement.Source = uri;
            if (this.IsAutoPlay) this.ElementHolder.Play();
        }

        public string DisplayName
        {
            get { return this.displayName; }
            set
            {
                if (value == this.displayName) return;
                this.displayName = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
