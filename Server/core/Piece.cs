using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PType { King, Queen, Rook, Bishop, Knight, Pawn };

class Piece
{
    private Point currentPos;
    private List<Move> moves;
    private PType type;

    private bool isBlack;
    private bool isFirstMove;

    public Piece(PType type, bool isBlack, Point initPoint) 
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
        moves.Add(GetMoveReletive(0, 1));
        moves.Add(GetMoveReletive(1, 0));
        moves.Add(GetMoveReletive(0, -1));
        moves.Add(GetMoveReletive(-1, 0));
        moves.Add(GetMoveReletive(1, 1));
        moves.Add(GetMoveReletive(-1, -1));
        moves.Add(GetMoveReletive(1, -1));
        moves.Add(GetMoveReletive(-1, 1));
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
        moves.Add(GetMoveReletive(-1, 2));
        moves.Add(GetMoveReletive(1, -2));
        moves.Add(GetMoveReletive(1, 2));
        moves.Add(GetMoveReletive(-1, -2));

        moves.Add(GetMoveReletive(2, -1));
        moves.Add(GetMoveReletive(-2, 1));
        moves.Add(GetMoveReletive(2, 1));
        moves.Add(GetMoveReletive(-2, -1));
    }
    private void UpdateMoves_Pawn(Board board)
    {
        if (!isBlack)
        {
            moves.Add(GetMoveReletive(0, 1));
            if (isFirstMove)
                moves.Add(GetMoveReletive(0, 2));
        }
        else
        {
            moves.Add(GetMoveReletive(0, -1));
            if (isFirstMove)
                moves.Add(GetMoveReletive(0, -2));
        }
    }
    private Move GetMoveReletive(sbyte x, sbyte y)
    {
        //movement reletive to the currentPos
        return GetMove((byte)(currentPos.x + x), (byte)(currentPos.y + y));
    }
    private Move GetMove(byte x, byte y)
    {
        return new Move(currentPos, new Point(x, y));
    }
    private void UpdateMoveLine(Board board, sbyte dx, sbyte dy)
    {
        //adds raw moves for a line (dx and dy is the direction of the line)
        byte x = (byte)(currentPos.x + dx);
        byte y = (byte)(currentPos.y + dy);
        for (;!board.IsOutsideBoard(new Point(x, y)); x = (byte)(dx + x), y = (byte)(dy + y))
        {
            moves.Add(GetMove(x, y));
            if (board.IsAllyOnPoint(new Point(x, y), isBlack))
                break;
        }
    }
    public byte GetX() => currentPos.x;
    public byte GetY() => currentPos.y;
    public bool GetIsBlack() => isBlack;
    public char GetPieceChar()
    {
        if (isBlack)
            return GetBaseType();
        return (char)(GetBaseType() + 'A' - 'a');
    }
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