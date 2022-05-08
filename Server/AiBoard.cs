using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AiBoard : Board
{
    protected static sbyte[,] pawn =
    {
    { 0, 0, 0, 0, 0, 0, 0, 0 },
    { 50, 50, 50, 50, 50, 50, 50, 50 },
    { 10, 10, 10, 10, 10, 10, 10, 10 },
    { 10, 10, 20, 30, 30, 20, 10, 10 },
    { 5, 5, 10, 25, 25, 10, 5, 5 },
    { 0, 0, 0, 20, 20, 0, 0, 0 },
    { 5, -5, -10, 0, 0, -10, -5, 5 },
    { 5, 10, 10, -20, -20, 10, 10, 5 },
    { 0, 0, 0, 0, 0, 0, 0, 0 }
    };
    protected static sbyte[,] knight =
    {
    { -50, -40, -30, -30, -30, -30, -40, -50 },
    { 50, 50, 50, 50, 50, 50, 50, 50 },
    { 10, 10, 10, 10, 10, 10, 10, 10 },
    { 10, 10, 20, 30, 30, 20, 10, 10 },
    { 5, 5, 10, 25, 25, 10, 5, 5 },
    { 0, 0, 0, 20, 20, 0, 0, 0 },
    { 5, -5, -10, 0, 0, -10, -5, 5 },
    { 5, 10, 10, -20, -20, 10, 10, 5 },
    { 0, 0, 0, 0, 0, 0, 0, 0 }
    };
    protected static sbyte[,] bishop =
    {
    { -20, -10, -10, -10, -10, -10, -10, -20 },
    { -10, 0, 0, 0, 0, 0, 0, -10 },
    { -10, 0, 5, 10, 10, 5, 0, -10 },
    { -10, 5, 5, 10, 10, 5, 5, -10 },
    { -10, 0, 10, 10, 10, 10, 0, -10 },
    { -10, 10, 10, 10, 10, 10, 10, -10},
    { -10, 5, 0, 0, 0, 0, 5, -10 },
    { -20, -10, -10, -10, -10, -10, -10, -20 }
    };
    protected static sbyte[,] rook =
    {
    { 0, 0, 0, 0, 0, 0, 0, 0},
    { 5, 10, 10, 10, 10, 10, 10, 5},
    { -5, 0, 0, 0, 0, 0, 0, -5 },
    { -5, 0, 0, 0, 0, 0, 0, -5 },
    { -5, 0, 0, 0, 0, 0, 0, -5 },
    { -5, 0, 0, 0, 0, 0, 0, -5 },
    { -5, 0, 0, 0, 0, 0, 0, -5 },
    { 0, 0, 0, 5, 5, 0, 0, 0}
    };
    protected static sbyte[,] queen =
    {
    { -20, -10, -10, -5, -5, -10, -10, -20 },
    { -10, 0, 0, 0, 0, 0, 0, -10 },
    { -10,  0,  5,  5,  5,  5,  0,-10 },
    { -5,  0,  5,  5,  5,  5,  0, -5, },
    { 0,  0,  5,  5,  5,  5,  0, -5, },
    { -10,  5,  5,  5,  5,  5,  0,-10, },
    { -10,  0,  5,  0,  0,  0,  0,-10, },
    { -20,-10,-10, -5, -5,-10,-10,-20 }
    };
    protected static sbyte[,] king =
    {
    { -30,-40,-40,-50,-50,-40,-40,-30 },
    { -30,-40,-40,-50,-50,-40,-40,-30 },
    { -30,-40,-40,-50,-50,-40,-40,-30},
    { -30,-40,-40,-50,-50,-40,-40,-30},
    { -20,-30,-30,-40,-40,-30,-30,-20},
    { -10,-20,-20,-20,-20,-20,-20,-10},
    { 20,20,0,0,0,0,20,20 },
    { 20,30,10,0,0,10,30,20 }
    };

    // king end game
    /*-50,-40,-30,-20,-20,-30,-40,-50,
    -30,-20,-10,  0,  0,-10,-20,-30,
    -30,-10, 20, 30, 30, 20,-10,-30,
    -30,-10, 30, 40, 40, 30,-10,-30,
    -30,-10, 30, 40, 40, 30,-10,-30,
    -30,-10, 20, 30, 30, 20,-10,-30,
    -30,-30,  0,  0,  0,  0,-30,-30,
    -50,-30,-30,-30,-30,-30,-30,-50*/
    public AiBoard() : base() { }
    public AiBoard(string fen) : base(fen) { }
    public Int16 EvaluatePiece(bool whiteToPlay, BasePiece p, int x, int y)
    {
        //returns the value of a piece acording to it's position
        PType type = p.GetPType();
        if (whiteToPlay == !p.GetIsBlack())
            y = (byte)(7 - y);
        switch (type)
        {
            case PType.King: return king[y, x];
            case PType.Queen: return queen[y, x];
            case PType.Rook: return rook[y, x];
            case PType.Bishop: return bishop[y, x];
            case PType.Knight: return knight[y, x];
            case PType.Pawn: return pawn[y, x];
            default: return 0;
        }
    }
    public Int16 EvaluateBoard(bool whiteToPlay)
    {
        //evaluate the points for the board
        Int16 count = 0;
        foreach (BasePiece p in board)
            if (p != null)
                count += EvaluatePiece(whiteToPlay, p, p.GetX(), p.GetY());
        return count;
    }
}
