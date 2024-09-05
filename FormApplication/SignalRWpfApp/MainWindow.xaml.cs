using System.Windows;
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
            hubConnection.On<TransData>("ReceiveMessage", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    chatMessage.Text += message + Environment.NewLine;
                });
            });

            hubConnection.SendAsync("AddUser", "admin");
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    hubConnection.StartAsync();
                });
            });
        }

        public async void Button_Click(object sender, RoutedEventArgs e)
        {
            await hubConnection.SendAsync("SendToAll", txtInput.Text);
            txtInput.Text = string.Empty;
        }

        public record TransData(string Id, string User, string Message);
    }
}