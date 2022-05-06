using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Board
{
    private BasePiece[,] board;
    private List<BasePiece> blackBasicPieces;
    private List<BasePiece> whiteBasicPieces;
    private int turn;

    public Board()
    {
        board = new BasePiece[8,8];
        blackBasicPieces = new List<BasePiece>();
        whiteBasicPieces = new List<BasePiece>();
        turn = 0;
        AddToBoard(new BasePiece(PType.Rook, false, new Point(0,0)));
        AddToBoard(new BasePiece(PType.Knight, false, new Point(1,0)));
        AddToBoard(new BasePiece(PType.Bishop, false, new Point(2,0)));
        AddToBoard(new BasePiece(PType.King, false, new Point(3,0)));
        AddToBoard(new BasePiece(PType.Queen, false, new Point(4,0)));
        AddToBoard(new BasePiece(PType.Bishop, false, new Point(5,0)));
        AddToBoard(new BasePiece(PType.Knight, false, new Point(6,0)));
        AddToBoard(new BasePiece(PType.Rook, false, new Point(7,0)));

        AddToBoard(new BasePiece(PType.Rook, true, new Point(0, 7)));
        AddToBoard(new BasePiece(PType.Knight, true, new Point(1, 7)));
        AddToBoard(new BasePiece(PType.Bishop, true, new Point(2, 7)));
        AddToBoard(new BasePiece(PType.Queen, true, new Point(3, 7)));
        AddToBoard(new BasePiece(PType.King, true, new Point(4, 7)));
        AddToBoard(new BasePiece(PType.Bishop, true, new Point(5, 7)));
        AddToBoard(new BasePiece(PType.Knight, true, new Point(6, 7)));
        AddToBoard(new BasePiece(PType.Rook, true, new Point(7, 7)));


        for (int i = 0; i < 8; i++)
        {
            AddToBoard(new BasePiece(PType.Pawn, false, new Point(i, 1)));
            AddToBoard(new BasePiece(PType.Pawn, true, new Point(i, 6)));
        }
    }
    public Board(string fen)
    {
        board = new BasePiece[8, 8];
        blackBasicPieces = new List<BasePiece>();
        whiteBasicPieces = new List<BasePiece>();



        int y = board.GetLength(0) - 1;
        int x = 0;
        foreach (char ch in fen)
        {
            if (ch == '/')
            {
                y--;
                x = 0;
            }
            else if (ch - '0' >= 0 && ch - '0' < 9)
            {
                x += ch - '0';
            }
            else
            {
                PType pType; bool isBlack;
                (pType,isBlack) = GetPTypeFromChar(ch);
                AddToBoard(new BasePiece(pType, isBlack, new Point(x, y)));
                x++;
            }
        }
    }
    private (PType, bool) GetPTypeFromChar(char ch)
    {
        //(PType, isBlack)
        bool isBlack = true;
        if (ch <= 'Z' && ch >= 'A')
        {
            isBlack = false;
            ch = (char)(ch - 'A' + 'a');
        }
        switch (ch)
        {
            case 'k':
                return (PType.King, isBlack);
            case 'q':
                return (PType.Queen, isBlack);
            case 'r':
                return (PType.Rook, isBlack);
            case 'b':
                return (PType.Bishop, isBlack);
            case 'n':
                return (PType.Knight, isBlack);
            case 'p':
                return (PType.Pawn, isBlack);
            default:
                return (PType.Pawn, isBlack);
        }
    }
    public void AddToBoard(BasePiece BasicPiece)
    {
        board[BasicPiece.GetX(), BasicPiece.GetY()] = BasicPiece;
        if (BasicPiece.GetIsBlack())
            blackBasicPieces.Add(BasicPiece);
        else
            whiteBasicPieces.Add(BasicPiece);
    }
    public bool IsOutsideBoard(Point p)
    {
        return p.x >= board.GetLength(0) || p.x < 0 || p.y >= board.GetLength(1) || p.y < 0;
    }
    public BasePiece GetBasicPiece(Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            return board[(int)p.x, (int)p.y];
        return null;
    }
    public void SetBasicPiece(BasePiece BasicPiece, Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            board[(int)p.x, (int)p.y] = BasicPiece;
    }
    public bool IsAllyOnPoint(Point targetP, bool isBlack)
    {
        return GetBasicPiece(targetP) != null && GetBasicPiece(targetP).GetIsBlack() == isBlack;
    }
    public string GetFen()
    {
        string fen = "";
        for (int y = board.GetLength(0) - 1; y >= 0; y--)
        {
            int countEmptySpace = 0;
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[x, y] != null)
                {
                    if (countEmptySpace > 0)
                    {
                        fen += countEmptySpace;
                        countEmptySpace = 0;
                    }
                    fen += board[x, y].GetPieceChar();
                }
                else
                    countEmptySpace++;
            }
            if (countEmptySpace > 0)
                fen += countEmptySpace;
            if (y > 0)
                fen += '/';
        }
        return fen;
    }
    public List<Move> GenerateMoves(bool isBlack)
    {
        List<Move> moves = new List<Move>();
        List<BasePiece> BasicPieces;
        if (isBlack)
            BasicPieces = blackBasicPieces;
        else
            BasicPieces = whiteBasicPieces;

        foreach (BasePiece BasicPiece in BasicPieces)
        {
            //temperey (shoud be done automaticly)
            BasicPiece.UpdateMoves(this);

            moves.AddRange(BasicPiece.GetMoves());
        }
        return moves;
    }
    
    public bool MakeMove(Move move)
    {
        if (!IsOutsideBoard(move.GetTargetPoint()))
        {
            BasePiece BasicPiece = board[(int)move.GetStartPoint().x, (int)move.GetStartPoint().y];
            BasePiece target = board[(int)move.GetTargetPoint().x, (int)move.GetTargetPoint().y];
            if (target != null)
            {
                if (target.GetIsBlack())
                    blackBasicPieces.Remove(target);
                else
                    whiteBasicPieces.Remove(target);
            }
            BasicPiece.setPos(move.GetTargetPoint());
            SetBasicPiece(BasicPiece, move.GetTargetPoint());
            SetBasicPiece(null, move.GetStartPoint());
        }
        return !IsOutsideBoard(move.GetTargetPoint());
    }
}
