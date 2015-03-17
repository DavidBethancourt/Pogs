using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        boardScript.SetupScene();
    }


    void Update()
    {

    }
}