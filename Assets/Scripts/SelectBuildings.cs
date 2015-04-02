using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SelectBuildings : MonoBehaviour 
{
    public GameObject Farm;
    public GameObject Incubator;
    public GameObject House;
    public GameObject TownHall;

    public Texture FarmMenuTexture;
    public Texture IncubatorMenuTexture;
    public Texture HouseMenuTexture;
    public Texture TownHallMenuTexture;

    private bool _buildMenuExposed = false;
    private GameObject _draggingBuilding = null;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0) && _draggingBuilding != null) // user is trying to plant the building
        {
            if (GameManager.instance.BoardLogic.Positions.AddBuildingToBoard(_draggingBuilding))
            {
                _draggingBuilding = null;// stop dragging the building around and leave it where it sits.
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            CancelBuildingDrag();
        }

	    if (_draggingBuilding != null)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _draggingBuilding.transform.Rotate(Vector3.back, 90.0f);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                _draggingBuilding.transform.Rotate(Vector3.forward, 90.0f);
            }

            Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            int Xclose = Convert.ToInt32(Math.Round(worldpoint.x, 0));
            int Yclose = Convert.ToInt32(Math.Round(worldpoint.y, 0));
            _draggingBuilding.transform.position = new Vector3(Xclose, Yclose, 0);
            GameManager.instance.BoardLogic.Positions.TinterizeForLegalPlacement(_draggingBuilding);
        }
	}

    private void CancelBuildingDrag()
    {
        if (_draggingBuilding != null)
        {
            Destroy(_draggingBuilding);
            _draggingBuilding = null;
        }
    }

    void OnGUI()
    {
        if (_buildMenuExposed)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 100, 0, 100, Screen.height));
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(">", GUILayout.Width(20.0f)))
                        {
                            _buildMenuExposed = false;
                        }
                        GUILayout.Label(" Build Menu");
                    }
                    GUILayout.EndHorizontal();
                    ButtonLogic("Farm", FarmMenuTexture, Farm);
                    ButtonLogic("Incubator", IncubatorMenuTexture, Incubator);
                    ButtonLogic("House", HouseMenuTexture, House);
                    ButtonLogic("TownHall", TownHallMenuTexture, TownHall);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(new Rect(Screen.width - 25, 0, 100, Screen.height));
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("<", GUILayout.Width(20.0f)))
                        {
                            _buildMenuExposed = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
    }

    private void ButtonLogic(string buttonTitle, Texture menuTexture, GameObject prefab)
    {
        if (GUILayout.Button(menuTexture))
        {
            Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            int Xclose = Convert.ToInt32(Math.Round(worldpoint.x, 0));
            int Yclose = Convert.ToInt32(Math.Round(worldpoint.y, 0));
            _draggingBuilding = (GameObject)Instantiate(prefab, new Vector3(Xclose, Yclose, 0), Quaternion.identity);
        }
    }
}
