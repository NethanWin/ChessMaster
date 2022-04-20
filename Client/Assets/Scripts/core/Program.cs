using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        BasePiece BasePiece = new BasePiece(PType.Pawn, true, new Point(2,2));
        Board board = new Board();
        board.AddToBoard(BasePiece);
        //board.AddToBoard(new BasePiece(PType.Pawn, true, new Point(1, 1)));
        Console.WriteLine(board.GetFen());
        Console.WriteLine();
        //BasePiece.UpdateMoves(board);
        //foreach (Move move in BasePiece.GetMoves())
        //  Console.WriteLine(move);

        Board oldBoard = new Board(board.GetFen());

        foreach (Move move in board.GenerateMoves(true))
        {
            Board tempBoard = new Board(oldBoard.GetFen());
            Console.WriteLine(move);
            tempBoard.MakeMove(move);
            Console.WriteLine(tempBoard.GetFen());
        }
    }
}
