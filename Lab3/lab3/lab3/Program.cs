using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Net;
using System.Xml;
using System.Net.Sockets;

namespace lab3
{
    class Program
    {
        //zadanie1
        public class TResultDataStructure
        {
            private int field1, field2;

            public int Field1 { get => field1; set => field1 = value; }
            public int Field2 { get => field2; set => field2 = value; }
        }
        static public Task<TResultDataStructure> AsynchronMet(byte[] buffer)
        {
            TaskCompletionSource<TResultDataStructure> taskCompletionSource = new TaskCompletionSource<TResultDataStructure>();
            Task.Run(() =>
            {
                taskCompletionSource.SetResult(new TResultDataStructure());
            });
            return taskCompletionSource.Task;
        }
        static public TResultDataStructure Zadanie1()
        {
            var zadanie = AsynchronMet(null);
            zadanie.Wait();
            return zadanie.GetAwaiter().GetResult();
        }
        static void zadanie1()
        {
            TResultDataStructure result = Zadanie1();
            var result2 = result.Field1;
            var result3 = result.Field2;
            Console.WriteLine(result2+'\n'+result3);
            Thread.Sleep(3000);
        }
        //zadanie2
        static void zadanie2()
        {
            bool Z2=false;
            Task.Run(() =>
                {
                    Z2 = true;
                });
        }
        //zadanie3
        static public async Task<string> Zadanie3(string address)
        {
            XmlDocument doc = new XmlDocument();
            WebClient wC = new WebClient();
            string x = await wC.DownloadStringTaskAsync(new Uri(address));
            //doc.Load(x);
            return x;
        }
        static void zadanie3()
        {
            var task = Zadanie3("http://www.feedforall.com/sample.xml");
            string doc;
            Thread.Sleep(3000);
            Console.WriteLine(task.IsCompleted);
            doc = task.GetAwaiter().GetResult();

            
            Console.WriteLine(doc);

            Thread.Sleep(3000);
        }
        
    
    static void Main(string[] args)
        {
            //zadanie1();
            //zadanie2();
            //zadanie3();
            
        }
    }
}
