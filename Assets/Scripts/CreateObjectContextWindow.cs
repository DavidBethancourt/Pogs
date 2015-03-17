using UnityEngine;
using System.Collections;
using System;

public class CreateObjectContextWindow : MonoBehaviour 
{
    public GameObject Pog;
    public GameObject Tree;
    public GameObject Rock;
    public GameObject Grass;
    private bool _contextMenuOpen = false;
    private Vector2 _rightClickScreenPosition;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButton(1))
        {
            if (!_contextMenuOpen)
            {
                _rightClickScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                _contextMenuOpen = true;
            }
        }
	}

    void OnGUI()
    {
        if (_contextMenuOpen)
        {
            GUILayout.BeginArea(new Rect(_rightClickScreenPosition.x, Screen.height - _rightClickScreenPosition.y, 200, 200));
                GUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Create Object");
                    if (GUILayout.Button("X", GUILayout.Width(23.0f)))
                    {
                        _contextMenuOpen = false;
                    }
                    GUILayout.EndHorizontal();
                    ObjectButtonLogic("Pog", Pog);
                    ObjectButtonLogic("Grass", Grass);
                    ObjectButtonLogic("Rock", Rock);
                    ObjectButtonLogic("Tree", Tree);
                GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    private void ObjectButtonLogic(string buttonTitle, GameObject item)
    {
        if (GUILayout.Button(buttonTitle))
        {
            _contextMenuOpen = false;
            Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(_rightClickScreenPosition.x, _rightClickScreenPosition.y, 0));
            int Xclose = Convert.ToInt32(Math.Round(worldpoint.x, 0));
            int Yclose = Convert.ToInt32(Math.Round(worldpoint.y, 0));
            Instantiate(item, new Vector3(Xclose, Yclose, 0), Quaternion.identity);
        }
    }

    void DrawWindow(int windowId)
    {
        GUILayout.Label("this is a label");
        if (GUILayout.Button("Close Window"))
        {
            _contextMenuOpen = false;
        }
    }
}
