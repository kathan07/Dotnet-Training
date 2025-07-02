using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello
{
    class Person
    {
        private string _name = "buddy";
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
    class Father : Person
    {
        private string _surname = "Patel";
        private string _religion = "Hindu";
        private string _village = "Vasai";
        public string Surname
        {
            get { return _surname; }
            set { _surname = value; }
        }
        public string Religion
        {
            get { return _religion; }
            set { _religion = value; }
        }
        public string Village
        {
            get { return _village; }
            set { _village = value; }
        }
        public void DisplayFatherProperties()
        {
            Console.WriteLine($"{Name} {_surname} {_religion}  {_village}");
            //Console.WriteLine(Name + " " + _surname + " " + _religion + " " + _village);
        }
    }

    class MathOperations
    {
        private int _a = 0;
        private int _b = 0;

        public (int, int) Variables
        {
            get { return (this._a, this._b); }
            set { this._a = value.Item1; this._b = value.Item2; }
        }

        public int AddSetted()
        {
            return this._a + this._b;
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Person person = new Person();
            Console.WriteLine("Enter Your Name Please :");
            string name = Console.ReadLine();
            person.Name = name;

            MathOperations ops = new MathOperations();
            Console.WriteLine("Enter Value of a");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter Value of b");
            int b;
            int.TryParse(Console.ReadLine(), out b);

            ops.Variables = (a, b); // Corrected assignment

            Console.WriteLine(ops.AddSetted());
            Console.WriteLine("Just an Add Function Nothing Much :");
            Console.WriteLine(ops.Add(20, 12));


            Father father = new Father();
            Console.WriteLine("Enter Your Surname :");
            string surname = Console.ReadLine();
            Console.WriteLine("Enter Your Religion :");
            string religion = Console.ReadLine();
            Console.WriteLine("Enter Your Village Name :");
            string village = Console.ReadLine();
            father.Surname = surname;
            father.Religion = religion;
            father.Village = village;
            father.Name = name;
            father.DisplayFatherProperties();

        }
    }
}
