using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Threading;

namespace Lab2
{
    class Program
    {
        static void myAsyncCallback(IAsyncResult state)
        {
            var incoming = state.AsyncState;
            byte[] message = (byte[])((object [])incoming)[1];
            ASCIIEncoding nazwa = new ASCIIEncoding();

            Console.WriteLine(nazwa.GetString(message));

            FileStream newFS = (FileStream)((object[])incoming)[1];
           
            autoResetEvent[0].Set();
            newFS.Close();
        }
        static AutoResetEvent[] autoResetEvent;
        static void zadanie6()
        {
            string path = "test.txt";

            FileStream fs = new FileStream(path, FileMode.Open);

            byte[] b = new byte[1024];

            fs.BeginRead(b, 0, b.Length, myAsyncCallback, new object[] { fs, b });

            autoResetEvent = new AutoResetEvent[1];
            autoResetEvent[0] = new AutoResetEvent(false);

            WaitHandle.WaitAll(autoResetEvent);
            //watek glowny czeka na zakonczenie operacji czytania
            //watek glowny nie czeka na callback
            Thread.Sleep(3000);
        }
        static void zadanie7()
        {
            string path = "test.txt";

            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] b = new byte[1024];
            var result = fs.BeginRead(b, 0, b.Length, null, null);
            fs.EndRead(result);
            ASCIIEncoding nazwa = new ASCIIEncoding();
            Console.WriteLine(nazwa.GetString(b));
            fs.Close();
            Thread.Sleep(4000);
        }
        delegate int DelegateType(object arguments);
        static DelegateType delegatename;
        static int silniaIt(object args)
        {
            var number = (int)args;
            int result = 1;
            for (int i = 1; i <= number; i++)
            {
                result *= i;
            }
            return result;
        }
        static int silniaRe(object args)
        {
            var num = (int)args;
            if (num < 1)
                return 1;
            else
                return num * silniaRe(num - 1);
        }
        static int fibRe(object args)
        {
            var n = (int)args;
            if ((n == 1) || (n == 2))
                return 1;
            else
                return fibRe(n - 1) + fibRe(n - 2);
        }
        static void zadanie8()
        {
            delegatename = new DelegateType(silniaIt);
            IAsyncResult ar = delegatename.BeginInvoke(20, null, null);
            int result = delegatename.EndInvoke(ar);

            delegatename = new DelegateType(silniaRe);
            IAsyncResult ar2 = delegatename.BeginInvoke(8, null, null);
            int result2 = delegatename.EndInvoke(ar2);

            delegatename = new DelegateType(fibRe);
            IAsyncResult ar3 = delegatename.BeginInvoke(8, null, null);
            int result3 = delegatename.EndInvoke(ar3);

            Console.WriteLine("Silnia1: " + result);
            Console.WriteLine("Silnia2: " + result2);
            Console.WriteLine("Fibbonaci: " + result3);
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            //zadanie6();
            zadanie7();
            //zadanie8();
            /**/
        }
    }
}
