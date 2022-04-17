public class Point
{
    public byte x;
    public byte y;

    public Point(byte x, byte y)
    {
        this.x = x;
        this.y = y;
    }
    public Point(string str)
    {
        //str = x,y
        string[] arr = str.Split(',');
        this.x = (byte)float.Parse(arr[0]);
        this.y = (byte)float.Parse(arr[1]);
    }
    public override string ToString()
    {
        return x + "," + y;
    }
}