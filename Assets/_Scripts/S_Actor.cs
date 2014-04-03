using UnityEngine;
using System.Collections.Generic;

public class S_Actor : MonoBehaviour
{
    public enum ActorState
    {
        Alive,
        Dead
    }

    public enum VerticalState
    {
        Airborn,
        Landing,
        Grounded
    }

    public Transform ActorTransform;
    private Animator spriteAnimator;
    public GameObject SpawnLocation;
    public ActorState CurrentActorState = ActorState.Alive;

    public VerticalState CurrentVerticalState = VerticalState.Airborn;


    //Set Hit Points
    public int HitPoints = 10;
    //determines whether or not the actor can be hit
    public bool CanBeHit = true;
    //Score Value
    int ScoreValue = 1;

    #region Horizontal vars

    protected bool CanLookLeftRight = true;
    public bool FacingRight = true;
    public float HorizontalSpeed;
    public float Acceleration;
    public float MaxHorizontalSpeed;
    //Stores the position of the wall, L or R
    public float WallPosition;
    public bool IsTouchingWall = false;
    public bool CanDetectFloors = true;
    public float ContactPositionY;
    public float ActiveFloorRotation;

    #endregion

    #region Vertical vars

    //TODO: Tie vertical speed to animator
    public float VerticalSpeed;
    public float MaxVerticalSpeed;
    public float Gravity;
    public bool CanJump = true;
    public bool IsHanging = false;
    public float JumpForce;

    #endregion

    //Children GameObjects
    public GameObject ActorFoot;

    //Debug object
    public GameObject MarkerObject;
    public List<GameObject> MarkerObjects = new List<GameObject>();
    public bool TestForPostitions = false;


    // Use this for initialization
    public virtual void Start()
    {
        //Cache the transform
        this.ActorTransform = transform;

        //Find and set the actor mesh
        foreach (Transform child in this.ActorTransform)
        {
            if (child.name == "Foot")
            {
                this.ActorFoot = child.gameObject;
            }
        }

        //Set the animation object
        spriteAnimator = this.GetComponent<Animator>();

        //Position the foot at the bottom of the actor collider
        this.ActorFoot.transform.localPosition = new Vector3(this.ActorFoot.transform.localPosition.x, -(this.ActorTransform.collider.bounds.size.y / 2), this.ActorFoot.transform.localPosition.z);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //ActorState checks
        switch (this.CurrentActorState)
        {
            case ActorState.Dead:
                this.DestroyActor();
                break;

            case ActorState.Alive:
                if (this.VerticalSpeed > 0)
                {
                    this.ActorFoot.transform.localPosition = new Vector3(
                        this.ActorFoot.transform.localPosition.x,
                        this.ActorTransform.collider.bounds.size.y / 2,
                        this.ActorFoot.transform.localPosition.z);
                }
                else
                {
                    this.ActorFoot.transform.localPosition = new Vector3(
                        this.ActorFoot.transform.localPosition.x,
                        -(this.ActorTransform.collider.bounds.size.y / 2),
                        this.ActorFoot.transform.localPosition.z);
                }
                if (this.CurrentVerticalState == VerticalState.Airborn && !this.IsHanging)
                {
                    //Apply gravity
                    this.VerticalSpeed -= (this.Gravity * Time.fixedDeltaTime);
                    this.ActorTransform.Translate(Vector3.up * this.VerticalSpeed, Space.World);

                    if (this.VerticalSpeed < this.MaxVerticalSpeed)
                    {
                        this.VerticalSpeed = this.MaxVerticalSpeed;
                    }

                    //If you fall below the Death Volume, die or respawn
                    //if (T_actorTransform.position.y < obj_deathVolume.transform.position.y)
                    //{
                    //    st_charState = S_Char.CharState.Dead;
                    //}

                    spriteAnimator.SetBool("Grounded", false);
                }
                if (this.CurrentVerticalState == VerticalState.Grounded)
                {
                    //Allow the actor to jump again
                    this.VerticalSpeed = 0f;
                    this.CanJump = true;
                    spriteAnimator.SetBool("Grounded", true);
                }
                break;
        }
    }

    void Flip()
    {
        //If the actor can face L or R, change direction appropriately
        if (this.CanLookLeftRight)
        {
            this.FacingRight = !this.FacingRight;
            var theScale = this.transform.localScale;
            theScale.x *= -1;
            this.transform.localScale = theScale;
        }
    }

    protected virtual void DestroyActor()
    {
        var manager = (S_GUI)GameObject.Find("GUI Manager").GetComponent("S_GUI");
        manager.i_score += this.ScoreValue;
        Debug.Log("Hit");
        Destroy(this.gameObject);
    }

