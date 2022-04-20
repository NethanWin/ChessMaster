public class Point
{
    public float x;
    public float y;
    public Point(float x, float y)
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
    public Point(Point p)
    {
        x = p.x;
        y = p.y;
    }
    public override string ToString() => x + ", " + y;
}
