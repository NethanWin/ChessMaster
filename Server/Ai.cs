using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Ai
{
    public static int depth;
    public static Move GetBestMoveOne(Board board, bool isWhite)
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
    public static Move GetBestMove(Board board)
    {
        Ai.depth = 3;
        return GetBestMove(Ai.depth, board, true, true).Item2;
    }

    private static (double, Move) GetBestMove(int depth, Board board,
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


    //Old Tests
    /*public static Move Minmax(Board board, bool blackToPlay)
    {
        return Minmax(board, 4, true, blackToPlay).Item2;
    }*/
    /*private static (double, Move) Minmax(Board board, int depth, bool maximizingPlayer, bool blackToPlay)
    {
        if (depth == 0)
        {
            return (board.EvaluateBoard(blackToPlay), null);
        }
        List<Board> nextGenBoards = board.GetNextGenBoards(blackToPlay);
        if (maximizingPlayer)
        {

            double value = int.MinValue;
            foreach (Board b in nextGenBoards)
            {
                double minmaxResult = Minmax(b, depth - 1, false, !blackToPlay).Item1;
                value = Math.Max(value, minmaxResult);
                if (depth == Constants.MAX_DEPTH)
                {
                    //moveScores.Add(b, minmaxResult);
                }
            }
            return (value, null);
        }
        else
        {
            double value = int.MaxValue;
            foreach (Board b in nextGenBoards)
            {
                double minmaxResult = Minmax(b, depth - 1, true, !blackToPlay).Item1;
                value = Math.Min(value, minmaxResult);
                if (depth == Constants.MAX_DEPTH)
                {
                    //moveScores.Add(move, minmaxResult);
                }
            }
            return (value, null);
        }
    }
    */
    /*
    public static int OldMinmax(Board board, int depth, bool maximizingPlayer, bool blackToPlay)
    {
        if (depth == 0)
        {
            return board.EvaluateBoard(blackToPlay);
        }
        List<Board> nextGenBoards = board.GetNextGenBoards(blackToPlay);
     //   Console.WriteLine(board.GenerateMoves(blackToPlay).Count);
        if (maximizingPlayer)
        {

            int value = int.MinValue;
            foreach (Board b in nextGenBoards)
            {
                int minmaxResult = OldMinmax(b, depth - 1, false, !blackToPlay);
                value = Math.Max(value, minmaxResult);
                if (depth == 4)
                {
                    //moveScores.Add(b, minmaxResult);
                }
            }
            return value;
        }
        else
        {
            int value = int.MaxValue;
            foreach (Board b in nextGenBoards)
            {
                int minmaxResult = OldMinmax(b, depth - 1, true, !blackToPlay);
                value = Math.Min(value, minmaxResult);
                if (depth == 4)
                {
                    //moveScores.Add(move, minmaxResult);
                }
            }
            return value;
        }
    }
    public static (double, Move) OldCalcBestMoveNoAB(int depth, Board board,
                                bool isWhite, bool isMaximizingPlayer = true)
    {
        //board = new Board(board.GetFen());
        if (depth == 0)
        {
            return (board.EvaluateBoard(isWhite), null);
        }

        Move bestMove = null;
        List<Move> possibleMoves = board.GenerateMoves(isWhite);
        Console.WriteLine(possibleMoves.Count + " " + board.GetNextGenBoards(isWhite).Count);
        double bestMoveValue = isMaximizingPlayer ? double.MinValue
                                                  : double.MaxValue;

        foreach (Move move in possibleMoves)
        {
            board.MakeMove(move);

            double value = OldCalcBestMoveNoAB(depth - 1, board, isWhite, !isMaximizingPlayer).Item1;

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
        return (bestMoveValue, bestMove);
    }

    public static (double, Move) OldCalcBestMove(int depth, Board board,
                                    bool isWhite,
                                    double alpha = double.MinValue,
                                    double beta = double.MaxValue,
                                    bool isMaximizingPlayer = true)
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
            double value = OldCalcBestMove(depth - 1, board, isWhite, alpha, beta, !isMaximizingPlayer).Item1;
            //Console.WriteLine((isMaximizingPlayer ? "Max: " : "Min:") + " " + depth + " " + move + " " + value);
            if (isMaximizingPlayer)
            {
                if (value > bestMoveValue)
                {
                    bestMoveValue = value;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, value);
            }
            else
            {
                if (value < bestMoveValue)
                {
                    bestMoveValue = value;
                    bestMove = move;
                }
                beta = Math.Max(beta, value);
            }
            board.UndoMove(move);
            if (beta <= alpha)
            {
                Console.WriteLine("prune", alpha, beta);
                break;
            }
        }
        //Console.WriteLine(bestMoveValue);
        return (bestMoveValue, bestMove);

    }
    
}*/
