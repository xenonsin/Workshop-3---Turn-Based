using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    public GUIStyle style;
    void OnGUI()
    {

        GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2, 300, 200));

        GUI.Label(new Rect(30, 20, 200, 50), "Save The Panda!", style);

        if (GUI.Button(new Rect(50, 60, 110, 50), "Start Game"))
        {

            Application.LoadLevel("level 1");
        }

        if (GUI.Button(new Rect(50, 120, 110, 50), "CPP Game Dev"))
        {
            Application.OpenURL("http://www.cppgamedev.com/");
        }

        GUI.EndGroup();

        
    }
}
