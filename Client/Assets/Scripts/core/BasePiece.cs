﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PType { King, Queen, Rook, Bishop, Knight, Pawn };

public class BasePiece
{
    private Point currentPos;
    private List<Move> moves;
    private PType type;

    private bool isBlack;
    private bool isFirstMove;

    public BasePiece(PType type, bool isBlack, Point initPoint) 
    {
        moves = new List<Move>();
        this.currentPos = initPoint;
        this.type = type;
        this.isBlack = isBlack;
        isFirstMove = true;
    }

    public void UpdateMoves(Board board)
    {
        moves.Clear();
        switch (type)
        {
            case PType.King:
                UpdateMoves_King(board); break;
            case PType.Queen: 
                UpdateMoves_Queen(board); break;
            case PType.Rook:
                UpdateMoves_Rook(board); break;
            case PType.Bishop:
                UpdateMoves_Bishop(board); break;
            case PType.Knight:
                UpdateMoves_Knight(board); break;
            case PType.Pawn:
                UpdateMoves_Pawn(board); break;
        }
        FilterMoves(board);
    }
    public void FilterMoves(Board board)
    {
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            if (board.IsOutsideBoard(moves[i].GetTargetPoint()))
                moves.RemoveAt(i);
            else if (board.IsAllyOnPoint(moves[i].GetTargetPoint(), isBlack))
                moves.RemoveAt(i);
        }
    }
    private void UpdateMoves_King(Board board)
    {
        moves.Add(GetMoveRelative(0, 1, board));
        moves.Add(GetMoveRelative(1, 0, board));
        moves.Add(GetMoveRelative(0, -1, board));
        moves.Add(GetMoveRelative(-1, 0, board));
        moves.Add(GetMoveRelative(1, 1, board));
        moves.Add(GetMoveRelative(-1, -1, board));
        moves.Add(GetMoveRelative(1, -1, board));
        moves.Add(GetMoveRelative(-1, 1, board));
    }
    private void UpdateMoves_Queen(Board board)
    {
        UpdateMoves_Rook(board);
        UpdateMoves_Bishop(board);
    }
    private void UpdateMoves_Rook(Board board)
    {
        UpdateMoveLine(board, 1, 0);
        UpdateMoveLine(board, 0, 1);
        UpdateMoveLine(board, -1, 0);
        UpdateMoveLine(board, 0, -1);
    }
    private void UpdateMoves_Bishop(Board board)
    {
        UpdateMoveLine(board, 1, 1);
        UpdateMoveLine(board, -1, -1);
        UpdateMoveLine(board, 1, -1);
        UpdateMoveLine(board, -1, 1);
    }
    private void UpdateMoves_Knight(Board board)
    {
        moves.Add(GetMoveRelative(-1, 2, board));
        moves.Add(GetMoveRelative(1, -2, board));
        moves.Add(GetMoveRelative(1, 2, board));
        moves.Add(GetMoveRelative(-1, -2, board));

        moves.Add(GetMoveRelative(2, -1, board));
        moves.Add(GetMoveRelative(-2, 1, board));
        moves.Add(GetMoveRelative(2, 1, board));
        moves.Add(GetMoveRelative(-2, -1, board));
    }
    private void UpdateMoves_Pawn(Board board)
    {
        int mul = isBlack ? -1 : 1;
        BasePiece tempPiece = board.GetPiece(new Point(currentPos.x, currentPos.y + (sbyte)mul));
        
        if (tempPiece == null)
            moves.Add(GetMoveRelative(0, (sbyte)mul, board));
        if (currentPos.y == 1)
            moves.Add(GetMoveRelative(0, (sbyte)(2 * mul), board));

        //runs twice for each side it can attack
        for (int targetX = -1; targetX < 2; targetX += 2)
        {
            Point targetPos = new Point(currentPos.x + targetX, currentPos.y + mul);
            if (!board.IsOutsideBoard(targetPos))
            {
                BasePiece basePiece = board.GetPiece(targetPos);
                if (basePiece != null && basePiece.isBlack != isBlack)
                    moves.Add(GetMoveRelative((sbyte)targetX, (sbyte)mul, board));
            }
        }
    }
    private Move GetMoveRelative(sbyte x, sbyte y, Board board)
    {
        //movement reletive to the currentPos
        return GetMove((byte)(currentPos.x + x), (byte)(currentPos.y + y), board);
    }
    private Move GetMove(byte x, byte y, Board board)
    {
        Point targetPos = new Point(x, y);
        return new Move(currentPos, targetPos, board.GetPiece(targetPos));
    }
    private void UpdateMoveLine(Board board, sbyte dx, sbyte dy)
    {
        //Adds raw moves for a line (dx and dy is the direction of the line)
        byte x = (byte)(currentPos.x + dx);
        byte y = (byte)(currentPos.y + dy);
        for (;!board.IsOutsideBoard(new Point(x, y)); x = (byte)(dx + x), y = (byte)(dy + y))
        {
            moves.Add(GetMove(x, y, board));
            if (board.GetPiece(new Point(x, y)) != null && board.GetPiece(new Point(x, y)).isBlack != isBlack)
                break;
            if (board.IsAllyOnPoint(new Point(x, y), isBlack))
                break;
        }
    }
    public int GetX() => (int)currentPos.x;
    public int GetY() => (int)currentPos.y;
    public bool GetIsBlack() => isBlack;
    public char GetPieceChar() => isBlack ? GetBaseType() : (char)(GetBaseType() + 'A' - 'a');
    public PType GetPType() => type;
    private char GetBaseType()
    {
        switch(type)
        {
            case PType.King:
                return 'k';
            case PType.Queen:
                return 'q';
            case PType.Rook:
                return 'r';
            case PType.Bishop:
                return 'b';
            case PType.Knight:
                return 'n';
            case PType.Pawn:
                return 'p';
            default:
                return ' ';
        }
    }
    //temp method
    public List<Move> GetMoves() => moves;
    public void setPos(Point newPos)
    {
        currentPos = new Point(newPos.x, newPos.y);
        isFirstMove = false;
    }
}
