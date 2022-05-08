public class Point
{
    public float x;
    public float y;

    public Point(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public Point(Point p)
    {
        x = p.x;
        y = p.y;
    }
    public Point(string str)
    {
        //str = x,y
        string[] arr = str.Split(',');
        this.x = float.Parse(arr[0]);
        this.y = float.Parse(arr[1]);
    }
    public override string ToString() => x + ", " + y;
}
