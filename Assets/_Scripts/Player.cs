using System;
using System.Collections;
using System.Security.Permissions;

using UnityEngine;

public class Player : Actor
{
    public bool IsAlwaysRunning;
    public float SpeedUpAmount;
    public int SpeedUpLevel;
    public GameObject AngerVein;
    private float blinkRate = .1f;
    private int numberOfTimesToBlink = 5;
    private int blinkCount;

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
                this.MoveHorizontal(0, false);
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

    public override IEnumerator TakeDamage(int damageAmount, float invincibileTime = -1)
    {
        //Use actor-specified invincibility time if none is supplied via parameters
        if (Math.Abs(invincibileTime - (-1f)) < .01)
            invincibileTime = InvincibilityTime;

        Debug.Log(invincibileTime);

        //Break out immediately if actor can't be hit
        if (!this.CanBeHit)
            yield break;

        //subtract hit points
        this.CurrentHitPoints -= damageAmount;

        //If the player isn't invincible and won't die, add the anger vein
        if (this.CanBeHit && this.CurrentHitPoints > 0)
        {
            var angerVein = (GameObject)Instantiate(AngerVein, this.transform.position + new Vector3(0, 1.5f, -1), Quaternion.identity);
            angerVein.transform.parent = this.gameObject.transform;
            Destroy(angerVein, this.InvincibilityTime);
        }

        //Temporary invincibility
        this.CanBeHit = false;

        while (blinkCount < numberOfTimesToBlink)
        {
            this.renderer.enabled = !this.renderer.enabled;

            if (gameObject.renderer.enabled)
                blinkCount++;

            yield return new WaitForSeconds(invincibileTime / numberOfTimesToBlink);
        }
        blinkCount = 0;
        this.CanBeHit = true;
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
