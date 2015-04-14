using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
    public BoardManager Board;						//Store a reference to our BoardManager which will set up the level.
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

        Board = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        Board.SetupScene();
    }

    void Update()
    {

    }

    internal void DoMarker(Vector2 position)
    {
        Instantiate(Marker, new Vector3(position.x, position.y - 2, 0f), Quaternion.identity);
    }
}