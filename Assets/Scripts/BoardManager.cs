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
        Positions.PlaceTerrain();

        DrawObjects(Grass, Positions.Grass);
        DrawObjects(Rock, Positions.Rocks);
        DrawObjects(Tree, Positions.Trees);
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
}