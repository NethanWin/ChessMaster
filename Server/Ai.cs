using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Ai
{
    public static int count = 0;
    public static int Minmax(Board board, int depth, bool maximizingPlayer, bool blackToPlay)
    {
        if (depth == 0)
        {
            count++;
            return board.EvaluateBoard(blackToPlay);
        }
        List<Board> nextGenBoards = board.GetNextGenBoards(blackToPlay);
        if (maximizingPlayer)
        {

            int value = int.MinValue;
            foreach (Board b in nextGenBoards)
            {
                int minmaxResult = Minmax(b, depth - 1, false, !blackToPlay);
                value = Math.Max(value, minmaxResult);
                if (depth == Constants.MAX_DEPTH)
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
                int minmaxResult = Minmax(b, depth - 1, true, !blackToPlay);
                value = Math.Min(value, minmaxResult);
                if (depth == Constants.MAX_DEPTH)
                {
                    //moveScores.Add(move, minmaxResult);
                }
            }
            return value;
        }
    }
}
