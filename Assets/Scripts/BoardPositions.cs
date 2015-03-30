
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

    private bool[,] _walkable;
    private bool[,] _doors;
    private List<Building> _buildings = new List<Building>();

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
        InitializeWalkableMap();
        InitializeDoorMap();
        InitialisePotentialGridPositions();
        Trees = new List<Vector2>();
        Rocks = new List<Vector2>();
        Grass = new List<Vector2>();
    }

    private void InitializeDoorMap()
    {
        _doors = new bool[Columns, Rows];
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                _doors[x, y] = false;
            }
        }
    }

    private void InitializeWalkableMap()
    {
        _walkable = new bool[Columns, Rows];
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                _walkable[x, y] = true;
            }
        }
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
        PlaceRandomGroupsOfObjects(Rocks, 20, 40, .3f, .6f, .01f, 5);
        PlaceObjectsWithPerlinNoise(Trees, 0.6f, 0.6f, 15.0f);
        PlaceObjectsWithPerlinNoise(Grass, 0.4f, 0.3f, 4.0f);
    }

    private void PlaceObjectsWithPerlinNoise(List<Vector2> terrainCoordinates, float placementCutoff, float sparseness, float intricacy)
    {
        float scale = intricacy;
        float xOrigin = Random.Range(0.0f, 10000.0f);
        float yOrigin = Random.Range(0.0f, 10000.0f);
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                float xCoord = xOrigin + (float)x / (float)Columns * scale;
                float yCoord = yOrigin + (float)y / (float)Rows * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                if (sample > placementCutoff)
                {
                    if (Random.Range(0.0f, 1.0f) > sparseness)
                    {
                        terrainCoordinates.Add(new Vector2(x, y));
                    }
                }
            }
        }
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

    internal void SetUnwalkable(int x, int y)
    {
        _walkable[x, y] = false;
    }

    internal bool IsWalkable(int x, int y)
    {
        return (_walkable[x,y] == true);
    }

    internal void SetDoor(int x, int y)
    {
        _doors[x, y] = true;
    }

    internal bool IsDoor(int x, int y)
    {
        return (_doors[x, y] == true);
    }

    internal bool AddBuildingToBoard(GameObject myBuilding)
    {
        Building newBuilding = new Building(myBuilding.name, new Vector2(myBuilding.transform.position.x, myBuilding.transform.position.y), myBuilding.transform.eulerAngles.z);
        List<Vector3> buildingOccupies = new List<Vector3>();
        bool canAdd = LegalBuildingPosition(myBuilding, newBuilding, buildingOccupies);

        if (canAdd)
        {
            TurnDraggingBuildingColor(Color.white, myBuilding);
            _buildings.Add(newBuilding);
            foreach (Vector3 spot in buildingOccupies)
            {
                SetUnwalkable((int)spot.x, (int)spot.y);
            }

            foreach (Vector2 door in newBuilding.DoorGridLocations)
            {
                SetDoor((int)door.x, (int)door.y);
            }
        }

        return canAdd;
    }

    private bool LegalBuildingPosition(GameObject buildingToCheck)
    {
        Building newBuilding = new Building(buildingToCheck.name, new Vector2(buildingToCheck.transform.position.x, buildingToCheck.transform.position.y), buildingToCheck.transform.eulerAngles.z);
        List<Vector3> buildingOccupies = new List<Vector3>();
        bool canAdd = LegalBuildingPosition(buildingToCheck, newBuilding, buildingOccupies);
        return canAdd;
    }

    private bool LegalBuildingPosition(GameObject draggingBuilding, Building newBuilding, List<Vector3> buildingOccupies)
    {
        bool canAdd = true;
        var spriteRenderers = draggingBuilding.GetComponentsInChildren(typeof(Renderer));
        foreach (var renderer in spriteRenderers)
        {
            if (renderer.name == "Main")
            {
                Vector3 start = ((Renderer)renderer).bounds.min;
                Vector3 end = ((Renderer)renderer).bounds.max;

                for (float x = start.x; x < end.x; x++)
                {
                    for (float y = start.y; y < end.y; y++)
                    {
                        Vector3 buildingOccupy = new Vector3(Convert.ToInt32(Math.Round(x + 0.5f)), Convert.ToInt32(Math.Round(y + 0.5f)), 0);
                        buildingOccupies.Add(buildingOccupy);
                    }
                }
            }
        }


        foreach (Vector2 door in newBuilding.DoorGridLocations)
        {
            buildingOccupies.Add(new Vector3(door.x, door.y, 0f));
        }

        foreach (Vector3 spot in buildingOccupies)
        {
            if (!IsWalkable((int)spot.x, (int)spot.y))
            {
                canAdd = false;
                break;
            }

            if (IsDoor((int)spot.x, (int)spot.y))
            {
                canAdd = false;
                break;
            }

            if (!IsOnTheMap(new Vector2(spot.x, spot.y)))
            {
                canAdd = false;
                break;
            }
        }

        return canAdd;
    }

    internal void TinterizeForLegalPlacement(GameObject myBuilding)
    {
        if (LegalBuildingPosition(myBuilding))
        {
            TurnDraggingBuildingColor(Color.green, myBuilding);
        }
        else
        {
            TurnDraggingBuildingColor(Color.red, myBuilding);
        }
    }

    private void TurnDraggingBuildingColor(Color tint, GameObject myBuilding)
    {
        if (myBuilding != null)
        {
            var spriteRenderers = myBuilding.GetComponentsInChildren(typeof(SpriteRenderer));
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                if (tint != Color.white)
                    tint = new Color(tint.r, tint.g, tint.b, 0.5f);

                renderer.color = tint;

            }
        }
    }
}
