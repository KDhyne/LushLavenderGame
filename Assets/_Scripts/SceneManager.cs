using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public enum LevelState
	{
        MainMenu,
		Start,
		GameLoop,
		End
	}

    public LevelState CurrentlevelState;

    public bool StartScreen;
	public int CurrentSilverBellCount;
    public int TotalSilverBellCount;
	public float Bellcount = 0f; //Used for animating the counting at the end
	public float CountdownTimer;
    public bool ShowEndGUI;
    public bool AddBells;
    public bool GoldenBellAwarded;
    public bool ShowTitle;
    public GameObject GoldenBell;
    public GameObject TitleText;

    public GUISkin Skin;

    private readonly List<GameObject> checkpoints = new List<GameObject>();
    private GameObject activeCheckpoint;

	private Player player;
    private CameraMan cameraMan;

    private readonly Dictionary<string, string> levelMap = new Dictionary<string, string> 
    {
        {"Main Menu", "Flowery Fields"},
		{"Flowery Fields", "Superfluous Sierra"},
		{"Superfluous Sierra", "Main Menu"}
	};

	// Use this for initialization
	void Start ()
	{
	    if (StartScreen)
        {
            this.CurrentlevelState = LevelState.MainMenu;
            this.InvokeRepeating("PunchTitle", 16.3f, .4f);
	    }

        else
            this.CurrentlevelState = LevelState.Start;

        SoundManager.SetVolumeMusic(.25f);

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
		switch (this.CurrentlevelState)
		{
            case LevelState.MainMenu:
                //Stop player
		        StartCoroutine(PlayMainMenuSequence());
                break;

		    case LevelState.Start:
			    //Stop player
			    player.CanPlayerMove = false;
		        //cameraMan.IsFollowingPlayer = false;
		        cameraMan.VerticalAdjustment = 0;
			    player.SpriteAnimator.SetBool("Ready Stance",true);
			
			    //CountdownTimer
			    CountdownTimer -= Time.deltaTime;

			    if (CountdownTimer <= 0)
			    {
				    player.CanPlayerMove = true;
                    cameraMan.IsFollowingPlayer = true;
                    player.SpriteAnimator.SetBool("Ready Stance", false);
				    this.CurrentlevelState = LevelState.GameLoop;
			    }
			    break;

		    case LevelState.GameLoop:
			    //general upkeep things
                if (StartScreen)
                {
                    player.MoveHorizontal(1, true);

                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                    {
                        StartCoroutine(StartGame());
                    }
                }

                //If the player passes the last checkpoint in the collection of all checkpoints,
                //change to the end state
		        if (checkpoints.IndexOf(activeCheckpoint) == (checkpoints.Count - 1))
		        {
                        this.CurrentlevelState = LevelState.End;
		        }
		            
			    break;

		    case LevelState.End:
		        StartCoroutine(PlayEndSequence());

		        if (StartScreen)
		        {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                    {
                        StartCoroutine(StartGame());
                    }

		            return;
		        }

                ShowEndGUI = true;
                if (AddBells)
                    this.Bellcount = iTween.FloatUpdate(this.Bellcount, CurrentSilverBellCount, 2f);
                break;
		}
	}

    void PunchTitle()
    {
        iTween.PunchScale(TitleText, Vector3.one * 0.1f, .3f);
    }

    /// <summary>
    /// Used for the main menu
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayMainMenuSequence()
    {
        yield return new WaitForSeconds(3);
        
        this.CurrentlevelState = LevelState.GameLoop;
    }

    /// <summary>
    /// Play the end of level sequence and reward the golden bell if necessary
    /// If this is the main screen, show title GUI
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayEndSequence()
    {
        //Stop player and center the camera

        player.CanPlayerMove = false;
		player.MoveHorizontal(0, false, true);
        cameraMan.IsFollowingPlayer = false;
        iTween.MoveTo(cameraMan.gameObject, new Vector3(player.transform.position.x, player.transform.position.y + 10f, -10f), 1f);

        yield return new WaitForSeconds(1.5f);

        this.player.SpriteAnimator.SetBool("Victory Pose", true);
        this.player.SpriteAnimator.SetBool("Skid", false);

        if (StartScreen)
        {
            ShowTitle = true;
            yield break;
        }

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
        player.SpriteAnimator.SetBool("Victory Pose", false);
		player.MoveHorizontal(1, true);

		yield return new WaitForSeconds(1f);

		cameraMan.TransitionOut();
        
		yield return new WaitForSeconds(3f);
		//Debug.Log(Application.loadedLevelName);
		Application.LoadLevel(levelMap[Application.loadedLevelName]);
    }

    /// <summary>
    /// Used to transition from the start screen to the first level
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGame()
    {
        cameraMan.TransitionOut();

        yield return new WaitForSeconds(3f);
        //Debug.Log(Application.loadedLevelName);
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
	    switch (this.CurrentlevelState)
	    {
	        case LevelState.Start:
                GUI.Label(new Rect((Screen.width / 2f) - 200, (Screen.height / 2f) - 100, 400, 200), Application.loadedLevelName, Skin.GetStyle("Big Label"));
                GUI.Label(new Rect((Screen.width / 2f) - 100, (Screen.height / 2f) - 40, 200, 200), Mathf.CeilToInt(this.CountdownTimer).ToString(), Skin.GetStyle("Big Label"));
	            break;

            case LevelState.GameLoop:
                break;

            case LevelState.End:
	            if (ShowTitle)
	            {
                    iTween.MoveTo(TitleText, iTween.Hash("islocal", true, "position", new Vector3(0f,7f,1f), "time", 1.5f, "easetype", iTween.EaseType.linear));
	                ShowTitle = false;
	            }

	            if (ShowEndGUI)
                    GUI.Label(new Rect((Screen.width / 2f) - 100, 70, 200, 200), "Silver Bells Collected: " + Mathf.CeilToInt(Bellcount) + "/" + this.TotalSilverBellCount, Skin.GetStyle("label"));
                break;
	    }
	}
}