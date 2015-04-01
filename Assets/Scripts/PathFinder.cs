using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathFinder
{
    private bool[,] _beenEvaluated;
    private bool[,] _walkableMap;
    private PriorityQueue<PathFindingNode> _pQueue;

    public PathFinder() { }

    private List<SimplePoint> FindPath(SimplePoint start, SimplePoint end, bool[,] walkableMap)
    {
        _beenEvaluated = new bool[walkableMap.GetLength(0), walkableMap.GetLength(1)];
        _walkableMap = walkableMap;
        _pQueue = new PriorityQueue<PathFindingNode>();

        List<SimplePoint> shortestPath = null;
        if (start.X == end.X && start.Y == end.Y)
        {
            shortestPath = new List<SimplePoint>();
            shortestPath.Add(end);
        }
        else if (!IsOnMap(start) || !IsOnMap(end))
        {
            shortestPath = null;
        }
        else
        {
            List<SimplePoint> path = new List<SimplePoint>();
            AddNearbyNodesToPriorityQueue(start, 0f, path, end);
            while (_pQueue.Count() > 0)
            {
                PathFindingNode candidate = _pQueue.Dequeue();
                if (candidate.FoundEnd)
                {
                    shortestPath = candidate.PathFromStart;
                    break;
                }

                AddNearbyNodesToPriorityQueue(new SimplePoint(candidate.Location.X, candidate.Location.Y), candidate.Gscore, candidate.PathFromStart, end);
            }
        }

        return shortestPath;
    }

    private void AddNearbyNodesToPriorityQueue(SimplePoint node, float Gscore, List<SimplePoint> pathSoFar, SimplePoint end)
    {
        AddNodeToQueueIfPossible(new SimplePoint(node.X + 1, node.Y), Gscore + 1f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X - 1, node.Y), Gscore + 1f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X, node.Y + 1), Gscore + 1f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X, node.Y - 1), Gscore + 1f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X + 1, node.Y + 1), Gscore + 1.414f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X + 1, node.Y - 1), Gscore + 1.414f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X - 1, node.Y - 1), Gscore + 1.414f, pathSoFar, end);
        AddNodeToQueueIfPossible(new SimplePoint(node.X - 1, node.Y + 1), Gscore + 1.414f, pathSoFar, end);
    }

    private void AddNodeToQueueIfPossible(SimplePoint node, float gScore, List<SimplePoint> pathSoFar, SimplePoint end)
    {
        if (IsOnMap(node) && _walkableMap[node.X, node.Y])
        {
            if (!_beenEvaluated[node.X, node.Y])
            {
                double hScore = Math.Sqrt(Math.Pow(Math.Abs(node.X - end.X), 2) + Math.Pow(Math.Abs(node.Y - end.Y), 2));
                PathFindingNode myNode = new PathFindingNode(node, gScore, Convert.ToSingle(hScore), pathSoFar, end);
                _pQueue.Enqueue(myNode);
                _beenEvaluated[node.X, node.Y] = true;
            }
        }
    }

    private bool IsOnMap(SimplePoint node)
    {
        bool onMap = true;
        if (node.X < 0 || node.X >= _walkableMap.GetLength(0))
            onMap = false;
        if (node.Y < 0 || node.Y >= _walkableMap.GetLength(1))
            onMap = false;

        return onMap;
    }
}

