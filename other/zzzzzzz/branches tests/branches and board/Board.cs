﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Board
{
    private Piece[,] board;
    private List<Piece> blackPieces;
    private List<Piece> whitePieces;
    private int turn;

    public Board()
    {
        board = new Piece[8,8];
        blackPieces = new List<Piece>();
        whitePieces = new List<Piece>();
        turn = 0;
        AddToBoard(new Piece(PType.Rook, false, new Point(0,0)));
        AddToBoard(new Piece(PType.Knight, false, new Point(1,0)));
        AddToBoard(new Piece(PType.Bishop, false, new Point(2,0)));
        AddToBoard(new Piece(PType.King, false, new Point(3,0)));
        AddToBoard(new Piece(PType.Queen, false, new Point(4,0)));
        AddToBoard(new Piece(PType.Bishop, false, new Point(5,0)));
        AddToBoard(new Piece(PType.Knight, false, new Point(6,0)));
        AddToBoard(new Piece(PType.Rook, false, new Point(7,0)));

        AddToBoard(new Piece(PType.Rook, true, new Point(0, 7)));
        AddToBoard(new Piece(PType.Knight, true, new Point(1, 7)));
        AddToBoard(new Piece(PType.Bishop, true, new Point(2, 7)));
        AddToBoard(new Piece(PType.Queen, true, new Point(3, 7)));
        AddToBoard(new Piece(PType.King, true, new Point(4, 7)));
        AddToBoard(new Piece(PType.Bishop, true, new Point(5, 7)));
        AddToBoard(new Piece(PType.Knight, true, new Point(6, 7)));
        AddToBoard(new Piece(PType.Rook, true, new Point(7, 7)));


        for (int i = 0; i < 8; i++)
        {
            AddToBoard(new Piece(PType.Pawn, false, new Point(i, 1)));
            AddToBoard(new Piece(PType.Pawn, true, new Point(i, 6)));
        }
    }
    public Board(string fen)
    {
        board = new Piece[8, 8];
        blackPieces = new List<Piece>();
        whitePieces = new List<Piece>();

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
                AddToBoard(new Piece(pType, isBlack, new Point(x, y)));
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
    public void AddToBoard(Piece piece)
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
    public Piece GetPiece(Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            return board[p.x, p.y];
        return null;
    }
    public void SetPiece(Piece piece, Point p)
    {
        if (p != null && !IsOutsideBoard(p))
            board[p.x, p.y] = piece;
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
                    fen += board[x, y].GetPieceType();
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
        List<Piece> pieces;
        if (isBlack)
            pieces = blackPieces;
        else
            pieces = whitePieces;

        foreach (Piece piece in pieces)
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
            Piece piece = board[move.GetstartPoint().x, move.GetstartPoint().y];
            Piece target = board[move.GetTargetPoint().x, move.GetTargetPoint().y];
            if (target != null)
            {
                if (target.GetIsBlack())
                    blackPieces.Remove(target);
                else
                    whitePieces.Remove(target);
            }
            piece.setPos(move.GetTargetPoint());
            SetPiece(piece, move.GetTargetPoint());
            SetPiece(null, move.GetstartPoint());
        }
        return !IsOutsideBoard(move.GetTargetPoint());
    }
    public List<Board> GetNextGen()
    {
        List<Board> boards = new List<Board>();
        Board tempBoard;

        foreach (Move move in GenerateMoves(true))
        {
            tempBoard = new Board(GetFen());
            tempBoard.MakeMove(move);
            boards.Add(tempBoard);
        }
        return boards;
    }
}
