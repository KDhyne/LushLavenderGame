using System;
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

    public enum ObstaclePosition
    {
        Left,
        Right,
        Floor,
        Ceiling
    }

    public VerticalState CurrentVerticalState = VerticalState.Airborn;

    #region Horizontal vars
    protected bool CanLookLeftRight = true;
    public bool FacingRight = true;
    private int latestInputValue = 0;
    public float HorizontalSpeed;
    public float Acceleration;
    public float Deceleration;
    public float MaxHorizontalSpeed;
    public float WallPosition;
    public bool IsTouchingWall = false;
    public bool IsTouchingFloorEdge = false;
    public bool CanDetectFloors = true;
    public float ContactPositionY;
    public float ActiveFloorRotation;
    #endregion

    #region Vertical vars
    public float VerticalSpeed;
    public float MaxVerticalSpeed;
    public float Gravity;
    public bool CanJump = true;
    public bool IsHanging = false;
    public float JumpForce;
    #endregion

    //Children GameObjects
    public GameObject ActorFoot;

    //Colliders
    private CapsuleCollider standardCollider;
    private BoxCollider slidingCollider;
    public Collider CurrentCollider;

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

        //Find and set colliders
        standardCollider = this.gameObject.GetComponent<CapsuleCollider>();
        slidingCollider = this.gameObject.GetComponent<BoxCollider>();
        CurrentCollider = standardCollider;

        //Position the foot at the bottom of the actor collider
        this.ActorFoot.transform.localPosition = new Vector3(
            this.ActorFoot.transform.localPosition.x,
            -(this.standardCollider.bounds.size.y / 2),
            this.ActorFoot.transform.localPosition.z);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Return if not alive
        if (this.CurrentActorState != ActorState.Alive)
            return;

        //Manage actorfoot position
        if (this.VerticalSpeed > 0)
            this.ActorFoot.transform.localPosition = new Vector3(
                this.ActorFoot.transform.localPosition.x,
                this.standardCollider.bounds.size.y / 2,
                this.ActorFoot.transform.localPosition.z);
        else
            this.ActorFoot.transform.localPosition = new Vector3(
                this.ActorFoot.transform.localPosition.x,
                -(this.standardCollider.bounds.size.y / 2),
                this.ActorFoot.transform.localPosition.z);

        //Apply airborn properties and gravity
        if (this.CurrentVerticalState == VerticalState.Airborn && !this.IsHanging)
        {
            this.VerticalSpeed -= (this.Gravity * Time.fixedDeltaTime);
            this.ActorTransform.Translate(Vector3.up * this.VerticalSpeed, Space.World);

            if (this.VerticalSpeed < this.MaxVerticalSpeed)
                this.VerticalSpeed = this.MaxVerticalSpeed;

            this.SpriteAnimator.SetFloat("vSpeed", this.VerticalSpeed * 10);
            this.SpriteAnimator.SetBool("Grounded", false);
        }

        //Apply ground properties
        if (this.CurrentVerticalState == VerticalState.Grounded)
        {
            //Allow the actor to jump again
            this.VerticalSpeed = 0f;
            this.CanJump = true;
            this.SpriteAnimator.SetBool("Grounded", true);
        }
    }

    /// <summary>
    /// Change the actor's local x scale to opposite values.
    /// </summary>
    void Flip()
    {
        //If the actor can't change direction, return
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
                Snap(otherObj, ObstaclePosition.Right, 0);

            else if (this.WallPosition < 0)
                Snap(otherObj, ObstaclePosition.Left, 0);

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

                    //Check wall position
                    this.WallPosition = otherObj.transform.position.x - this.ActorTransform.position.x;

                    //Stop player from moving on appropriate side and snap smoothly
                    if (this.WallPosition > 0)
                        Snap(otherObj, ObstaclePosition.Right, 0);

                    else if (this.WallPosition < 0)
                        Snap(otherObj, ObstaclePosition.Left, 0);
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

        else
            this.IsTouchingFloorEdge = false;
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
    public virtual void Snap(Collider obstacleCollider, ObstaclePosition obstaclePosition, float yContactPosition)
    {
        var colliderWidth = GetComponent<CapsuleCollider>().bounds.size.x / 2;
        var obstacleWidth = obstacleCollider.bounds.size.x / 2;
        var snapOffsetX = colliderWidth + obstacleWidth;

        var colliderHeight = GetComponent<CapsuleCollider>().bounds.size.y / 2;
        var obstacleHeight = obstacleCollider.transform.localScale.y / 2;
        var snapOffsetY = (colliderHeight / Mathf.Cos(this.ActiveFloorRotation)) + obstacleHeight;

        switch (obstaclePosition)
        {
            case ObstaclePosition.Right:
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x - snapOffsetX, this.ActorTransform.position.y);
                break;

            case ObstaclePosition.Left:
                this.ActorTransform.position = new Vector3(obstacleCollider.transform.position.x + snapOffsetX, this.ActorTransform.position.y);
                break;

            case ObstaclePosition.Floor:
                //Debug object
                if (TestForPostitions)
                {
                    var newMarker = (GameObject)Instantiate(this.MarkerObject, new Vector3(this.ActorTransform.position.x, yContactPosition, -1), Quaternion.identity);
                    this.MarkerObjects.Add(newMarker);
                }
                
                this.ActorTransform.position = new Vector3(this.ActorTransform.position.x, yContactPosition + snapOffsetY);
                break;

            case ObstaclePosition.Ceiling:
                //Debug.Log(snapOffsetY);
                this.ActorTransform.position = new Vector3(this.ActorTransform.position.x, yContactPosition - snapOffsetY);
                break;
        }
    }

    /// <summary>
    /// Move the actor horizontally.
    /// </summary>
    /// <param name="moveInput">Determines the direction at which the actor moves</param>
    /// <param name="setInput">True will cause the player to flip if a different signed value is given</param>
    public virtual void MoveHorizontal(int moveInput, bool setInput)
    {
        //Scale the running animation's speed based on player speed
        //spriteAnim.GetAnimation("Lavender_Run").speed = (HorizontalSpeed/MaxHorizontalSpeed);
        //TODO: See if I can scale animation speed with animator

        if (setInput)
            latestInputValue = moveInput;
        
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
        }

        //Allow horizontal movement if the actor isn't hanging on a wall
        if (this.IsHanging)
            return;

        //Allow movement in both directions if not against a wall
        if (!this.IsTouchingWall && !this.IsTouchingFloorEdge)
        {
            Vector3 test = Quaternion.Euler(0, 0, this.ActiveFloorRotation * Mathf.Rad2Deg) * Vector3.right;
            this.ActorTransform.Translate(test * this.HorizontalSpeed * latestInputValue * Time.fixedDeltaTime);
        }
        else //Restrict movement to one side
        {
            //Wall is on the right
            if (this.WallPosition > 0)
            {
                if (moveInput < 0)
                    this.ActorTransform.Translate(Vector3.right * this.HorizontalSpeed * latestInputValue * Time.fixedDeltaTime);
            }
                //Wall is on the left
            else if (this.WallPosition < 0)
            {
                if (moveInput > 0)
                    this.ActorTransform.Translate(Vector3.right * this.HorizontalSpeed * latestInputValue * Time.fixedDeltaTime);
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

        if (this.HorizontalSpeed >= this.MaxHorizontalSpeed)
            this.HorizontalSpeed = this.MaxHorizontalSpeed;
    }

    /// <summary>
    /// Decelerate to zero movement speed. Only decelerate main player if no input received.
    /// </summary>
    protected virtual void Decelerate()
    {
        //Maintain horizontal momentum while in the air
        if (this.CurrentVerticalState == VerticalState.Grounded)
        {
            this.HorizontalSpeed -= this.Deceleration;
        }

        if (this.HorizontalSpeed <= 0)
        {
            this.HorizontalSpeed = 0;
            this.SpriteAnimator.SetBool("Running", false);
            this.SpriteAnimator.SetBool("Sliding", false);
        }
    }

    /// <summary>
    /// Move the actor vertically.
    /// </summary>
    /// <param name="moveInput">Determines the direction in which the actor moves</param>
    public virtual void MoveVertical(int moveInput)
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
            if (moveInput < 0 && CurrentVerticalState == VerticalState.Grounded)
            {
                if (Math.Abs(this.HorizontalSpeed) > 0.01f)
                {
                    //Slide
                    CurrentCollider = slidingCollider;
                    SpriteAnimator.SetBool("Sliding", true);
                }

                else
                    CurrentCollider = standardCollider;
            }
            else
            {
                if (moveInput > 0)
                    Jump(this.JumpForce);

                CurrentCollider = standardCollider;

                if (moveInput == 0)
                    SpriteAnimator.SetBool("Sliding", false);
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
        if (this.CurrentVerticalState == VerticalState.Grounded && this.CanJump)
        {
            VerticalSpeed = 0;
            this.VerticalSpeed = jumpAmount;
            this.CurrentVerticalState = VerticalState.Airborn;
        }
        //Double Jump
        else if (this.CurrentVerticalState == VerticalState.Airborn && this.CanJump)
        {
            VerticalSpeed = 0;
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
