using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        Board board = new Board();
        Console.WriteLine(board.GetFen());
        
        foreach (Board b in board.GetNextGen())
        {
            Console.WriteLine(b.GetFen());
        }
    }
}
