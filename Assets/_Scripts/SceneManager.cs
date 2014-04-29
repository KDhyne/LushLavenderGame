using System.Collections;
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
	public float Bellcount = 0f; //Used for animating the counting at the end

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
        for (var i = 0; i < transform.childCount; i++)
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



                //If the player passes the last checkpoint in the collection of all checkpoints,
                //change to the end state
		        if (checkpoints.IndexOf(activeCheckpoint) == (checkpoints.Count - 1))
		        {
		            currentlevelState = LevelState.End;
		        }
			    break;

		    case LevelState.End:
		        StartCoroutine(PlayEndSequence());
			    break;
		}
	
	}

    private IEnumerator PlayEndSequence()
    {
        //Stop player and center the camera

        player.CanPlayerMove = false;
		player.MoveHorizontal(0);
        cameraMan.IsFollowingPlayer = false;
        iTween.MoveUpdate(cameraMan.gameObject, new Vector3(player.transform.position.x, 2.5f, -10f), 1f);
		//Add up current bells
		this.Bellcount = Mathf.CeilToInt(iTween.FloatUpdate(this.Bellcount, CurrentSilverBellCount, 0.1f));

        //TODO: Check if the number of bells collected equals the total number in the level. If so, Give a Golden Bell
        yield return new WaitForSeconds(2f);

		//Walk player off the stage
		player.MoveHorizontal(1);
    }

    public void SetActiveCheckpoint(GameObject newActiveCheckpoint)
    {
        activeCheckpoint = newActiveCheckpoint;
    }

    public Vector2 GetPlayerSpawnLocation()
    {
        return activeCheckpoint.transform.position;
    }

	public void OnGUI()
	{
	    switch (this.currentlevelState)
	    {
	        case LevelState.Start:
	            UnityEngine.GUI.Label(new Rect((Screen.width/2f),(Screen.height/2f) - 70, 200, 200), (Mathf.CeilToInt(this.CountdownTimer)).ToString());
	            break;

            case LevelState.GameLoop:
                break;

            case LevelState.End:
                break;
	    }

	    if (UnityEngine.GUI.Button(new Rect(50f, 100f, 100f, 50f), "Respawn"))
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.GetPlayerSpawnLocation();
        }
	}
}
