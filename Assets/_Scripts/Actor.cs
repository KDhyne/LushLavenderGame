using System.Collections;

using UnityEngine;
using System.Collections.Generic;

public class Actor : ActorBase
{
    public enum VerticalState
    {
        Airborn,
        Landing,
        Grounded
    }

    public VerticalState CurrentVerticalState = VerticalState.Airborn;

    #region Horizontal vars
    protected bool CanLookLeftRight = true;
    public bool FacingRight = true;
    public float HorizontalSpeed;
    public float Acceleration;
    public float MaxHorizontalSpeed;
    public float WallPosition;
    public bool IsTouchingWall = false;
    public bool IsTouchingFloorEdge = false;
    public bool CanDetectFloors = true;
    public float ContactPositionY;
    public float ActiveFloorRotation;
    #endregion

    #region Vertical vars
    //TODO: Animation needs some work. Either here or in the animator window
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
    public override void Start()
    {
        base.Start();

        //Find and set the actor mesh
        foreach (Transform child in this.ActorTransform)
        {
            if (child.name == "Foot")
                this.ActorFoot = child.gameObject;
        }

        //Position the foot at the bottom of the actor collider
        this.ActorFoot.transform.localPosition = new Vector3(
            this.ActorFoot.transform.localPosition.x, 
            -(this.ActorTransform.GetComponent<CapsuleCollider>().bounds.size.y / 2),
            this.ActorFoot.transform.localPosition.z);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //ActorState checks
        if (this.CurrentActorState == ActorState.Alive)
        {
            if (this.VerticalSpeed > 0)
            {
                this.ActorFoot.transform.localPosition = new Vector3(
                    this.ActorFoot.transform.localPosition.x,
                    this.ActorTransform.GetComponent<CapsuleCollider>().bounds.size.y / 2,
                    this.ActorFoot.transform.localPosition.z);
            }
            else
            {
                this.ActorFoot.transform.localPosition = new Vector3(
                    this.ActorFoot.transform.localPosition.x,
                    -(this.ActorTransform.GetComponent<CapsuleCollider>().bounds.size.y / 2),
                    this.ActorFoot.transform.localPosition.z);
            }

            if (this.CurrentVerticalState == VerticalState.Airborn && !this.IsHanging)
            {
                //Apply gravity
                this.VerticalSpeed -= (this.Gravity * Time.fixedDeltaTime);
                this.ActorTransform.Translate(Vector3.up * this.VerticalSpeed, Space.World);

                if (this.VerticalSpeed < this.MaxVerticalSpeed)
                    this.VerticalSpeed = this.MaxVerticalSpeed;

                //If you fall below the Death Volume, die or respawn
                //if (T_actorTransform.position.y < obj_deathVolume.transform.position.y)
                //{
                //    st_charState = S_Char.CharState.Dead;
                //}

                this.SpriteAnimator.SetFloat("vSpeed", VerticalSpeed * 10);
                this.SpriteAnimator.SetBool("Grounded", false);
            }

            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                //Allow the actor to jump again
                this.VerticalSpeed = 0f;
                this.CanJump = true;
                this.SpriteAnimator.SetBool("Grounded", true);
            }
        }
    }

    /// <summary>
    /// Change the actor's local x scale to opposite values.
    /// </summary>
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
                Snap(otherObj, "Right", 0);

            else if (this.WallPosition < 0)
                Snap(otherObj, "Left", 0);

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
        if (otherObj.tag == "Floor" && this.CurrentVerticalState == VerticalState.Airborn && this.CanDetectFloors)
        {
            if (this.ActorTransform.position.x < otherObj.bounds.min.x || this.ActorTransform.position.x > otherObj.bounds.max.x)
            {
                if (otherObj.bounds.max.y > (this.ActorTransform.GetComponent<CapsuleCollider>().bounds.min.y + .125f)
                    && otherObj.bounds.min.y < (this.ActorTransform.GetComponent<CapsuleCollider>().bounds.max.y - .125f))
                {
                    this.IsTouchingFloorEdge = true;
                    //HorizontalSpeed = 0;

                    //Check wall position
                    this.WallPosition = otherObj.transform.position.x - this.ActorTransform.position.x;

                    //Stop player from moving on appropriate side and snap smoothly
                    if (this.WallPosition > 0)
                        Snap(otherObj, "Right", 0);

                    else if (this.WallPosition < 0)
                        Snap(otherObj, "Left", 0);
                }
            }
        }
    }

    public virtual void OnTriggerStay(Collider otherObj)
    {
        if (otherObj.tag == "Wall")
            this.IsTouchingWall = true;

        if (otherObj.tag == "Floor" && this.CanDetectFloors)
            this.IsTouchingFloorEdge = true;
    }

    public virtual void OnTriggerExit(Collider otherObj)
    {
        if (otherObj.tag == "Wall")
            this.IsTouchingWall = false;

        if (otherObj.tag == "Floor" && this.CanDetectFloors)
            this.IsTouchingFloorEdge = false;

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
        var colliderWidth = GetComponent<CapsuleCollider>().bounds.size.x / 2;
        var obstacleWidth = obstacleCollider.bounds.size.x / 2;
        var snapOffsetX = colliderWidth + obstacleWidth;

        var colliderHeight = GetComponent<CapsuleCollider>().bounds.size.y / 2;
        var obstacleHeight = obstacleCollider.transform.localScale.y / 2;
        var snapOffsetY = (colliderHeight / Mathf.Cos(this.ActiveFloorRotation)) + obstacleHeight;

        switch (obstaclePosition)
        {
            case "Right":
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x - snapOffsetX, this.ActorTransform.position.y);
                break;
            case "Left":
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x + snapOffsetX, this.ActorTransform.position.y);
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
        //TODO: See if I can scale animation speed with animator

        //Manage HorizontalSpeed min and max
        if (this.HorizontalSpeed <= 0)
            this.HorizontalSpeed = 0;

        else if (this.HorizontalSpeed >= this.MaxHorizontalSpeed)
            this.HorizontalSpeed = this.MaxHorizontalSpeed;
        
        if (moveInput < 0)
        {
            if (FacingRight)
                this.Flip();

            this.Accelerate();

            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                var activeFloor = ActorFoot.GetComponent<ActorFoot>().ChooseActiveFloor(this.FacingRight);
                this.ActiveFloorRotation = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);

                this.SpriteAnimator.SetBool("Running", true);
            }
            
            ////If the actor's speed is more then 2/3 the max speed, stop and skid
            //if (HorizontalSpeed >= (MaxHorizontalSpeed * .66f))
            //{
            //    HorizontalSpeed /= 2f;
            //}

        }
        else if (moveInput > 0)
        {
            if (!FacingRight)
                this.Flip();

            this.Accelerate();

            if (this.CurrentVerticalState == VerticalState.Grounded)
            {
                var activeFloor = ActorFoot.GetComponent<ActorFoot>().ChooseActiveFloor(this.FacingRight);
                this.ActiveFloorRotation = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);

                this.SpriteAnimator.SetBool("Running", true);
            }
        }
        //If moveInput == 0, Decelerate
        else
        {
            //Maintain horizontal momentum while in the air
            if (this.CurrentVerticalState == VerticalState.Grounded)
                Decelerate();

            this.SpriteAnimator.SetBool("Running", false);
        }

        //Allow horizontal movement if the actor isn't hanging on a wall
        if (!this.IsHanging)
        {
            //Allow movement in both directions if not against a wall
            if (!this.IsTouchingWall && !this.IsTouchingFloorEdge)
            {
                Vector3 test = Quaternion.Euler(0, 0, this.ActiveFloorRotation * Mathf.Rad2Deg) * Vector3.right;
                this.ActorTransform.Translate(test * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
            }
            else //Restrict movement to one side
            {
                //Wall is on the right
                if (this.WallPosition > 0)
                {
                    if (moveInput < 0)
                        this.ActorTransform.Translate(Vector3.right * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
                }
                //Wall is on the left
                else if (this.WallPosition < 0)
                {
                    if (moveInput > 0)
                        this.ActorTransform.Translate(Vector3.right * moveInput * this.HorizontalSpeed * Time.fixedDeltaTime);
                }
            }
        }        
    }

    /// <summary>
    /// Move the actor vertically.
    /// </summary>
    /// <param name="moveInput">Determines the direction in which the actor moves</param>
    public virtual void MoveVertical(float moveInput)
    {
        if (this.IsHanging)
        {
            if (moveInput > 0)
                WallJump(this.JumpForce / 1.5f, 75);

            else if (moveInput < 0)
                WallJump(0, 0);
        }
        else
        {
            if (moveInput > 0)
                Jump(this.JumpForce);
            
            else if (moveInput < 0)
            {
                //Drop down through platform
            }
        }
    }

    /// <summary>
    /// Accelerate actor horizontally determined by ground state.
    /// </summary>
    protected virtual void Accelerate()
    {
        //Accelerate faster on ground
        if (this.CurrentVerticalState == VerticalState.Grounded)
            this.HorizontalSpeed += this.Acceleration;
        
        //Accelerate slower in the air
        else
            this.HorizontalSpeed += this.Acceleration * 0.67f;
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
                HorizontalSpeed = 0;
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
        VerticalSpeed = 0;

        if (this.CurrentVerticalState == VerticalState.Grounded && this.CanJump)
        {
            this.VerticalSpeed = jumpAmount;
            this.CurrentVerticalState = VerticalState.Airborn;
        }
        //Double Jump
        else if (this.CurrentVerticalState == VerticalState.Airborn && this.CanJump)
        {
            this.VerticalSpeed = jumpAmount;// * (0.9f);
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

    //Draw the GUI every frame
    void OnGUI()
    {
        /*if (GUI.Button(new Rect(50, 50, 100, 60), "Clear MarkerObjects"))
        {
            foreach (var marker in this.MarkerObjects)
            {
                Destroy(marker);
            }
        }*/
    }
}
