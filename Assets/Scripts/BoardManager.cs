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
    private BoardPositions _boardPositions;

    public void SetupScene()
    {
        _boardPositions = new BoardPositions(columns, rows);
        _terrainHolder = new GameObject("Terrain").transform; 
        TheWholeWorldIsDirt();
        _boardPositions.PlaceTerrain();

        DrawObjects(Grass, _boardPositions.Grass);
        DrawObjects(Rock, _boardPositions.Rocks);
        DrawObjects(Tree, _boardPositions.Trees);
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

    internal void SetUnwalkable(int x, int y)
    {
        _boardPositions.SetUnwalkable(x, y);
    }

    internal bool IsWalkable(int x, int y)
    {
        return _boardPositions.IsWalkable(x, y);
    }

    internal bool AddBuildingToBoard(GameObject _draggingBuilding)
    {
        return _boardPositions.AddBuildingToBoard(_draggingBuilding);
    }

    internal void TinterizeForLegalPlacement(GameObject _draggingBuilding)
    {
        _boardPositions.TinterizeForLegalPlacement(_draggingBuilding);
    }
}