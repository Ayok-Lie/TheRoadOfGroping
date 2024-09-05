using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly HubConnection hubConnection;

        public MainWindow()
        {
            InitializeComponent();
            hubConnection = new HubConnectionBuilder()
               .WithUrl("https://localhost:7285/testHub")
               .Build();
            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    chatMessage.Text += message + Environment.NewLine;
                });
            });

            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    hubConnection.StartAsync();
                });
            });
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            hubConnection.SendAsync("SendMessage", new string[] { txtInput.Text });

            txtInput.Text = string.Empty;
        }
    }
}