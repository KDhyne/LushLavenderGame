using UnityEngine;

public class S_Player : S_Actor
{
    public bool IsAlwaysRunning;

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

	// Use this for initialization
	public override void Start() 
	{
        this.HitPoints = 15;
        SpawnLocation = GameObject.Find("Spawn");
        base.Start();
	}
	
	// Update is called once per frame
    public override void Update() 
	{
        #region Player Movement

        if(this.IsAlwaysRunning)
            this.MoveHorizontal(1);

        else //Use the Horizontal movement axis for input
            MoveHorizontal(Input.GetAxis("Horizontal"));

        //Jumping/Sliding
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveVertical(1);

        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            MoveVertical(-1);

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

        base.Update();
	}

    public override void OnTriggerEnter(Collider otherObj)
    {
        base.OnTriggerEnter(otherObj);

        if (otherObj.tag == "Silver Bell")
        {
            Debug.Log("+1 Silver Bell");
            var bell = otherObj.GetComponent<S_Collectable>();

            //TODO: Tell the level manager that the player has collected a silver bell
            //TODO: Check if the number of bells collected equals the total number in the level. If so, Give a Golden Bell

            bell.DestroyCollectable();
        }
    }

    protected override void DestroyActor()
    {
        this.HitPoints = 10;
        this.ActorTransform.position = SpawnLocation.transform.position;
        this.CurrentActorState = ActorState.Alive;
    }

    protected override void Decelerate()
    {
        //If no horizontal movement key is pushed, Decelerate
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            this.HorizontalSpeed -= 1f;
        }
    }
}
