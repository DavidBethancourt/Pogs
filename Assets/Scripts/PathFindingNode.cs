using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathFindingNode : IComparable<PathFindingNode>
{
    public float Gscore;
    public float Hscore;
    public double priority; // Smaller values are higher priority. oriority is the same thing as Fscore in this implementation
    public SimplePoint Location;
    public bool FoundEnd = false;
    private List<SimplePoint> _pathFromStart;
    public List<SimplePoint> PathFromStart
    {
        get { return _pathFromStart; }
    }
    public PathFindingNode(SimplePoint node, float gScore, float hScore, List<SimplePoint> pathFromPreviousNode, SimplePoint end)
    {
        priority = gScore + hScore;
        Gscore = gScore;
        Hscore = hScore;
        Location.X = node.X;
        Location.Y = node.Y;
        _pathFromStart = new List<SimplePoint>(pathFromPreviousNode);
        _pathFromStart.Add(new SimplePoint(node.X, node.Y));

        if (end.X == node.X && end.Y == node.Y)
            FoundEnd = true;
    }

    public override string ToString()
    {
        return string.Format("Node [{0},{1}] G:{3} H:{4} F:{5}", Location.X, Location.Y, Gscore, Hscore, priority);
    }

    public int CompareTo(PathFindingNode other)
    {
        if (this.priority < other.priority) return -1;
        else if (this.priority > other.priority) return 1;
        else return 0;
    }
}

