using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal enum BuildingUse
{
    Uknown,
    Farm,
    Incubator,
    TownHall,
    House
}
internal class Building
{
    internal BuildingUse BuildingType { get; set; }
    internal Vector2 Location { get; set; }
    internal float Rotation { get; set; }
    internal List<Vector2> DoorGridLocations { get; set; }
    internal List<Vector2> BuildingGridLocations { get; set; }
    internal Building(string buildingName, Vector2 location, float rotation)
    {
        DoorGridLocations = new List<Vector2>();
        BuildingGridLocations = new List<Vector2>();
        string shortBuildingName = buildingName.Remove(buildingName.Length - 7);
        try
        {
            BuildingType = (BuildingUse)Enum.Parse(typeof(BuildingUse), shortBuildingName);
        }
        catch(Exception ex)
        {
            Debug.Log(string.Format("{0} is not a BuildingUse. {1}", shortBuildingName, ex.StackTrace));
        }
        Location = location;
        Rotation = (float)Math.Round(rotation); // need to get it exactly to provide the grid location of the door

        switch (BuildingType)
        {
            case BuildingUse.Incubator:
            case BuildingUse.House:
                if (Rotation == 0f) DoorGridLocations.Add(new Vector2(location.x, location.y - 2f));
                else if (Rotation == 90f) DoorGridLocations.Add(new Vector2(location.x + 2f, location.y));
                else if (Rotation == 180f) DoorGridLocations.Add(new Vector2(location.x, location.y + 2f));
                else if (Rotation == 270f) DoorGridLocations.Add(new Vector2(location.x - 2f, location.y));
                break;
            case BuildingUse.TownHall:
                if (Rotation == 0f)
                {
                    DoorGridLocations.Add(new Vector2(location.x, location.y - 2));
                    DoorGridLocations.Add(new Vector2(location.x + 1, location.y - 2));
                }
                else if (Rotation == 90f)
                {
                    DoorGridLocations.Add(new Vector2(location.x + 2f, location.y));
                    DoorGridLocations.Add(new Vector2(location.x + 2f, location.y + 1));
                }
                else if (Rotation == 180f)
                {
                    DoorGridLocations.Add(new Vector2(location.x - 1, location.y + 2));
                    DoorGridLocations.Add(new Vector2(location.x, location.y + 2));
                }
                else if (Rotation == 270f)
                {
                    DoorGridLocations.Add(new Vector2(location.x - 2f, location.y - 1));
                    DoorGridLocations.Add(new Vector2(location.x - 2f, location.y));
                }
                break;
            case BuildingUse.Farm: // no doors required
                break;
            default:
                throw new Exception(string.Format("Unsupported building type = {0}.", BuildingType));
        }
    }
}
