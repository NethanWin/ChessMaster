public class Move
{
    private Point startPoint;
    private Point targetPoint;
    public Move(Point start, Point target)
    {
        startPoint = start;
        targetPoint = target;
    }
    public Point GetstartPoint() => startPoint;
    public Point GetTargetPoint() => targetPoint;
    public override string ToString()
    {
        return startPoint + "-->" + targetPoint;
    }
}