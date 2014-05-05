﻿using System.Collections;
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

    LevelState currentlevelState;

	public int CurrentSilverBellCount;
	public float Bellcount = 0f; //Used for animating the counting at the end
	public float CountdownTimer;
    public bool ShowEndGUI;
    public bool AddBells;
    public bool GoldenBellAwarded;
    public GameObject GoldenBell;

    public int TotalSilverBellCount;

    private readonly List<GameObject> checkpoints = new List<GameObject>();
    private GameObject activeCheckpoint;

	private Player player;
    private CameraMan cameraMan;

	Dictionary<string, string> levelMap = new Dictionary<string, string>()
	{
		{"1_Flowery Fields", "2_Superfluous Sierra"},
		{"2_Superfluous Sierra", "0_Main Menu"}
	};

	// Use this for initialization
	void Start ()
	{
		currentlevelState = LevelState.Start;
		player = GameObject.Find("Player").GetComponent<Player>();
	    cameraMan = Camera.main.GetComponent<CameraMan>();

        //Get the total silver bell length depending on the number in the scene
	    //this.TotalSilverBellCount = FindObjectsOfType<SilverBell>().Length;

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
		            currentlevelState = LevelState.End;
			    break;

		    case LevelState.End:
		        StartCoroutine(PlayEndSequence());
                ShowEndGUI = true;
                if (AddBells)
                    this.Bellcount = iTween.FloatUpdate(this.Bellcount, CurrentSilverBellCount, 2f);
                break;
		}
	}

    private IEnumerator PlayEndSequence()
    {
        //Stop player and center the camera

        player.CanPlayerMove = false;
		player.MoveHorizontal(0, false);
        cameraMan.IsFollowingPlayer = false;
        iTween.MoveTo(cameraMan.gameObject, new Vector3(player.transform.position.x, player.transform.position.y + 10f, -10f), 1f);

        yield return new WaitForSeconds(1.5f);

		//Allow Update to add up current bells
        AddBells = true;
		
        yield return new WaitForSeconds(3f);

        if (CurrentSilverBellCount == TotalSilverBellCount && !GoldenBellAwarded)
        {
            Instantiate(this.GoldenBell, this.player.transform.position + new Vector3(0, 12, 0), Quaternion.identity);
            GoldenBellAwarded = true;
        }

        yield return new WaitForSeconds(2f);

		//Walk player off the stage
		player.MoveHorizontal(1, true);

		yield return new WaitForSeconds(1f);

		cameraMan.TransitionOut();

		yield return new WaitForSeconds(2f);
		Debug.Log(Application.loadedLevelName);
		Application.LoadLevel(levelMap[Application.loadedLevelName]);
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
	            GUI.Label(new Rect((Screen.width/2f),(Screen.height/2f) - 70, 200, 200), Mathf.CeilToInt(this.CountdownTimer).ToString());
	            break;

            case LevelState.GameLoop:
                break;

            case LevelState.End:
	            if (ShowEndGUI)
	            {
	                GUI.Label(new Rect((Screen.width/2f),(Screen.height/2f) - 70, 200, 200), "Silver Bells Collected: " + Mathf.CeilToInt(Bellcount) + "/" + this.TotalSilverBellCount);
	            }
                break;
	    }

	    if (GUI.Button(new Rect(50f, 100f, 100f, 50f), "Respawn"))
            GameObject.FindGameObjectWithTag("Player").transform.position = this.GetPlayerSpawnLocation();
	}
}
