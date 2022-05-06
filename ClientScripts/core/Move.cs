public class Move
{
    private Point startPoint;
    private Point targetPoint;
    public Move(Point start, Point target)
    {
        startPoint = start;
        targetPoint = target;
    }
    public Move(string str, string str2)
    {
        startPoint = new Point(str);
        targetPoint = new Point(str2);
    }
    public Point GetStartPoint() => startPoint;
    public Point GetTargetPoint() => targetPoint;
    public override string ToString()
    {
        return string.Format("{0}_{1}", startPoint, targetPoint);
    }
}