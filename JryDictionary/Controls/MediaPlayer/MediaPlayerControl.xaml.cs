using System;
using System.Windows;
using System.Windows.Controls;
using Jasily.Windows.Controls;

namespace JryDictionary.Controls.MediaPlayer
{
    /// <summary>
    /// MediaPlayerControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPlayerControl
    {
        private readonly MediaElementHolder mediaElementHolder;

        public MediaPlayerControl()
        {
            this.InitializeComponent();
            this.mediaElementHolder = new MediaElementHolder(this.MediaElement);
            this.mediaElementHolder.StatusChanged += this.StatusCached_StatusChanged;
        }

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
            switch (this.mediaElementHolder.CurrentStatus)
            {
                case MediaState.Play:
                    this.mediaElementHolder.Stop();
                    break;

                default:
                    this.mediaElementHolder.Play();
                    break;
            }
        }

        public bool IsAutoPlay { get; set; }

        public MediaPlayerControl SetSource(Uri uri, string name)
        {
            this.MediaElement.Source = uri;
            if (this.IsAutoPlay) this.mediaElementHolder.Play();
            return this;
        }
    }
}
