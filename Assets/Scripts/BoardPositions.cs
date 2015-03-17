
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BoardPositions
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<Vector2> Trees { get; set; }
    public List<Vector2> Rocks { get; set; }
    public List<Vector2> Grass { get; set; }

    private List<Vector2> _potentialGridPositions = new List<Vector2>();
    public BoardPositions(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        InitProperties();
    }

    public BoardPositions()
    {
        InitProperties();
    }

    private void InitProperties()
    {
        InitialisePotentialGridPositions();
        Trees = new List<Vector2>();
        Rocks = new List<Vector2>();
        Grass = new List<Vector2>();
    }

    void InitialisePotentialGridPositions()
    {
        _potentialGridPositions.Clear();
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                _potentialGridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    internal void PlaceTerrain()
    {
        PlaceRandomGroupsOfObjects(Grass, 5, 10, .9f, 1f, .01f, 12);
        PlaceRandomGroupsOfObjects(Rocks, 10, 20, .3f, .6f, .01f, 5);
        PlaceRandomGroupsOfObjects(Trees, 5, 10, .7f, .7f, .01f, 9);
    }

    private void PlaceRandomGroupsOfObjects(List<Vector2> terrainCoordinates, int minCenters, int maxCenters, float reproRate, float sparseness, float newCenterLikelihood, int maxDistanceFromCenter)
    {
        List<Vector2> consideredCoordinates = new List<Vector2>();
        int numCenters = Random.Range(minCenters, maxCenters);
        for (int i = 0; i < numCenters; i++)
        {
            Vector2 center = RandomUnusedPosition();
            terrainCoordinates.Add(center);
            consideredCoordinates.Add(center);
            ApplyBloomCriteria(center, terrainCoordinates, consideredCoordinates, reproRate, maxDistanceFromCenter, center);
        }

        List<Vector2> toRemove = new List<Vector2>();
        foreach (Vector2 terrainCoordinate in terrainCoordinates)
        {
            if (Random.Range(0.0f, 1.0f) > sparseness)
            {
                toRemove.Add(terrainCoordinate);
            }
        }

        foreach(Vector2 removable in toRemove)
        {
            terrainCoordinates.Remove(removable);
        }
    }

    private void ApplyBloomCriteria(Vector2 bloom, List<Vector2> terrainCoordinates, List<Vector2> consideredCoordinates, float reproRate, int maxDistanceFromCenter, Vector2 originalCenter)
    {
        List<Vector2> nearbyCoords = GetNearbyCoordinates(bloom);
        foreach(Vector2 nearbyCoord in nearbyCoords)
        {
            if (!LivesInPositons(consideredCoordinates, nearbyCoord))
            {
                if (Distance(originalCenter, nearbyCoord) < maxDistanceFromCenter)
                {
                    if (Random.Range(0.0f, 1.0f) < reproRate)
                    {
                        terrainCoordinates.Add(nearbyCoord);
                        consideredCoordinates.Add(nearbyCoord);
                        ApplyBloomCriteria(nearbyCoord, terrainCoordinates, consideredCoordinates, reproRate, maxDistanceFromCenter, originalCenter);
                    }
                }
            }

            if (!LivesInPositons(consideredCoordinates, nearbyCoord))
                consideredCoordinates.Add(nearbyCoord);
        }
    }

    private bool LivesInPositons(List<Vector2> positions, Vector2 point)
    {
        bool foundIt = false;
        foreach (Vector2 position in positions)
        {
            if (position.x == point.x && position.y == point.y)
            {
                foundIt = true;
                break;
            }
        }
        return foundIt;
    }

    private float Distance(Vector2 point1, Vector2 point2)
    {
        float horizontalDistance = point1.x - point2.x;
        float verticalDistance = point1.y - point2.y;
        double distance = Math.Sqrt(Math.Pow(horizontalDistance, 2.0f) + Math.Pow(verticalDistance, 2.0f));
        return Convert.ToSingle(distance);
    }

    private List<Vector2> GetNearbyCoordinates(Vector2 center)
    {
        List<Vector2> potentialNearby = new List<Vector2>();
        potentialNearby.Add(new Vector2(center.x, center.y + 1));
        potentialNearby.Add(new Vector2(center.x + 1, center.y + 1));
        potentialNearby.Add(new Vector2(center.x + 1, center.y));
        potentialNearby.Add(new Vector2(center.x + 1, center.y - 1));
        potentialNearby.Add(new Vector2(center.x, center.y - 1));
        potentialNearby.Add(new Vector2(center.x - 1, center.y - 1));
        potentialNearby.Add(new Vector2(center.x - 1, center.y));
        potentialNearby.Add(new Vector2(center.x - 1, center.y + 1));
        
        List<Vector2> nearby = new List<Vector2>();
        foreach(Vector2 potential in potentialNearby)
        {
            if (IsOnTheMap(potential))
                nearby.Add(potential);
        }  
        return nearby;
    }

    private bool IsOnTheMap(Vector2 potential)
    {
        if (potential.x < Columns && potential.x >= 0)
            if ((potential.y < Rows && potential.y >= 0))
                return true;

        return false;
    }

    Vector2 RandomUnusedPosition()
    {
        int randomIndex = Random.Range(0, _potentialGridPositions.Count);
        Vector2 randomPosition = _potentialGridPositions[randomIndex];
        _potentialGridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }
}
