using UnityEngine;
using System.Collections.Generic;

public class S_RunActor : MonoBehaviour
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
	
	public Transform T_actorTransform;
	
	//Animation handlers
	protected exSpriteAnimation spriteAnim;
	public S_AnimationEngine s_animEngine;
	
	//True means this is the main character
	public bool b_Main;
	//Respawn location
	public GameObject obj_respawn;
	//Set the current state as alive
	public ActorState st_actorState = ActorState.Alive;
	
	public VerticalState st_verticalState = VerticalState.Airborn;
	
	
	//Set Hit Points
	public int i_hp = 10;
	//determines whether or not the actor can be hit
	public bool b_canBeHit = true;
	//Score Value
	int i_scoreValue = 1;
	
	#region Horizontal vars
	//Determine whether the actor can turn
	public bool b_canLookLeftRight = true;
	//Determine where the actor is facing
	public bool b_facingRight = false;
	//Determines how fast the player can move
	public float f_actorSpeed;
	//Player Acceleration constant
	public float f_actorAcceleration;
	//Maximum actor speed
	public float f_maxActorSpeed;
	//Stores the position of the wall, L or R
	public float f_wallPos;
	//Is this object up against a wall?
	public bool b_walled = false;
	
	public bool b_canDetectFloors = true;
	
	public float f_yContactPos;
	
	public float f_floorRot;
	#endregion
	
	#region Vertical vars
	//Vertical actor velocity
	public float f_velY;
	
	public float f_maxVelY;
	//Strength of gravity
	public float f_gravity;
	//Can the actor jump?
	public bool b_canJump = true;
	
	public bool b_hanging = false;
	
	public float f_jumpAmount;
	
	
	#endregion
	
	//Children GameObjects
	public GameObject go_actorSprite;
	private Vector3 v3_actorSpriteScale;
	public GameObject go_actorFoot;
	
	//Referenced Scripts
	public S_ActorFoot s_actorFoot;
	
	//Debug object
	public GameObject go_marker;
	public List<GameObject> markers = new List<GameObject>();
	
	public bool TestForPostitions = false;
	
	
	// Use this for initialization
	public virtual void Start()
	{
		//Cache the transform
		T_actorTransform = transform;
		
		//Find and set the actor mesh
		foreach (Transform child in T_actorTransform)
		{
			//The mesh should be the only thing with the renderer 
			if (child.tag == "Sprite")
			{
				go_actorSprite = child.gameObject;
			}
			else if (child.name == "Foot")
			{
				go_actorFoot = child.gameObject;
			}
		}
		
		//Get the initial scale of the mesh
		v3_actorSpriteScale = go_actorSprite.transform.localScale;
		
		//Get the ActorFoot's script
		s_actorFoot = (S_ActorFoot)go_actorFoot.gameObject.GetComponent("S_ActorFoot");
		
		//Set the animation object
		spriteAnim = (exSpriteAnimation)go_actorSprite.GetComponent("exSpriteAnimation");
		
		s_animEngine = (S_AnimationEngine)go_actorSprite.GetComponent("S_AnimationEngine");
		
		
		//Position the foot at the bottom of the actor collider
		go_actorFoot.transform.localPosition = new Vector3(go_actorFoot.transform.localPosition.x, -(T_actorTransform.collider.bounds.size.y / 3), go_actorFoot.transform.localPosition.z);
		
		//If this is the main character, set the respawner
		if (b_Main)
		{
			obj_respawn = GameObject.Find("Spawn");
		}
	}
	
	// Update is called once per frame
	public virtual void Update()
	{
		//ActorState checks
		if (st_actorState == ActorState.Alive)
		{
			//Move the foot to the top or bottom of the actor depending on vertical velocity
			if (f_velY > 0)
			{
				go_actorFoot.transform.localPosition = new Vector3(go_actorFoot.transform.localPosition.x, T_actorTransform.collider.bounds.size.y / 3, go_actorFoot.transform.localPosition.z);
			}
			else if (f_velY < 0)
			{
				s_animEngine.PlaySpriteAnimation("Lavender_Fall", false);
				
				go_actorFoot.transform.localPosition = new Vector3(go_actorFoot.transform.localPosition.x, -(T_actorTransform.collider.bounds.size.y / 3), go_actorFoot.transform.localPosition.z);
			}
			else
			{
				go_actorFoot.transform.localPosition = new Vector3(go_actorFoot.transform.localPosition.x, -(T_actorTransform.collider.bounds.size.y / 3), go_actorFoot.transform.localPosition.z);
			}
			
			if (st_verticalState == VerticalState.Airborn && !b_hanging)
			{
				//Apply gravity
				f_velY -= (f_gravity * Time.fixedDeltaTime);
				T_actorTransform.Translate(Vector3.up * f_velY, Space.World);
				
				if (f_velY < f_maxVelY)
				{
					f_velY = f_maxVelY;
				}
				
				//If you fall below the Death Volume, die or respawn
				//if (T_actorTransform.position.y < obj_deathVolume.transform.position.y)
				//{
				//    st_charState = S_Char.CharState.Dead;
				//}
			}
			
			if (st_verticalState == VerticalState.Grounded)
			{
				//s_animEngine.PlaySpriteAnimation("Lavender_Landing", true);
				
				//Allow the actor to jump again
				f_velY = 0f;
				b_canJump = true;
			}
		}
		
		
		if (st_actorState == ActorState.Dead)
		{
			//If this is the main character, 
			//reset HP, move to respawn, and 
			//change state to alive
			if (b_Main)
			{
				i_hp = 10;
				T_actorTransform.position = obj_respawn.transform.position;
				st_actorState = ActorState.Alive;
			}
			else //Destroy it
			{
				var manager = (S_GUI)GameObject.Find("GUI Manager").GetComponent("S_GUI");
				manager.i_score += i_scoreValue;
				Debug.Log("Hit");
				Destroy(gameObject);
			}
		}
		
		//If the actor can face L or R, change direction appropriately
		if (b_canLookLeftRight)
		{
			//Dirty player facing code
			if (b_facingRight)
			{
				//Debug.Log("facing right");
				go_actorSprite.transform.localScale = new Vector3(v3_actorSpriteScale.x, v3_actorSpriteScale.y, v3_actorSpriteScale.z);
			}
			else
			{
				//Debug.Log("facing left");
				go_actorSprite.transform.localScale = new Vector3(-v3_actorSpriteScale.x, v3_actorSpriteScale.y, v3_actorSpriteScale.z);
			}
		}
	}
	
	#region Trigger Events
	public virtual void OnTriggerEnter(Collider otherObj)
	{
		if (otherObj.tag == "Wall" || otherObj.tag == "MovingWall")
		{
			b_walled = true;
			//HorizontalSpeed = 0;
			
			//Check wall position
			f_wallPos = otherObj.transform.position.x - T_actorTransform.position.x;
			
			//Stop player from moving on appropriate side and snap smoothly
			if (f_wallPos > 0)
			{
				//Debug.Log("Hit Right");
				Snap(otherObj, "Right", 0);
			}
			else if (f_wallPos < 0)
			{
				//Debug.Log("Hit Left");
				Snap(otherObj, "Left", 0);
			}
			
			if (otherObj.tag == "MovingWall")
			{
				T_actorTransform.parent = otherObj.transform;
				//CurrentVerticalState = VerticalState.Grounded;
				b_hanging = true;
				b_canJump = true;
			}      
			
		}
		//Check position of the floor. If the center of the floor 
		//is between the top and bottom of the collider, treat the
		//floor as a wall
		if (otherObj.tag == "Floor" && st_verticalState == VerticalState.Airborn && b_canDetectFloors)
		{
			if (T_actorTransform.position.x < otherObj.bounds.min.x || T_actorTransform.position.x > otherObj.bounds.max.x)
			{
				if (otherObj.bounds.max.y > (T_actorTransform.collider.bounds.min.y + .125f) && (otherObj.bounds.min.y < T_actorTransform.collider.bounds.max.y - .125f))
				{
					b_walled = true;
					//HorizontalSpeed = 0;
					
					//Check wall position
					f_wallPos = otherObj.transform.position.x - T_actorTransform.position.x;
					
					//Stop player from moving on appropriate side and snap smoothly
					if (f_wallPos > 0)
					{
						Snap(otherObj, "Right", 0);
					}
					else if (f_wallPos < 0)
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
			b_walled = false;
		}
		if (otherObj.tag == "MovingWall")
		{
			b_walled = false;
			b_hanging = false;
			T_actorTransform.parent = null;
		}
	}
	#endregion
	
	/// <summary>
	/// Snaps the actor to the given obstacle
	/// </summary>
	/// <param name="obstacleCollider"></param>
	/// <param name="obstaclePos"></param>
	/// <param name="yContactPos"></param>
	public virtual void Snap(Collider obstacleCollider, string obstaclePos, float yContactPos)
	{
		float colliderWidth = collider.bounds.size.x / 2;
		float wallWidth = obstacleCollider.bounds.size.x / 2;
		float snapOffsetX = colliderWidth + wallWidth;
		
		float colliderHeight = collider.bounds.size.y/2;
		float wallHeight = obstacleCollider.transform.localScale.y/2;
		float snapOffsetY = (colliderHeight/Mathf.Cos(f_floorRot)) + wallHeight;
		
		switch (obstaclePos)
		{
		case "Right":
			T_actorTransform.position = new Vector3(obstacleCollider.transform.position.x - snapOffsetX, T_actorTransform.position.y);
			//Debug.Log("Snapping wall");
			break;
		case "Left":
			T_actorTransform.position = new Vector3(obstacleCollider.transform.position.x + snapOffsetX, T_actorTransform.position.y);
			//Debug.Log("Snapping wall");
			break;
		case "Floor":
			//Debug object
			if (TestForPostitions)
			{
				var newMarker = (GameObject)Instantiate(go_marker, new Vector3(T_actorTransform.position.x, yContactPos, -1), Quaternion.identity);
				markers.Add(newMarker);
			}
			
			T_actorTransform.position = new Vector3(T_actorTransform.position.x, yContactPos + snapOffsetY);
			break;
		case "Ceiling":
			//Debug.Log(snapOffsetY);
			T_actorTransform.position = new Vector3(T_actorTransform.position.x, yContactPos - snapOffsetY);
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
		spriteAnim.GetAnimation("Lavender_Run").speed = (f_actorSpeed/f_maxActorSpeed);
		
		//Manage HorizontalSpeed min and max
		if (f_actorSpeed <= 0)
		{
			f_actorSpeed = 0;
		}
		else if (f_actorSpeed >= f_maxActorSpeed)
		{
			f_actorSpeed = f_maxActorSpeed;
		}
		
		//Change facing direction based on sign of input
		//Face left
		if (moveInput < 0)
		{
			b_facingRight = false;
			
			if (st_verticalState == VerticalState.Grounded)
			{
				GameObject activeFloor = s_actorFoot.ChooseActiveFloor(b_facingRight);
				f_floorRot = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);
				
				s_animEngine.PlaySpriteAnimation("Lavender_Run", false);
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
			b_facingRight = true;
			
			if (st_verticalState == VerticalState.Grounded)
			{
				GameObject activeFloor = s_actorFoot.ChooseActiveFloor(b_facingRight);
				f_floorRot = (activeFloor.transform.eulerAngles.z * Mathf.Deg2Rad);
				
				s_animEngine.PlaySpriteAnimation("Lavender_Run", false);
				
			}
		}
		//If moveInput == 0, Decelerate and show Idle animation
		else
		{
			if (st_verticalState == VerticalState.Grounded)
			{
				s_animEngine.PlaySpriteAnimation("Lavender_Idle", false);
				
			}
			Decelerate(b_Main);
		}
		
		//Allow horizontal movement if the actor isn't hanging on a wall
		if (!b_hanging)
		{
			//Allow movement in both directions if not against a wall
			if (!b_walled)
			{
				Vector3 test = Quaternion.Euler(0, 0, f_floorRot * Mathf.Rad2Deg) * Vector3.right;
				T_actorTransform.Translate(test * moveInput * f_actorSpeed * Time.fixedDeltaTime);
			}
			else //Restrict movement to one side
			{
				//Wall is on the right
				if (f_wallPos > 0)
				{
					//Debug.Log("Walled Right");
					if (moveInput < 0)
					{
						T_actorTransform.Translate(Vector3.right * moveInput * f_actorSpeed * Time.fixedDeltaTime);
					}
				}
				//Wall is on the left
				else if (f_wallPos < 0)
				{
					//Debug.Log("Walled Left");
					if (moveInput > 0)
					{
						T_actorTransform.Translate(Vector3.right * moveInput * f_actorSpeed * Time.fixedDeltaTime);
					}
				}
			}
			
			//Accelerate faster on ground
			if (st_verticalState == VerticalState.Grounded)
			{
				f_actorSpeed += f_actorAcceleration;
			}
			//Accelerate slower in the air
			else
			{
				f_actorSpeed += f_actorAcceleration * (float) 0.6666667;
			}
		}        
	}
	
	public virtual void MoveVertical(float moveInput)
	{
		if (b_hanging)
		{
			if (moveInput > 0)
			{
				WallJump(f_jumpAmount / 1.5f, 75);
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
				Jump(f_jumpAmount);
			}
			else if (moveInput < 0)
			{
				//Drop down through platform
			}
		}
	}
	
	
	/// <summary>
	/// Decelerate to zero movement speed. Only decelerate main player if no input received.
	/// </summary>
	/// <param name="main"></param>
	void Decelerate(bool main)
	{
		//Maintain horizontal momentum while in the air
		if (st_verticalState == VerticalState.Grounded)
		{
			if (main)
			{
				//If no horizontal movement key is pushed, Decelerate
				if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
				{
					f_actorSpeed -= 1f;
				}
			}
			else
			{
				f_actorSpeed -= 1f;
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
		s_animEngine.PlaySpriteAnimation("Lavender_Jump", false);
		
		
		if (st_verticalState == VerticalState.Grounded && b_canJump)
		{
			f_velY = jumpAmount;
			st_verticalState = VerticalState.Airborn;
		}
		//Double Jump
		else if (st_verticalState == VerticalState.Airborn && b_canJump)
		{
			f_velY = jumpAmount * (0.75f);
			b_canJump = false;
		}
	}
	
	/// <summary>
	/// Useful for jumping off of a wall or other vertical surface that the actor is attached to
	/// </summary>
	/// <param name="vertJumpAmount">vertical force of the jump</param>
	/// <param name="horzJumpAmount">horizontal movement of the jump. Direction determined by actor facing direction</param>
	public void WallJump(float vertJumpAmount, float horzJumpAmount)
	{
		b_hanging = false;
		f_actorSpeed = horzJumpAmount;// *go_actorSprite.transform.localScale.x;
		Jump(vertJumpAmount / 0.75f);
	}
	
	
	/// <summary>
	/// Subtract the specified number of hit points from this Actor's health.
	/// </summary>
	/// <param name="damageAmount">Number of hit points to subtract</param>
	public void TakeDamage(int damageAmount)
	{
		if (b_canBeHit)
		{
			//subtract hit points
			i_hp -= damageAmount;
			//Temporary invincibility
			b_canBeHit = false;
			//Wait a given time, then make the actor hittable again
			Invoke("CanHit", 1.5f);
		}
		
		//HP check for death
		if (i_hp <= 0)
		{
			st_actorState = ActorState.Dead;
		}
	}
	
	///// <summary>
	///// Play the sprite animation only if it hasn't already been playing.
	///// </summary>
	///// <param name="animationName">The animation to play</param>
	//public void PlaySpriteAnimation(string animationName)
	//{
	//    if (!spriteAnim.IsPlaying(animationName))
	//    {
	//        spriteAnim.Play(animationName);
	//    }
	//}
	
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(50, 50, 100, 60), "Clear MarkerObjects"))
		{
			foreach (GameObject marker in markers)
			{
				Destroy(marker);
			}
		}
	}
	
}
