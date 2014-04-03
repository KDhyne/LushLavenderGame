using UnityEngine;
using System.Collections;
using System.IO;

public class S_RunPlayer : S_Actor
{	
	enum PlayerState
	{
		normal,
		aiming
	}
    //Cache the PlayerState
	PlayerState st_currentPlayerState;
    public Vector3 v3_bulletOffset;
	//The target ridicule
	public GameObject go_targetPrefab;
    //Bullets
    public GameObject go_bulletPrefab;
	//Cache the mouse position vector
	public Vector2 mousePos;

	// Use this for initialization
	public override void Start() 
	{
        i_hp = 15;
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() 
	{
        #region Player Movement
        //Use the Horizontal movement axis for input
        MoveHorizontal(1);
        f_actorSpeed = f_maxActorSpeed;

        //Jump
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveVertical(1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveVertical(-1);
        }
        #endregion

        base.Update();
	}

    
}
