public class Move
{
    private Point startPoint;
    private Point targetPoint;
    public Move(Point start, Point target)
    {
        startPoint = start;
        targetPoint = target;
    }
    public Point GetStartPoint() => startPoint;
    public Point GetTargetPoint() => targetPoint;
    public override string ToString()
    {
        return startPoint + "-->" + targetPoint;
    }
}