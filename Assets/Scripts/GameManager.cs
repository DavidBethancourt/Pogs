using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager _boardScript;						//Store a reference to our BoardManager which will set up the level.
    public GameObject Marker;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        _boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        _boardScript.SetupScene();
    }

    public void SetUnwalkable(int x, int y)
    {
        _boardScript.SetUnwalkable(x, y);
    }
    internal bool IsWalkable(int x, int y)
    {
        return _boardScript.IsWalkable(x, y);
    }


    void Update()
    {

    }

    internal bool AddBuildingToBoard(GameObject _draggingBuilding)
    {
        return _boardScript.AddBuildingToBoard(_draggingBuilding);
    }

    internal void TinterizeForLegalPlacement(GameObject _draggingBuilding)
    {
        _boardScript.TinterizeForLegalPlacement(_draggingBuilding);
    }
}