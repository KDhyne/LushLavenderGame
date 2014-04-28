using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	enum LevelState
	{
		Start,
		GameLoop,
		End
	}

	public int TotalSilverBellCount;
	public int CurrentSilverBellCount;
	public float bellcount = 0f; //Used for animating the counting at the end

	public float CountdownTimer;

	private S_Player player;

	LevelState currentlevelState;

	// Use this for initialization
	void Start ()
	{
		currentlevelState = LevelState.Start;
		player = GameObject.Find("Player").GetComponent<S_Player>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (currentlevelState)
		{
		case LevelState.Start:
			//TODO: Add intro sequence
			//Stop player
			player.CanPlayerMove = false;
			player.SpriteAnimator.SetBool("Ready Stance",true);
			
			//CountdownTimer
			CountdownTimer -= Time.deltaTime;

			if (CountdownTimer <= 0)
			{
				player.CanPlayerMove = true;
				currentlevelState = LevelState.GameLoop;
				player.SpriteAnimator.SetBool("Ready Stance",false);
			}
			break;

		case LevelState.GameLoop:
			//general upkeep things
			break;

		case LevelState.End:
			//Stop player and center the camera
			player.MoveHorizontal(0);
			//Add up current bells over total bells
			bellcount = iTween.FloatUpdate(bellcount,(float)CurrentSilverBellCount, 0.1f);
			//Walk player off the stage
			player.MoveHorizontal(1);
			break;

		default:
			break;
		}
	
	}

	public void OnGUI()
	{
		if (currentlevelState == LevelState.Start)
		{
			GUI.Label(new Rect((Screen.width/2f),(Screen.height/2f) - 70, 200, 200), (Mathf.CeilToInt(CountdownTimer)).ToString());
		}

	}
}
