public class Point
{
    public byte x;
    public byte y;

    public Point(byte x, byte y)
    {
        this.x = x;
        this.y = y;
    }
    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}