    #region Trigger Events
    public virtual void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Wall" || otherObj.tag == "MovingWall")
        {
            this.IsTouchingWall = true;
            //HorizontalSpeed = 0;

            //Check wall position
            this.WallPosition = otherObj.transform.position.x - this.ActorTransform.position.x;

            //Stop player from moving on appropriate side and snap smoothly
            if (this.WallPosition > 0)
            {
                //Debug.Log("Hit Right");
                Snap(otherObj, "Right", 0);
            }
            else if (this.WallPosition < 0)
            {
                //Debug.Log("Hit Left");
                Snap(otherObj, "Left", 0);
            }

            if (otherObj.tag == "MovingWall")
	        {
                this.ActorTransform.parent = otherObj.transform;
                //CurrentVerticalState = VerticalState.Grounded;
                this.IsHanging = true;
                this.CanJump = true;
	        }      
        }
        //Check position of the floor. If the center of the floor 
        //is between the top and bottom of the collider, treat the
        //floor as a wall
        //TODO: check if this also fires when jumping onto a slanted floor
        if (otherObj.tag == "Floor" && this.CurrentVerticalState == VerticalState.Airborn && this.CanDetectFloors)
        {
            if (this.ActorTransform.position.x < otherObj.bounds.min.x || this.ActorTransform.position.x > otherObj.bounds.max.x)
            {
                if (otherObj.bounds.max.y > (this.ActorTransform.collider.bounds.min.y + .125f) && (otherObj.bounds.min.y < this.ActorTransform.collider.bounds.max.y - .125f))
                {
                    this.IsTouchingWall = true;
                    //HorizontalSpeed = 0;

                    //Check wall position
                    this.WallPosition = otherObj.transform.position.x - this.ActorTransform.position.x;

                    //Stop player from moving on appropriate side and snap smoothly
                    if (this.WallPosition > 0)
                    {
                        Snap(otherObj, "Right", 0);
                    }
                    else if (this.WallPosition < 0)
                    {
                        Snap(otherObj, "Left", 0);
                    }
                }
            }
        }
    }

    public virtual void OnTriggerExit(Collider otherObj)
    {
        if (otherObj.tag == "Wall" || otherObj.tag == "Floor")
        {
            //Debug.Log("Not walled");
            this.IsTouchingWall = false;
        }
        if (otherObj.tag == "MovingWall")
        {
            this.IsTouchingWall = false;
            this.IsHanging = false;
            this.ActorTransform.parent = null;
        }
    }
    #endregion

    /// <summary>
    /// Snaps the actor to the given obstacle
    /// </summary>
    /// <param name="obstacleCollider"></param>
    /// <param name="obstaclePosition">String description of where the obstacle is in relation the Actor</param>
    /// <param name="yContactPosition"></param>
    public virtual void Snap(Collider obstacleCollider, string obstaclePosition, float yContactPosition)
    {
        var colliderWidth = collider.bounds.size.x / 2;
        var obstacleWidth = obstacleCollider.bounds.size.x / 2;
        var snapOffsetX = colliderWidth + obstacleWidth;

        var colliderHeight = collider.bounds.size.y / 2;
        var obstacleHeight = obstacleCollider.transform.localScale.y / 2;
        var snapOffsetY = (colliderHeight / Mathf.Cos(this.ActiveFloorRotation)) + obstacleHeight;

        switch (obstaclePosition)
        {
            case "Right":
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x - snapOffsetX, this.ActorTransform.position.y);
                //Debug.Log("Snapping wall");
                break;
            case "Left":
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x + snapOffsetX, this.ActorTransform.position.y);
                //Debug.Log("Snapping wall");
                break;
            case "Floor":
                //Debug object
                if (TestForPostitions)
                {
                    var newMarker = (GameObject)Instantiate(this.MarkerObject, new Vector3(this.ActorTransform.position.x, yContactPosition, -1), Quaternion.identity);
                    this.MarkerObjects.Add(newMarker);
                }
                
                this.ActorTransform.position = new Vector3(this.ActorTransform.position.x, yContactPosition + snapOffsetY);
                break;
            case "Ceiling":
                //Debug.Log(snapOffsetY);
                this.ActorTransform.position = new Vector3(this.ActorTransform.position.x, yContactPosition - snapOffsetY);
                break;
        }
    }

    /// <summary>
    /// Move the actor horizontally.
    /// </summary>
    /// <param name="moveInput">Determines the direction and speed at which the actor moves</param>
    public virtual void MoveHorizontal(float moveInput)
    {
        //Scale the running animation's speed based on player speed
        //spriteAnim.GetAnimation("Lavender_Run").speed = (HorizontalSpeed/MaxHorizontalSpeed);
        //TODO: See if I can do this with animator

        //Manage HorizontalSpeed min and max
        if (this.HorizontalSpeed <= 0)
        {
            this.HorizontalSpeed = 0;
        }
        else if (this.HorizontalSpeed >= this.MaxHorizontalSpeed)
        {
            this.HorizontalSpeed = this.MaxHorizontalSpeed;
        }

        //Change facing direction based on sign of input
        //Face left
        if (moveInput < 0)
        {
            if (FacingRight)
                this.Flip();

            this.Accelerate();

            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                var activeFloor = ActorFoot.GetComponent<S_ActorFoot>().ChooseActiveFloor(this.FacingRight);
                this.ActiveFloorRotation = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);

                spriteAnimator.SetBool("Running", true);
            }
            
            ////If the actor's speed is more then 2/3 the max speed, stop and skid
            //if (HorizontalSpeed >= (MaxHorizontalSpeed * .66f))
            //{
            //    HorizontalSpeed /= 2f;
            //}

        }
        //Face right
        else if (moveInput > 0)
        {
            if (!FacingRight)
                this.Flip();

            this.Accelerate();

            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                var activeFloor = ActorFoot.GetComponent<S_ActorFoot>().ChooseActiveFloor(this.FacingRight);
                this.ActiveFloorRotation = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);

                spriteAnimator.SetBool("Running", true);
            }
        }
        //If moveInput == 0, Decelerate
        else
        {
            //Maintain horizontal momentum while in the air
            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                Decelerate();
            }

            spriteAnimator.SetBool("Running", false);
        }

        //Allow horizontal movement if the actor isn't hanging on a wall
        if (!this.IsHanging)
        {
            //Allow movement in both directions if not against a wall
            if (!this.IsTouchingWall)
            {
                Vector3 test = Quaternion.Euler(0, 0, this.ActiveFloorRotation * Mathf.Rad2Deg) * Vector3.right;
                this.ActorTransform.Translate(test * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
            }
            else //Restrict movement to one side
            {
                //Wall is on the right
                if (this.WallPosition > 0)
                {
                    //Debug.Log("Walled Right");
                    if (moveInput < 0)
                    {
                        this.ActorTransform.Translate(Vector3.right * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
                    }
                }
                //Wall is on the left
                else if (this.WallPosition < 0)
                {
                    //Debug.Log("Walled Left");
                    if (moveInput > 0)
                    {
                        this.ActorTransform.Translate(Vector3.right * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
                    }
                }
            }
        }        
    }

    public virtual void MoveVertical(float moveInput)
    {
        if (this.IsHanging)
        {
            if (moveInput > 0)
            {
                WallJump(this.JumpForce / 1.5f, 75);
            }
            else if (moveInput < 0)
            {
                WallJump(0, 0);
            }
        }
        else
        {
            if (moveInput > 0)
            {
                Jump(this.JumpForce);
            }
            else if (moveInput < 0)
            {
                //Drop down through platform
            }
        }
    }

    protected virtual void Accelerate()
    {
        Debug.Log("Acceling");

        //Accelerate faster on ground
        if (this.CurrentVerticalState == VerticalState.Grounded)
        {
            this.HorizontalSpeed += this.Acceleration;
        }
        //Accelerate slower in the air
        else
        {
            this.HorizontalSpeed += this.Acceleration * (float)0.6666667;
        }
    }

    /// <summary>
    /// Decelerate to zero movement speed. Only decelerate main player if no input received.
    /// </summary>
    protected virtual void Decelerate()
    {
        //Maintain horizontal momentum while in the air
        if (this.CurrentVerticalState == VerticalState.Grounded)
        {
            this.HorizontalSpeed -= 20f;
            if (HorizontalSpeed <= 0)
            {
                HorizontalSpeed = 0;
            }
        }
    }

    /// <summary>
    /// Jump with the specified vertical velocity.
    /// </summary>
    /// <param name='jumpAmount'>
    /// vertical jump amount
    /// </param>
    public void Jump(float jumpAmount)
    {
        //s_animEngine.PlaySpriteAnimation("Lavender_Jump", false);
        

        if (this.CurrentVerticalState == VerticalState.Grounded && this.CanJump)
        {
            this.VerticalSpeed = jumpAmount;
            this.CurrentVerticalState = VerticalState.Airborn;
        }
        //Double Jump
        else if (this.CurrentVerticalState == VerticalState.Airborn && this.CanJump)
        {
            this.VerticalSpeed = jumpAmount * (0.75f);
            this.CanJump = false;
        }
    }

    /// <summary>
    /// Useful for jumping off of a wall or other vertical surface that the actor is attached to
    /// </summary>
    /// <param name="vertJumpAmount">vertical force of the jump</param>
    /// <param name="horzJumpAmount">horizontal movement of the jump. Direction determined by actor facing direction</param>
    public void WallJump(float vertJumpAmount, float horzJumpAmount)
    {
        this.IsHanging = false;
        this.HorizontalSpeed = horzJumpAmount;// *go_actorSprite.transform.localScale.x;
        Jump(vertJumpAmount / 0.75f);
    }


    /// <summary>
    /// Subtract the specified number of hit points from this Actor's health.
    /// </summary>
    /// <param name="damageAmount">Number of hit points to subtract</param>
    public void TakeDamage(int damageAmount)
    {
        if (this.CanBeHit)
        {
            //subtract hit points
            this.HitPoints -= damageAmount;
            //Temporary invincibility
            this.CanBeHit = false;
            //Wait a given time, then make the actor hittable again
            Invoke("CanHit", 1.5f);
        }

        //HP check for death
        if (this.HitPoints <= 0)
        {
            this.CurrentActorState = ActorState.Dead;
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 100, 60), "Clear MarkerObjects"))
        {
            foreach (GameObject marker in this.MarkerObjects)
            {
                Destroy(marker);
            }
        }
    }

}
