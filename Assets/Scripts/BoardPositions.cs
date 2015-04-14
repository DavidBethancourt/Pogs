
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public List<Building> Buildings { get; set; }
    public bool[,] WalkableMap {get; set;}
    public bool[,] DoorMap {get; set;}

    public BoardPositions() { }
    public BoardPositions(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        InitializeWalkableMap();
        InitializeDoorMap();
        Trees = new List<Vector2>();
        Rocks = new List<Vector2>();
        Grass = new List<Vector2>();
        Buildings = new List<Building>();
    }

    

    private void InitializeDoorMap()
    {
        DoorMap = new bool[Columns, Rows];
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                DoorMap[x, y] = false;
            }
        }
    }

    private void InitializeWalkableMap()
    {
        WalkableMap = new bool[Columns, Rows];
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                WalkableMap[x, y] = true;
            }
        }
    }



    internal bool IsOnTheMap(Vector2 potential)
    {
        if (potential.x < Columns && potential.x >= 0)
            if ((potential.y < Rows && potential.y >= 0))
                return true;

        return false;
    }

    internal void SetUnwalkable(int x, int y)
    {
        WalkableMap[x, y] = false;
    }

    internal void SetWalkable(int x, int y)
    {
        WalkableMap[x, y] = true;
    }

    internal bool IsWalkable(int x, int y)
    {
        return (WalkableMap[x,y] == true);
    }

    internal void SetDoor(int x, int y)
    {
        DoorMap[x, y] = true;
    }
    internal void SetNoDoor(int x, int y)
    {
        DoorMap[x, y] = false;
    }

    internal bool IsDoor(int x, int y)
    {
        return (DoorMap[x, y] == true);
    }

    internal bool AddBuildingToBoard(GameObject myBuilding)
    {
        Building newBuilding = new Building(myBuilding.name, new Vector2(myBuilding.transform.position.x, myBuilding.transform.position.y), myBuilding.transform.eulerAngles.z);
        List<Vector3> buildingOccupies = new List<Vector3>();
        bool fit = DoesNewBuildingFit(myBuilding, newBuilding, buildingOccupies);

        if (fit)
        {
            Buildings.Add(newBuilding);
            foreach (Vector3 spot in buildingOccupies)   // buildingOccupies has door locations in it!
            {
                SetUnwalkable((int)spot.x, (int)spot.y);  // so we should not set all spots unwalkable; some are door spots... 
            }

            foreach (Vector2 door in newBuilding.DoorGridLocations)
            {
                SetDoor((int)door.x, (int)door.y);
            }
        }

        return fit;
    }

    internal bool LegalBuildingPosition(GameObject buildingToCheck)
    {
        Building newBuilding = new Building(buildingToCheck.name, new Vector2(buildingToCheck.transform.position.x, buildingToCheck.transform.position.y), buildingToCheck.transform.eulerAngles.z);
        List<Vector3> buildingOccupies = new List<Vector3>();
        bool fit = DoesNewBuildingFit(buildingToCheck, newBuilding, buildingOccupies);
        return fit;
    }

    private bool DoesNewBuildingFit(GameObject draggingBuilding, Building newBuilding, List<Vector3> buildingOccupies)
    {
        bool fit = true;
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
            if (!GridSquareAvailable(door))
                fit = false;
        }

        foreach (Vector3 spot in buildingOccupies)
        {
            if(!GridSquareAvailable(new Vector2(spot.x, spot.y)))
                fit = false;
        }

        return fit;
    }

    private bool  GridSquareAvailable(Vector2 spot)
    {
        bool canAdd = true;
        if (!IsWalkable((int)spot.x, (int)spot.y))
        {
            canAdd = false;
        }

        if (IsDoor((int)spot.x, (int)spot.y))
        {
            canAdd = false;
        }

        if (!IsOnTheMap(new Vector2(spot.x, spot.y)))
        {
            canAdd = false;
        }
        return canAdd;
    }
}
