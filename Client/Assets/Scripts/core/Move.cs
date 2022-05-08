public class Move
{
    private Point startPoint;
    private Point targetPoint;
    private BasePiece destroyedPiece = null;
    public Move(Point start, Point target, BasePiece destroyPiece = null)
    {
        startPoint = start;
        targetPoint = target;
        destroyedPiece = destroyPiece;
    }
    public Move(string str, string str2, BasePiece destroyPiece = null)
    {
        startPoint = new Point(str);
        targetPoint = new Point(str2);
        destroyedPiece = destroyPiece;
    }
    public Point GetStartPoint() => startPoint;
    public Point GetTargetPoint() => targetPoint;
    public BasePiece GetDestroyedPiece() => destroyedPiece;
    public override string ToString()
    {
        return string.Format("{0}_{1}", startPoint, targetPoint);
    }
}