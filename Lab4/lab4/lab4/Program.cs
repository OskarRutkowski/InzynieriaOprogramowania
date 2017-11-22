using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab4
{
    class Program
    {
        class Server
        {
            TcpListener server;
            CancellationTokenSource cts = new CancellationTokenSource();

            Task serverTask;
            public Task ServerTask{ get { return serverTask; }}

            int port;
            public int Port { get { return port; } set { if (!running) port = value; else Console.WriteLine("Zły port"); } }

            IPAddress address;
            public IPAddress Address { get { return address; } set { if (!running) address = value; else Console.WriteLine("Zły adres"); } }

            bool running;
            public bool Running { get { return running; } }

            public Server(int portin, IPAddress adr)
            {
                Address = adr;
                port = portin;
            }
            public Server()
            {
                address = IPAddress.Any;
                port = 2048;
            }
            public Server(int portin)
            {
                port = portin;
            }
            public Server(IPAddress adr)
            {
                address = adr;
            }
            public void Run()
            {
                serverTask = RunAsync();
            }
            public void RequestStop()
            {
                cts.Cancel();
                server.Stop();
            }
            public void Stop()
            {
                RequestStop();
            }
            public async Task RunAsync()
            {
                server = new TcpListener(address, port);
                try
                {
                    server.Start();
                    running = true;
                }
                catch(Exception ex)
                {
                    throw(ex);
                }

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    byte[] buffer = new byte[1024];

                    await client.GetStream().ReadAsync(buffer, 0, buffer.Length).ContinueWith(
                        async (t) =>
                        {
                            int i = t.Result;
                            while (true)
                            {
                                client.GetStream().WriteAsync(buffer, 0, i);
                                i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                            }
                        });
                }
            }
        }
        class Client
        {
            TcpClient client;

            IPAddress clientAddress;
            public IPAddress ClientAddress{ set { clientAddress = value; } get { return clientAddress; } }

            int clientPort;
            public int ClientPort { set { clientPort = value; } get { return clientPort; } }

            public Client()
            {
                clientAddress = IPAddress.Parse("127.0.0.1");
                clientPort = 2048;
            }
            public void Connect()
            {
                client = new TcpClient();
                client.Connect(new IPEndPoint(clientAddress, clientPort));
            }
            public async Task<string> Ping(string message)
            {
                byte[] buffer = new ASCIIEncoding().GetBytes(message);
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                buffer = new byte[1024];
                var t = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                Console.WriteLine("Pingowanie: "+Encoding.UTF8.GetString(buffer, 0, t));
                return Encoding.UTF8.GetString(buffer, 0, t);
            }
            public async Task<IEnumerable<string>> keepPinging(string message, CancellationToken token)
            {
                List<string> messages = new List<string>();
                bool done = false;
                while (!done)
                {
                    if (token.IsCancellationRequested)
                        done = true;
                    messages.Add(await Ping(message));
                }
                Console.WriteLine(messages);
                return messages;
            }
        }

        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();

            Client client1 = new Client();
            Client client2 = new Client();
            Client client3 = new Client();
            Client client4 = new Client();

            client1.Connect();
            client2.Connect();
            client3.Connect();
            client4.Connect();

            client1.Ping("123");
            client2.Ping("abc");
            client3.Ping("rrr");
            client4.Ping("ooo");

            CancellationTokenSource ct1 = new CancellationTokenSource();
            CancellationTokenSource ct2 = new CancellationTokenSource();
            CancellationTokenSource ct3 = new CancellationTokenSource();
            CancellationTokenSource ct4 = new CancellationTokenSource();

            var client1T = client1.keepPinging("wiadomosc", ct1.Token);
            var client2T = client2.keepPinging("123456778", ct2.Token);
            var client3T = client3.keepPinging("rrroooror", ct3.Token);
            var client4T = client4.keepPinging("xxxxxxxx", ct4.Token);
            ct1.CancelAfter(10);
            ct2.CancelAfter(1000);
            ct3.CancelAfter(2000);
            ct4.CancelAfter(100);

            //Thread.Sleep(10000);
            Task.WaitAll(new Task[] { client1T, client2T, client3T, client4T });
            s.Stop();
        }
    }
}
