using System.Collections.Generic;
using UnityEngine;

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

    private readonly List<GameObject> checkpoints = new List<GameObject>();

    private GameObject activeCheckpoint;

	private Player player;
    private CameraMan cameraMan;

	LevelState currentlevelState;

	// Use this for initialization
	void Start ()
	{
		currentlevelState = LevelState.Start;
		player = GameObject.Find("Player").GetComponent<Player>();
	    cameraMan = Camera.main.GetComponent<CameraMan>();

        //Get all child gameobjects and add them to the checkpoint list
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i).gameObject;
            this.checkpoints.Add(c);
        }
        //Set active checkpoint to first in list
        this.SetActiveCheckpoint(this.checkpoints[0]);
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
                cameraMan.IsFollowingPlayer = false;
                iTween.MoveUpdate(cameraMan.gameObject, new Vector3(player.transform.position.x, 2.5f, -10f), 1f);
			    player.SpriteAnimator.SetBool("Ready Stance",true);
			
			    //CountdownTimer
			    CountdownTimer -= Time.deltaTime;

			    if (CountdownTimer <= 0)
			    {
				    player.CanPlayerMove = true;
                    cameraMan.IsFollowingPlayer = true;
                    player.SpriteAnimator.SetBool("Ready Stance", false);
				    currentlevelState = LevelState.GameLoop;
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
		}
	
	}

    public void SetActiveCheckpoint(GameObject gameObject)
    {
        activeCheckpoint = gameObject;
    }

    public Vector2 GetPlayerSpawnLocation()
    {
        return activeCheckpoint.transform.position;
    }

	public void OnGUI()
	{
		if (currentlevelState == LevelState.Start)
		{
			UnityEngine.GUI.Label(new Rect((Screen.width/2f),(Screen.height/2f) - 70, 200, 200), (Mathf.CeilToInt(CountdownTimer)).ToString());
		}

        if (UnityEngine.GUI.Button(new Rect(50f, 100f, 100f, 50f), "Respawn"))
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.GetPlayerSpawnLocation();
        }

	}
}
