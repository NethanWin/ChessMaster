using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp10
{
    class Program
    {
        public static int MinMax(Branch b, int depth)
        {
            if (depth == 0 || Branch.IsEmptyList(b.next))
                return Chess.Score(b.board);
            if (b.isMax)
            {
                int value = int.MinValue;
                foreach (Branch child in b.next)
                {
                    Console.WriteLine(MinMax(child, depth - 1));
                    value = Math.Max(value, MinMax(child, depth - 1));
                }
                return value;
            }
            else
            {
                int value = int.MaxValue;
                foreach (Branch child in b.next)
                    value = Math.Min(value, MinMax(child, depth - 1));
                return value;
            }
        }
        static void Main(string[] args)
        {
            Branch mainBranch = new Branch("123");

            Console.WriteLine(mainBranch);
            Console.WriteLine(MinMax(mainBranch, 1));
            
        }
    }
}
