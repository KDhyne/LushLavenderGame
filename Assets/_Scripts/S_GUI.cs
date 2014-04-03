using UnityEngine;
using System.Collections;
using System.IO;

public class S_GUI : MonoBehaviour 
{
    public int i_waveCount;
    public float f_enemySpeed = 1f;

    public int i_score = 0;
    //awful lose code
    public int i_loseScore = 0;

    public GUIStyle GS_style;


    //awful lose code
    public GameObject go_player;
    public GameObject go_floor;
    
    void Start()
    {
        i_waveCount = 1;
        InvokeRepeating("RaiseScore", 5f, 10f);
    }

    void Update()
    {
        if (i_loseScore >= 15)
        {
            go_player.rigidbody.isKinematic = false;
            go_player.rigidbody.useGravity = true;
            go_floor.rigidbody.isKinematic = false;
            go_floor.rigidbody.useGravity = true;
        }
    }

    void OnGUI()
    {

        

        //GUI.Label(new Rect(10, 10, 50, 25), "Score: " + i_score.ToString(), GS_style);

        //GUI.Label(new Rect(10, 40, 50, 25), "Life: " + (15 - i_loseScore).ToString(), GS_style);

        if (Time.timeSinceLevelLoad <= 5f)
        {
            //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 50, 25), "Get 100 points!", GS_style);
        }
        

        if (i_score >= 100)
        {
            GUI.Label(new Rect(Screen.width/2, Screen.height/2, 200, 50), "You win!", GS_style);
        }

        if (i_loseScore >= 15)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2, 200, 100), "Lose. Press F5 to restart", GS_style);
        }
        
    }

    void RaiseScore()
    {
        f_enemySpeed += .3f;
    }
}
