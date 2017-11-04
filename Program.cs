using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Lab1
{
    class Program
    {
        static void taskOne(Object stateInfo)
        {
            
            var time1 = ((object[])stateInfo)[0];
            var time = Convert.ToInt32(time1);
           
            Thread.Sleep(time);
            Console.WriteLine("Waiting time: "+time);
        }
        static void taskTwo(Object stateInfo)
        {
            
            var time1 = ((object[])stateInfo)[0];
            var time = Convert.ToInt32(time1);

            Console.WriteLine("Waiting time: " + time);
            
        }
        static void zadanie1()
        {
            ThreadPool.QueueUserWorkItem(taskOne, new object[] { 6000 });
            ThreadPool.QueueUserWorkItem(taskTwo, new object[] { 4000 });
            Thread.Sleep(10000);
        }
        static void Server(Object stateInfo)
        {
            Object thisLock = new Object();
            TcpListener server = new TcpListener(IPAddress.Any, 2048);
            server.Start();
            while (true)
            {
                //+zad4
                lock(thisLock)
                {
                    TcpClient client = server.AcceptTcpClient();
                    //+zad3
                    ThreadPool.QueueUserWorkItem(ClientHandler, client);
                }
                
            }
        }
        static void Client(Object stateInfo)
        {
            var number1 = ((object[])stateInfo)[0];
            var number = Convert.ToInt32(number1);

            TcpClient client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048));
            byte[] message = new ASCIIEncoding().GetBytes("client"+number+"'s message");
            client.GetStream().Write(message, 0, message.Length);

            byte[] incomeBuffer = new byte[1024];
            client.GetStream().Read(incomeBuffer, 0, 1024);
            writeConsoleMessage(new ASCIIEncoding().GetString(incomeBuffer), ConsoleColor.Red);
        }
        static void ClientHandler(Object clientObject)
        {
            TcpClient client = clientObject as TcpClient;
            byte[] buffer = new byte[1024];
            client.GetStream().Read(buffer, 0, 1024);
            //+zad3
            writeConsoleMessage(new ASCIIEncoding().GetString(buffer), ConsoleColor.Green);
            client.GetStream().Write(buffer, 0, buffer.Length);
            Thread.Sleep(1000);
            client.Close();
        }
        static void zadanie2()
        {
            ThreadPool.QueueUserWorkItem(Server);
            ThreadPool.QueueUserWorkItem(Client, new object[] { 1 });
            ThreadPool.QueueUserWorkItem(Client, new object[] { 2 });
            Thread.Sleep(7000);
        }
        static void writeConsoleMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("Otrzymałem wiadomość: "+message);
            Console.ResetColor();
        }
        static void zadanie3()
        {
            zadanie2();
            //Problemem jest brak przewidywalności, kiedy server odpowie, któremu klientowi
        }
        static void zadanie4()
        {
            //Polaczenie serwer-klient zostaje wykonane jedno po drugim (wraz z wyslaniem wiadomosci powitalnej)
            //Zamek czeka, az polaczenie zostanie calkowicie wykonane
            zadanie3();
        }
        static void Sum(Object stateInfo)
        {
            var table = ((object[])stateInfo)[0];
            
        }
        static void zadanie5(int length,int piece)
        {
            List<int> integers = new List<int>(length);
            Random rand = new Random();
            for(int x=0;x<length;x++ )
            {
                integers.Add(rand.Next(1, 99));
            }
            ThreadPool.QueueUserWorkItem( Sum,new object[] { integers,  piece });
        }
        static void Main(string[] args)
        {
            zadanie5(6,3);
            Console.ReadKey();
                
        }
            
        
    
    }
}

