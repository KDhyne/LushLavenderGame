using System.Collections;

using UnityEngine;

public class Player : Actor
{
    public bool IsAlwaysRunning;
    public float SpeedUpAmount;
    public int SpeedUpLevel;

    private float initialMaxHorizontalSpeed;

	/*enum PlayerState
	{
		Normal,
		Aiming
	}*/

	//PlayerState currentPlayerState;

    public Vector3 BulletOffset;
	public GameObject TargetPrefab;
    public GameObject BulletPrefab;
	public Vector2 MousePosition2D;

	public bool CanPlayerMove = true;

	// Use this for initialization
	public override void Start() 
	{
        SpawnLocation = GameObject.Find("Spawn");
	    SpeedUpLevel = 0;
	    this.initialMaxHorizontalSpeed = this.MaxHorizontalSpeed;
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update()
    {
        base.Update();

        #region Player Movement

        //Change the max speed based on speed up level
        this.MaxHorizontalSpeed = (initialMaxHorizontalSpeed + (SpeedUpAmount * SpeedUpLevel));

        //Return if player can't move
        if (!this.CanPlayerMove)
            return;

        if (this.IsAlwaysRunning) //Always move at max speed
        {
            this.HorizontalSpeed = this.MaxHorizontalSpeed;
            this.MoveHorizontal(1, true);
        }

        else //Use the Horizontal movement axis for input
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
                this.MoveHorizontal(-1, true);
		        
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
                this.MoveHorizontal(1, true);
            
            else
            {
                this.MoveHorizontal(0, false);
            }
        }

        //Jumping/Sliding
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
            this.MoveVertical(-1);
		    
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) 
            this.MoveVertical(1);

        else 
            this.MoveVertical(0);

        #endregion

        #region Aim and Shoot
        /*//Move the target with the mouse
        MousePosition2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TargetPrefab.transform.position = MousePosition2D;

        //Fire a projectile whenever the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            this.CanLookLeftRight = false;

            //Build the bullet's position
            if (this.FacingRight)
            {
                BulletOffset = new Vector3(-1.7f, .6f);
            }
            else
            {
                BulletOffset = new Vector3(1.7f, .6f);
            }
            //Create a bullet at the player's position
            Instantiate(BulletPrefab, this.ActorTransform.position + BulletOffset, Quaternion.identity);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.CanLookLeftRight = true;
        }
*/
        #endregion

	}

    public override IEnumerator DestroyActor()
    {
        this.CurrentHitPoints = MaxHitPoints;

        //Kill any velocity the player has
        VerticalSpeed = 0f;
        HorizontalSpeed = 0f;

        //Move them to the current spawnpoint
        ActorTransform.position = GameObject.Find("SceneManager").GetComponent<SceneManager>().GetPlayerSpawnLocation();
        this.CurrentActorState = ActorState.Alive;
        return null;
    }
}
