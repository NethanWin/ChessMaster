using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Point2
{
    public byte x;
    public byte y;

    public Point2(byte x, byte y)
    {
        this.x = x;
        this.y = y;
    }
    public Point2((byte, byte) p)
    {
        this.x = p.Item1;
        this.y = p.Item2;
    }
    public (byte, byte) GetPoint()
    {
        return (x, y);
    }
}
