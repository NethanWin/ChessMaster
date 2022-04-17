public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
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
        return x + ", " + y;
    }
}