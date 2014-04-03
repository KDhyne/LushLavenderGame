using UnityEngine;
using System.Collections.Generic;

public class S_ActorFoot : MonoBehaviour 
{
    //Actor to whom the foot is attached
    public S_Actor s_actor;

    //Number of contact floors
    public int i_floorCount = 0;

    float f_yContactPos;

    float f_actorFloorPos;

    //Stores the floors depending on orientation to actor
    public GameObject go_leftFloor;
    public GameObject go_rightFloor;

    //public bool b_canDetectWalls;

	// Use this for initialization
	void Start () 
    {
        s_actor = (S_Actor)transform.parent.gameObject.GetComponent("S_Actor");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (i_floorCount >= 3)
        {
            Debug.Log("Aha!!");
        }
	}

    void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Floor" || otherObj.tag == "MovingFloor")
        {
            //Raise the number of contact Floors by 1
            i_floorCount++;

            

            //s_actor.s_animEngine.PlaySpriteAnimation("Lavender_Landing", S_AnimationEngine.AnimEvents.OnComplete);
			
			//Add the floor to the floor array. This will be used to determine which incline to use for snapping
            CheckFloorLR(otherObj);

            //Get the floor's rotation
            s_actor.f_floorRot = (otherObj.transform.eulerAngles.z * Mathf.Deg2Rad);

            

            s_actor.b_canDetectFloors = false;

            //Ground the actor and stop vertical velocity
            s_actor.st_verticalState = S_Actor.VerticalState.Grounded;
            s_actor.f_velY = 0;
            
            //Get the center of contact, located at the middle of the platform directly under the actor's feet
            float f_xDistFromFloorCenter = otherObj.transform.position.x - transform.position.x;
            //Debug.Log("xDist = " + f_xDistFromFloorCenter);
            float f_yDistFromFloorCenter = f_xDistFromFloorCenter * Mathf.Tan(s_actor.f_floorRot);

            f_yContactPos = otherObj.transform.position.y - f_yDistFromFloorCenter;

            //Check actor's y position relative to floor
            f_actorFloorPos = s_actor.T_actorTransform.position.y - f_yContactPos;

            SnapToFloor(otherObj);

            ////Snap smoothly
            ////if actor is above floor
            //if (f_actorFloorPos > 0 && i_floorCount == 1)
            //{
            //    s_actor.Snap(otherObj, "Floor", f_yContactPos);
            //    //Debug.Log("Snapping to " + otherObj.gameObject.name);

            //}
            ////else if actor is below floor
            //else if (f_actorFloorPos < 0 && i_floorCount == 1)
            //{
            //    s_actor.Snap(otherObj, "Ceiling", f_yContactPos);
            //    s_actor.st_verticalState = S_Actor.VerticalState.Airborn;
            //}

            //if (otherObj.tag == "MovingFloor")
            //{
            //    s_actor.gameObject.transform.parent = otherObj.transform;
            //}
        }
    }

    void OnTriggerExit(Collider otherObj)
    {
        if (otherObj.tag == "Floor" || otherObj.tag == "MovingFloor")
        {
            //Reduce number of contact floors by 1
            i_floorCount--;

            if (i_floorCount == 1)
            {
                if (s_actor.b_facingRight)
                {
                    go_leftFloor = go_rightFloor;
                    //SnapToFloor(go_rightFloor.collider);
                }
                else
                {
                    go_rightFloor = go_leftFloor;
                    //SnapToFloor(go_leftFloor.collider);
                }
            }


            //If the number of contact floors is zero, make the actor airborn,
            //set horizontal movement rotation to zero, and unparent its transform
            if (i_floorCount <= 0)
            {
                go_leftFloor = null;
                go_rightFloor = null;

                s_actor.st_verticalState = S_Actor.VerticalState.Airborn;
                s_actor.b_canDetectFloors = true;
                s_actor.gameObject.transform.parent = null;
                s_actor.f_floorRot = 0;
            }
        }
    }
	
	public void CheckFloorLR(Collider floorCollider)
    {
        float xDistFromFloorCenter = floorCollider.transform.position.x - transform.position.x;

        //If the floor is to the right of the actor 
        if (xDistFromFloorCenter >= 0)
        {
            //If there is no right floor, set it now
            if (go_rightFloor == null)
            {
                go_rightFloor = floorCollider.gameObject;
            }
            else //Make the right floor the left floor to make room for the NEW right floor
            {
                if (go_rightFloor.transform.position.x <= transform.position.x)
                {
                    go_leftFloor = go_rightFloor;
                }

                
                go_rightFloor = floorCollider.gameObject;
            }
        }
        else //if it's to the left of the actor
        {
            //If there is no left floor, set it now
            if (go_leftFloor == null)
            {
                go_leftFloor = floorCollider.gameObject;
            }
            else //Make the left floor the right floor to make room for the NEW left floor
            {
                if (go_leftFloor.transform.position.x > transform.position.x)
                {
                    go_rightFloor = go_leftFloor;
                }

                
                go_leftFloor = floorCollider.gameObject;
            }
        }
    }

    public GameObject ChooseActiveFloor(bool facingRight)
    {
        //If the actor is only touching 1 floor, use it no matter its orientation
        if (i_floorCount == 1)
        {
            //Facing right
            if (s_actor.b_facingRight )
            {
                if (go_rightFloor == null)
                {
                    return go_leftFloor;
                }
                else
                {
                    return go_rightFloor;
                }
            }
            else //Facing left
            {
                if (go_leftFloor == null)
                {
                    return go_rightFloor;
                }
                else
                {
                    return go_leftFloor;
                }
            }
        }
        //Otherwise use the appropriate floor
        else if (i_floorCount > 1)
        {
            if (facingRight && go_rightFloor != null)
            {
                return go_rightFloor;
            }
            else if(!facingRight && go_leftFloor != null)
            {
                return go_leftFloor;
            }
            else
            {
                Debug.Log("Something is null");
                return this.gameObject;
            }
        }
        else
        {
            //Debug.Log("floorcount is more than 2");
            return this.gameObject;
        }
    }

    void SnapToFloor(Collider floor)
    {
        //Snap smoothly
        //if actor is above floor
        if (f_actorFloorPos > 0)
        {
            s_actor.Snap(floor, "Floor", f_yContactPos);
            //Debug.Log("Snapping to " + otherObj.gameObject.name);

        }
        //else if actor is below floor
//        else if (f_actorFloorPos < 0)
//        {
//            s_actor.Snap(floor, "Ceiling", f_yContactPos);
//            s_actor.st_verticalState = S_Actor.VerticalState.Airborn;
//        }

        if (floor.tag == "MovingFloor")
        {
            s_actor.gameObject.transform.parent = floor.transform;
        }
    }

}
