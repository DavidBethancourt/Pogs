using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{
    public int columns = 100;
    public int rows = 100;
    public GameObject Dirt;
    public GameObject Tree;
    public GameObject Rock;
    public GameObject Grass;

    private Transform _terrainHolder;
    public BoardPositions Positions;

    public void SetupScene()
    {
        Positions = new BoardPositions(columns, rows);
        _terrainHolder = new GameObject("Terrain").transform; 
        TheWholeWorldIsDirt();
        PlaceTerrain();

        DrawObjects(Grass, Positions.Grass);
        DrawObjects(Rock, Positions.Rocks);
        DrawObjects(Tree, Positions.Trees);
    }

    internal void PlaceTerrain()
    {
        PlaceRandomGroupsOfObjects(Positions.Rocks, 20, 40, .3f, .6f, .01f, 5);
        PlaceObjectsWithPerlinNoise(Positions.Trees, 0.6f, 0.6f, 15.0f);
        PlaceObjectsWithPerlinNoise(Positions.Grass, 0.4f, 0.3f, 4.0f);
    }

    private void PlaceRandomGroupsOfObjects(List<Vector2> terrainCoordinates, int minCenters, int maxCenters, float reproRate, float sparseness, float newCenterLikelihood, int maxDistanceFromCenter)
    {
        List<Vector2> potentials = new List<Vector2>();
        for (int x = 0; x < Positions.Columns; x++)
        {
            for (int y = 0; y < Positions.Rows; y++)
            {
                potentials.Add(new Vector2(x, y));
            }
        }

        List<Vector2> consideredCoordinates = new List<Vector2>();
        int numCenters = Random.Range(minCenters, maxCenters);
        for (int i = 0; i < numCenters; i++)
        {
            Vector2 center = RandomUnusedPosition(potentials);
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

        foreach (Vector2 removable in toRemove)
        {
            terrainCoordinates.Remove(removable);
        }
    }

    Vector2 RandomUnusedPosition(List<Vector2> potentials)
    {
        int randomIndex = Random.Range(0, potentials.Count);
        Vector2 randomPosition = potentials[randomIndex];
        potentials.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void ApplyBloomCriteria(Vector2 bloom, List<Vector2> terrainCoordinates, List<Vector2> consideredCoordinates, float reproRate, int maxDistanceFromCenter, Vector2 originalCenter)
    {
        List<Vector2> nearbyCoords = GetNearbyCoordinates(bloom);
        foreach (Vector2 nearbyCoord in nearbyCoords)
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
        foreach (Vector2 potential in potentialNearby)
        {
            if (Positions.IsOnTheMap(potential))
                nearby.Add(potential);
        }
        return nearby;
    }

    private void PlaceObjectsWithPerlinNoise(List<Vector2> terrainCoordinates, float placementCutoff, float sparseness, float intricacy)
    {
        float scale = intricacy;
        float xOrigin = Random.Range(0.0f, 10000.0f);
        float yOrigin = Random.Range(0.0f, 10000.0f);
        for (int x = 0; x < Positions.Columns; x++)
        {
            for (int y = 0; y < Positions.Rows; y++)
            {
                float xCoord = xOrigin + (float)x / (float)Positions.Columns * scale;
                float yCoord = yOrigin + (float)y / (float)Positions.Rows * scale;
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

    private void DrawObjects(GameObject thingee, List<Vector2> positions)
    {
        foreach (Vector2 position in positions)
        {
            GameObject instance = Instantiate(thingee, new Vector3(position.x, position.y, 0f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(_terrainHolder); //just organizational to avoid cluttering hierarchy.
        }
    }

    private void TheWholeWorldIsDirt()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject instance = Instantiate(Dirt, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(_terrainHolder); //just organizational to avoid cluttering hierarchy.
            }
        }
    }

    internal void TinterizeForLegalPlacement(GameObject myBuilding)
    {
        if (Positions.LegalBuildingPosition(myBuilding))
        {
            TurnDraggingBuildingColor(Color.green, myBuilding);
        }
        else
        {
            TurnDraggingBuildingColor(Color.red, myBuilding);
        }
    }

    internal void TurnDraggingBuildingColor(Color tint, GameObject myBuilding)
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