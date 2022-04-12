using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Chess
{
    /*//king = 1, queen = 2, rook = 3, bishop = 4, knight = 5, pawn = 6
    private enum PieceType { King = 1, Queen, Rook, Bishop, Knight, Pawn }
    public static List<Branch> GetNextGen(Branch b)
    {
        //temperery
        if (b.board == "123")
        {
            List<Branch> list = new List<Branch>();
            for (int i = 0; i < 3; i++)
            {
                list.Add(new Branch(i.ToString(), !b.isMax));
            }
            return list;
        }
        return null; 
    }
    public List<Move> GetMoves(byte[,] board, byte pieceType)
    {
        /*switch (pieceType)
        {
            case (byte)PieceType.King: { return GetMoves_King(board); }
            case (byte)PieceType.Queen: { return GetMoves_Queen(board); }
            case (byte)PieceType.Rook: { return GetMoves_Rook(board); }
            case (byte)PieceType.Bishop: { return GetMoves_Bishop(board); }
            case (byte)PieceType.Knight: { return GetMoves_Knight(board); }
            case (byte)PieceType.Pawn: { return GetMoves_Pawn(board); }
        }
        return null;
    }

    private List<Move> GetMoves_King(byte[,] board)
    {/*
        //get all king's moves
        Point oldLoc = GetLocation(board);
        List<Move> validMoves = new List<Move>();
        foreach (Point offset in king_offsets)
        {
            Point newLoc = new Point(oldLoc.x + offset.x, oldLoc.y + offset.y);
            TileType moveType = GetTileType(board, newLoc);

            if (moveType != TileTypes.blocked)
            {
                validMoves.Add(new Move(newLoc, moveType.standard));
            }
        }
        return null;
    }
    public static int Score(string board)  //Todo
    {
        int count = -1;
        foreach (char letter in board)
        {
            if (letter == '1')
                count++;
        }
        return count;
    }*/
}
