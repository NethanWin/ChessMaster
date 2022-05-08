using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Ai
{
    public static int depth;
    public static Move GetBestMoveOne(AiBoard board, bool isWhite)
    {
        List<Move> possibleMoves = board.GenerateMoves(isWhite);
        Move bestMove = null;
        double BestMoveValue = double.MinValue;

        foreach (Move move in possibleMoves)
        {
            board.MakeMove(move);
            double moveValue = board.EvaluateBoard(isWhite);
            Console.WriteLine(moveValue);
            if (moveValue > BestMoveValue)
            {
                bestMove = move;
                BestMoveValue = moveValue;
            }
            board.UndoMove(move);
        }
        Console.WriteLine(BestMoveValue);
        return bestMove;
    }
    public static Move GetBestMove(AiBoard board)
    {
        Ai.depth = 3;
        return GetBestMove(Ai.depth, board, true, true).Item2;
    }
    private static (double, Move) GetBestMove(int depth, AiBoard board,
                                bool isWhite, bool isMaximizingPlayer)
    {
        if (depth == 0)
        {
            return (board.EvaluateBoard(isWhite), null);
        }

        Move bestMove = null;
        List<Move> possibleMoves = board.GenerateMoves(isWhite);
        double bestMoveValue = isMaximizingPlayer ? double.MinValue
                                                  : double.MaxValue;
        foreach (Move move in possibleMoves)
        {
            board.MakeMove(move);

            double value = GetBestMove(depth - 1, board, isWhite, !isMaximizingPlayer).Item1;

            if (isMaximizingPlayer)
            {
                if (value > bestMoveValue)
                {
                    bestMoveValue = value;
                    bestMove = move;
                }
            }
            else if (value < bestMoveValue)
            {
                bestMoveValue = value;
                bestMove = move;
            }
            board.UndoMove(move);
        }
        if (depth == Ai.depth)
            return (bestMoveValue, bestMove);
        return (bestMoveValue, null);
    }
}
