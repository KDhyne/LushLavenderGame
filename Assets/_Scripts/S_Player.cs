using UnityEngine;
using System.Collections;
using System.IO;

public class S_Player : S_Actor
{
    public bool isAlwaysRunning;

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

        if(isAlwaysRunning)
            this.MoveHorizontal(1);
        else
        {
            //Use the Horizontal movement axis for input
            MoveHorizontal(Input.GetAxis("Horizontal"));   
        }

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

        #region Aim and Shoot
        //Move the target with the mouse
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        go_targetPrefab.transform.position = mousePos;

        //Fire a projectile whenever the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            b_canLookLeftRight = false;

            //Build the bullet's position
            if (b_facingRight)
            {
                v3_bulletOffset = new Vector3(-1.7f, .6f);
            }
            else
            {
                v3_bulletOffset = new Vector3(1.7f, .6f);
            }
            //Create a bullet at the player's position
            Instantiate(go_bulletPrefab, T_actorTransform.position + v3_bulletOffset, Quaternion.identity);
        }
        if (Input.GetMouseButtonUp(0))
        {
            b_canLookLeftRight = true;
        }

        #endregion

        base.Update();
	}

    
}
