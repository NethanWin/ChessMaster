using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Board
{
    protected BasePiece[,] board;
    protected List<BasePiece> blackPieces;
    protected List<BasePiece> whitePieces;
    protected int turn;

    public Board()
    {
        board = new BasePiece[8,8];
        blackPieces = new List<BasePiece>();
        whitePieces = new List<BasePiece>();
        turn = 0;
        AddToBoard(new BasePiece(PType.Rook, false, new Point(0,0)));
        AddToBoard(new BasePiece(PType.Knight, false, new Point(1,0)));
        AddToBoard(new BasePiece(PType.Bishop, false, new Point(2,0)));
        AddToBoard(new BasePiece(PType.Queen, false, new Point(3,0)));
        AddToBoard(new BasePiece(PType.King, false, new Point(4,0)));
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
        blackPieces = new List<BasePiece>();
        whitePieces = new List<BasePiece>();

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
                (pType, isBlack) = GetPTypeFromChar(ch);
                AddToBoard(new BasePiece(pType, isBlack, new Point(x, y)));
                x++;
            }
        }
    }
    protected (PType, bool) GetPTypeFromChar(char ch)
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
    public void AddToBoard(BasePiece piece)
    {
        board[piece.GetX(), piece.GetY()] = piece;
        if (piece.GetIsBlack())
            blackPieces.Add(piece);
        else
            whitePieces.Add(piece);
    }
    public bool IsOutsideBoard(Point p)
    {
        return p.x >= board.GetLength(0) || p.x < 0 || p.y >= board.GetLength(1) || p.y < 0;
    }
    public BasePiece GetPiece(Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            return board[(int)p.x, (int)p.y];
        return null;
    }
    public void SetPiece(BasePiece piece, Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            board[(int)p.x, (int)p.y] = piece;
    }
    public bool IsAllyOnPoint(Point targetP, bool isBlack)
    {
        return GetPiece(targetP) != null && GetPiece(targetP).GetIsBlack() == isBlack;
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
        List<BasePiece> pieces;
        if (isBlack)
            pieces = blackPieces;
        else
            pieces = whitePieces;

        foreach (BasePiece piece in pieces)
        {
            //temperey (shoud be done automaticly)
            piece.UpdateMoves(this);
            moves.AddRange(piece.GetMoves());
        }
        return moves;
    }
    public bool MakeMove(Move move)
    {
        if (!IsOutsideBoard(move.GetTargetPoint()))
        {
            BasePiece piece = board[(int)move.GetStartPoint().x, (int)move.GetStartPoint().y];
            BasePiece target = board[(int)move.GetTargetPoint().x, (int)move.GetTargetPoint().y];
            if (target != null)
            {
                if (target.GetIsBlack())
                    blackPieces.Remove(target);
                else
                    whitePieces.Remove(target);
            }
            piece.setPos(move.GetTargetPoint());
            SetPiece(piece, move.GetTargetPoint());
            SetPiece(null, move.GetStartPoint());
        }
        return !IsOutsideBoard(move.GetTargetPoint());
    }
    public bool UndoMove(Move move)
    {
        if (!IsOutsideBoard(move.GetStartPoint()))
        {
            BasePiece piece = board[(int)move.GetTargetPoint().x, (int)move.GetTargetPoint().y];
            BasePiece target = board[(int)move.GetStartPoint().x, (int)move.GetStartPoint().y];
            if (target != null)
            {
                if (target.GetIsBlack())
                    blackPieces.Remove(target);
                else
                    whitePieces.Remove(target);
            }
            piece.setPos(move.GetStartPoint());
            SetPiece(piece, move.GetStartPoint());
            SetPiece(move.GetDestroyedPiece(), move.GetTargetPoint());
        }
        return !IsOutsideBoard(move.GetStartPoint());
    }
    public List<Board> GetNextGenBoards(bool blackToPlay)
    {
        List<Board> boards = new List<Board>();
        Board tempBoard;

        foreach (Move move in GenerateMoves(blackToPlay))
        {
            tempBoard = new Board(GetFen());
            tempBoard.MakeMove(move);
            boards.Add(tempBoard);
        }
        return boards;
    }
}
