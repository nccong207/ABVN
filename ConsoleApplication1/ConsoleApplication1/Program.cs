using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal result = 0.0m;
            decimal posValue = 3.45m;
            decimal negValue = -3.45m;

            // By default, round a positive and a negative value to the nearest even number.  
            // The precision of the result is 1 decimal place.

            result = Math.Round(posValue, 1);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1)", result, posValue);
            result = Math.Round(negValue, 1);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1)", result, negValue);
            Console.WriteLine();

            // Round a positive value to the nearest even number, then to the nearest number away from zero.  
            // The precision of the result is 1 decimal place.

            result = Math.Round(posValue, 1, MidpointRounding.ToEven);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1, MidpointRounding.ToEven)", result, posValue);
            result = Math.Round(posValue, 1, MidpointRounding.AwayFromZero);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1, MidpointRounding.AwayFromZero)", result, posValue);
            Console.WriteLine();

            // Round a negative value to the nearest even number, then to the nearest number away from zero.  
            // The precision of the result is 1 decimal place.

            result = Math.Round(negValue, 1, MidpointRounding.ToEven);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1, MidpointRounding.ToEven)", result, negValue);
            result = Math.Round(negValue, 1, MidpointRounding.AwayFromZero);
            Console.WriteLine("{0,4} = Math.Round({1,5}, 1, MidpointRounding.AwayFromZero)", result, negValue);
            Console.WriteLine();

            decimal a = 34285.5m;
            Console.WriteLine(Math.Round(a, 0, MidpointRounding.AwayFromZero));
            Console.WriteLine(Math.Round(a,0,MidpointRounding.ToEven));
            decimal b = 45454.5m;
            Console.WriteLine(Math.Round(b, 0, MidpointRounding.AwayFromZero));
            Console.WriteLine(Math.Round(b, 0, MidpointRounding.ToEven));

            decimal c = 0.5m;
            Console.WriteLine(Math.Round(c, 0, MidpointRounding.AwayFromZero));
            Console.WriteLine(Math.Round(c, 0, MidpointRounding.ToEven));
        

            Console.ReadLine();
        }
    }
}
