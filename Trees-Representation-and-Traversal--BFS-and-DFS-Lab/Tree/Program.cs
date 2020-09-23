using System;
using System.Collections.Generic;
using System.Text;

namespace Tree
{
    class Program
    {
        static void Main(string[] args)
        {
            Test(15); 
        }

        public static void Test(int number)
        {
            if (number == 0)
            {
                return;
            }

            Console.WriteLine(number);
            number--;
            Test(number);
        }
    }
}
