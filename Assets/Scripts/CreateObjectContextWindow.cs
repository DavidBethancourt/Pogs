using UnityEngine;
using System.Collections;

public class CreateObjectContextWindow : MonoBehaviour 
{
    public GameObject Pog;
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
                if (GUILayout.Button("Pog"))
                {
                    _contextMenuOpen = false;
                    Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(_rightClickScreenPosition.x, _rightClickScreenPosition.y, 0));
                    Instantiate(Pog, new Vector3(worldpoint.x, worldpoint.y, 0), Quaternion.identity);
                }
                GUILayout.EndVertical();
            GUILayout.EndArea();
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
