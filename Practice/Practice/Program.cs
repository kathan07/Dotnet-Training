using System.Linq;
using System;
using System.Collections.Generic;

namespace Practice
{
    internal class Program
    {

        static void Main(string[] args)
        {
            int a = 5;
            int b = 7;
            a  ^= b;
            b ^= a;
            a ^= b;
            Console.WriteLine($"{a} and {b}");            
        }
    }
}
