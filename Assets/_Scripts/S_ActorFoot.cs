using UnityEngine;

public class S_ActorFoot : MonoBehaviour 
{
    //Actor to whom the foot is attached
    private S_Actor parentActor;

    //Number of contact floors
    private int numFloorsTouching;

    private float contactPositionY;

    float relativeFloorPosition;

    //Stores the floors depending on orientation to actor
    public GameObject leftFloor;
    public GameObject rightFloor;

    //public bool b_canDetectWalls;

	// Use this for initialization
	void Start () 
    {
        this.parentActor = transform.parent.gameObject.GetComponent<S_Actor>();
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
            this.parentActor.CurrentVerticalState = S_Actor.VerticalState.Grounded;
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
        if (otherObj.tag == "Floor" || otherObj.tag == "MovingFloor")
        {
            //Reduce number of contact floors by 1
            this.numFloorsTouching--;

            if (this.numFloorsTouching == 1)
            {
                if (this.parentActor.FacingRight)
                {
                    this.leftFloor = this.rightFloor;
                    //SnapToFloor(rightFloor.collider);
                }
                else
                {
                    this.rightFloor = this.leftFloor;
                    //SnapToFloor(leftFloor.collider);
                }
            }

            //If the number of contact floors is zero, make the actor airborn,
            //set horizontal movement rotation to zero, and unparent its transform
            if (this.numFloorsTouching <= 0)
            {
                this.leftFloor = null;
                this.rightFloor = null;

                this.parentActor.CurrentVerticalState = S_Actor.VerticalState.Airborn;
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

        //If the floor is to the right of the actor 
        if (xDistFromFloorCenter >= 0)
        {
            //If there is no right floor, set it now
            if (this.rightFloor == null)
            {
                this.rightFloor = floorCollider.gameObject;
            }
            else //Make the right floor the left floor to make room for the NEW right floor
            {
                if (this.rightFloor.transform.position.x <= transform.position.x)
                {
                    this.leftFloor = this.rightFloor;
                }

                this.rightFloor = floorCollider.gameObject;
            }
        }
        else //if it's to the left of the actor
        {
            //If there is no left floor, set it now
            if (this.leftFloor == null)
            {
                this.leftFloor = floorCollider.gameObject;
            }
            else //Make the left floor the right floor to make room for the NEW left floor
            {
                if (this.leftFloor.transform.position.x > transform.position.x)
                {
                    this.rightFloor = this.leftFloor;
                }
                
                this.leftFloor = floorCollider.gameObject;
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
                return this.rightFloor ?? this.leftFloor;
            else
                return this.leftFloor ?? this.rightFloor;
        }
        //Otherwise use the appropriate floor
        if (this.numFloorsTouching > 1)
        {
            if (facingRight && this.rightFloor != null)
            {
                return this.rightFloor;
            }
            if(!facingRight && this.leftFloor != null)
            {
                return this.leftFloor;
            }
            else
            {
                Debug.Log("Something is null");
                return this.gameObject;
            }
        }
        else
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
        {
            this.parentActor.Snap(floor, "Floor", this.contactPositionY);
            //Debug.Log("Snapping to " + otherObj.gameObject.name);
        }
        //else if actor is below floor
        else if (relativeFloorPosition < 0)
        {
            this.parentActor.Snap(floor, "Ceiling", contactPositionY);
            this.parentActor.CurrentVerticalState = S_Actor.VerticalState.Airborn;
        }

        if (floor.tag == "MovingFloor")
        {
            this.parentActor.gameObject.transform.parent = floor.transform;
        }
    }
}
