using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMoreTestProjects {
    class Program {
        static void Main(string[] args) {
            again:
            try {
                var y = double.Parse(Console.ReadLine());
                var x = double.Parse(Console.ReadLine());
                var r = Math.Atan(y / x);
                Console.WriteLine(r );
                Console.WriteLine("again");
                Console.ReadLine();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            goto again;
        }
    }
}
