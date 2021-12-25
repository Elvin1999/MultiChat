using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace MuliChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string Receive;
        public string TextToSend;


        private readonly BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private readonly BackgroundWorker backgroundWorker2 = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker2.DoWork += BackgroundWorker2_DoWork;
            ServerIpTextbox.Text = "10.2.24.36";


        }


        private async void start_Click(object sender, RoutedEventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ServerIpTextbox.Text), int.Parse(ServerPortTextBox.Text));
            listener.Start();


            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;


            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;

        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIpTextbox.Text), int.Parse(ClientPortTextbox.Text));

            try
            {

                ChatScreenTextbox.Text += "Connect To Server\n";
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    Receive = STR.ReadLine();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ChatScreenTextbox.Text += "You : " + Receive + "\n";
                        Receive = "";
                    });
                }
                catch (Exception)
                {

                }
            }
        }
        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                App.Current.Dispatcher.Invoke(() =>
                {
                    ChatScreenTextbox.Text += "Me : " + TextToSend + "\n";

                });
            }
            else
            {
                MessageBox.Show("Sending Failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text != "")
            {
                TextToSend = MessageTextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessageTextBox.Text = "";
        }
    }
}
