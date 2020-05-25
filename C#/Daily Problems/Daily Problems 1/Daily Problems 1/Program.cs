using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily_Problems_1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = { 10, 15, 3, 7 };
            int k = 10;
            bool isSuccess = false;

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    if(array[i] + array[j] == k)
                    {
                        isSuccess = true;
                        break;
                    }
                }
            }

            Console.WriteLine(isSuccess.ToString());

            Console.ReadLine();

        }
    }
}
