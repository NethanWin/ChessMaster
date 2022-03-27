using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Branch
{
    public string board;
    public bool isMax;
    public List<Branch> next;

    public Branch(string board, bool isMax = true)
    {
        this.board = board;
        this.isMax = isMax;
        this.next = Chess.GetNextGen(this);
    }

    public override string ToString()
    {
        string str = board;
        if (!IsEmptyList(next))
        {
            str += "\n";
            foreach (Branch b in next)
                if (!IsEmptyList(next))
                    str += b.ToString() + " - ";
        }
        return str;
    }
    /*public (int, List<Branch>) GetBestMoveAndScore()
    {
        
    }*/
    public static bool IsEmptyList<T>(List<T> list)
    {
        if (list == null)
        {
            return true;
        }

        return !list.Any();
    }
}
