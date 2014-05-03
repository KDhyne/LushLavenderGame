using UnityEngine;

public class ActorFoot : MonoBehaviour 
{
    //Actor to whom the foot is attached
    private Actor parentActor;

    //Number of contact floors
    private int numFloorsTouching;

    private float contactPositionY;

    float relativeFloorPosition;

    //Stores the floors depending on orientation to actor
    public GameObject LeftFloor;
    public GameObject RightFloor;

    //public bool b_canDetectWalls;

	// Use this for initialization
	void Start () 
    {
        this.parentActor = transform.parent.gameObject.GetComponent<Actor>();
	}

    void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Floor" || otherObj.tag == "MovingFloor")
        {
            //Raise the number of contact Floors by 1
            this.numFloorsTouching++;
			
			//Add the floor to the floor array. This will be used to determine which incline to use for snapping
            this.CheckSurroundingFloors(otherObj);

            //Get the floor's rotation
            this.parentActor.ActiveFloorRotation = (otherObj.transform.eulerAngles.z * Mathf.Deg2Rad);

            this.parentActor.CanDetectFloors = false;

            //Ground the actor and stop vertical velocity
            this.parentActor.CurrentVerticalState = Actor.VerticalState.Grounded;
            this.parentActor.VerticalSpeed = 0;
            
            //Get the center of contact, located at the middle of the platform directly under the actor's feet
            var xDistanceFromFloorCenter = otherObj.transform.position.x - transform.position.x;
            //Debug.Log("xDist = " + f_xDistFromFloorCenter);
            var yDistFromFloorCenter = xDistanceFromFloorCenter * Mathf.Tan(this.parentActor.ActiveFloorRotation);

            this.contactPositionY = otherObj.transform.position.y - yDistFromFloorCenter;

            //Check actor's y position relative to floor
            this.relativeFloorPosition = this.parentActor.ActorTransform.position.y - this.contactPositionY;

            SnapToFloor(otherObj);
        }
    }

    void OnTriggerExit(Collider otherObj)
    {
        //Debug.Log("Exiting floor");
        if (otherObj.tag == "Floor" || otherObj.tag == "MovingFloor")
        {
            //Reduce number of contact floors by 1
            this.numFloorsTouching--;

            if (this.numFloorsTouching == 1)
            {
                if (this.parentActor.FacingRight)
                    this.LeftFloor = this.RightFloor;

                else
                    this.RightFloor = this.LeftFloor;
            }

            //If the number of contact floors is zero, make the actor airborn,
            //set horizontal movement rotation to zero, and unparent its transform
            if (this.numFloorsTouching <= 0)
            {
                this.LeftFloor = null;
                this.RightFloor = null;

                this.parentActor.CurrentVerticalState = Actor.VerticalState.Airborn;
                this.parentActor.CanDetectFloors = true;
                this.parentActor.gameObject.transform.parent = null;
                this.parentActor.ActiveFloorRotation = 0;
            }
        }
    }
	
    /// <summary>
    /// Check where the floor the foot is in contact with lies and determine if it
    /// lies on the left or right of the actor relative to the camera
    /// </summary>
    /// <param name="floorCollider"></param>
	public void CheckSurroundingFloors(Collider floorCollider)
    {
        var xDistFromFloorCenter = floorCollider.transform.position.x - transform.position.x;

        //If both floors are null, assign both Left and Right Floors to avoid null object errors
        if (this.LeftFloor == null && this.RightFloor == null)
        {
            this.LeftFloor = floorCollider.gameObject;
            this.RightFloor = floorCollider.gameObject;
        }

        //If the floor is to the right of the actor 
        if (xDistFromFloorCenter >= 0)
        {
            //If there is no right floor, set it now
            if (this.RightFloor == null)
                this.RightFloor = floorCollider.gameObject;
            
            else //Make the right floor the left floor to make room for the NEW right floor
            {
                if (this.RightFloor.transform.position.x <= transform.position.x)
                    this.LeftFloor = this.RightFloor;

                this.RightFloor = floorCollider.gameObject;
            }
        }
        else //if it's to the left of the actor
        {
            //If there is no left floor, set it now
            if (this.LeftFloor == null)
                this.LeftFloor = floorCollider.gameObject;
            
            else //Make the left floor the right floor to make room for the NEW left floor
            {
                if (this.LeftFloor.transform.position.x > transform.position.x)
                    this.RightFloor = this.LeftFloor;
                
                this.LeftFloor = floorCollider.gameObject;
            }
        }
    }

    /// <summary>
    /// Select a floor based on the orientation of the actor
    /// </summary>
    /// <param name="facingRight">Whether or not the actor is facing right</param>
    /// <returns></returns>
    public GameObject ChooseActiveFloor(bool facingRight)
    {
        //If the actor is only touching 1 floor, use it no matter its orientation
        if (this.numFloorsTouching == 1)
        {
            //Facing right
            if (facingRight)
                return this.RightFloor ?? this.LeftFloor;

            //else
                return this.LeftFloor ?? this.RightFloor;
        }

        //Otherwise use the appropriate floor
        if (this.numFloorsTouching > 1)
        {
            if (facingRight && this.RightFloor != null)
                return this.RightFloor;
            
            if(!facingRight && this.LeftFloor != null)
                return this.LeftFloor;
            
            //else
            Debug.Log("Something is null");
            return this.gameObject;
        }

        //else
        return this.gameObject;
    }

    /// <summary>
    /// Snap the actor to either a floor or ceiling depending on the actor's
    /// position relative to the collided floor
    /// </summary>
    /// <param name="floor">Floor collided with</param>
    void SnapToFloor(Collider floor)
    {
        //if actor is above floor
        if (this.relativeFloorPosition > 0)
        {//TODO: Figure out why this is snapping up if the floor is above the player??
            this.parentActor.Snap(floor, Actor.ObstaclePosition.Floor, this.contactPositionY);
            //Debug.Log("Snapping up");
        }
            
        //else if actor is below floor
        else if (relativeFloorPosition < 0)
        {
            this.parentActor.Snap(floor, Actor.ObstaclePosition.Ceiling, contactPositionY);
            this.parentActor.CurrentVerticalState = Actor.VerticalState.Airborn;
        }

        //inherit a moving floor's momentum
        if (floor.tag == "MovingFloor")
            this.parentActor.gameObject.transform.parent = floor.transform;
    }
}
