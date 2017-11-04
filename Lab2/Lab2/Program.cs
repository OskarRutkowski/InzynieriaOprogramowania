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

            
            autoResetEvent[0].Set();
        }
        static AutoResetEvent[] autoResetEvent;
        static void zadanie1()
        {
            string path = "test.txt";

            FileStream fs = new FileStream(path, FileMode.Open);

            byte[] b = new byte[1024];

            fs.BeginRead(b, 0, b.Length, myAsyncCallback, new object[] { fs, b });

            autoResetEvent = new AutoResetEvent[1];
            autoResetEvent[0] = new AutoResetEvent(false);

            WaitHandle.WaitAll(autoResetEvent);

            Thread.Sleep(3000);
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
        static void Main(string[] args)
        {
            delegatename = new DelegateType(silniaIt);
            int result = delegatename.Invoke(8);
            delegatename = new DelegateType(silniaRe);
            int result2 = delegatename.Invoke(5);
            Console.WriteLine("Silnia1: "+result);
            Console.WriteLine("Silnia2: "+result2);
            Console.ReadKey();
        }
    }
}
