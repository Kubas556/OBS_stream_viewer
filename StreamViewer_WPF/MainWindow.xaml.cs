using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace StreamViewer_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibVLCSharp.Shared.MediaPlayer _mediaplayer;
        public MainWindow()
        {
            InitializeComponent();
            Core.Initialize();
            this.Focus();

            using var libvlc = new LibVLC(enableDebugLogs: false);
            using var media = new Media(libvlc, new Uri($@"rtp://{LocalIPAddress()}:8081"));
            media.StateChanged += Media_StateChanged;
            _mediaplayer = new LibVLCSharp.Shared.MediaPlayer(media);
            VideoView.MediaPlayer = _mediaplayer;

            _mediaplayer.Play();
        }

        private void Media_StateChanged(object? sender, MediaStateChangedEventArgs e)
        {
            if(e.State == VLCState.Error)
            {
                MessageBox.Show("Cannot play stream!", "Error occurred", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.BeginInvoke(this.Close);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mediaplayer.Dispose();
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        private bool _fullscreen = false;
        private WindowState _oldState;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftAlt) && e.KeyboardDevice.IsKeyDown(Key.Enter)) || (e.KeyboardDevice.IsKeyDown(Key.F12)))
            {
                if(_fullscreen)
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = _oldState;
                } else
                {
                    _oldState = this.WindowState;
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                }


                _fullscreen = !_fullscreen;
            }
        }
    }
}
