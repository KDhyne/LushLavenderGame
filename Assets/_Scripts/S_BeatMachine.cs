using UnityEngine;
using System.Collections.Generic;

public class S_BeatMachine : MonoBehaviour 
{
	
	public int i_measure;
	public float f_beat;
	public int i_bpm;
	public int i_timeSignatureNumerator;
	public int i_noteDivision;
	
	public const float f_errorAllowance = 0.075f;
	
	public List<int> beatList;
	public GameObject[] go_vizualizerMarkers;
    public int i_correctBeats = 0;
	
	private float f_initialTime;
	private bool b_beatFired = false;
    private bool b_pulseFired = false;
    private float timer = 0f;
	
	public AudioSource SongAudio;

    //Debug object
    public GameObject go_marker;
    public List<GameObject> markers = new List<GameObject>();

    public GameObject PlayerGameObject;

    public bool TestForPostitions = false;

	// Use this for initialization
	void Start () 
	{
		// Get the song
		SongAudio = (AudioSource)GetComponent("AudioSource");
		
		//beatArray = new S_Beat[];

	    if (TestForPostitions)
	    {
	        PlayerGameObject = GameObject.Find("_Player");
	    }
		
		// Set bpm and time signature
		i_bpm = 149;
		i_timeSignatureNumerator = 4;
		i_noteDivision = 2;
		
		// Start at the beginning of the song
		i_measure = 0;
		f_beat = 0;
		
		// Get the time the Beat Machine was created
		f_initialTime = Time.time;
	}
	
	// FixedUpdate is called once per time tick
	// This fires off every 5 milliseconds
	void FixedUpdate () 
	{
		// Get the current song time with time correction (may not need later)
		float songPosition = SongAudio.time - f_initialTime;		
		
		// Calculate the number of measures and beats
		i_measure = Mathf.FloorToInt((songPosition / (60f / (i_bpm * i_noteDivision))) / (i_timeSignatureNumerator * i_noteDivision));
		f_beat = (songPosition / (60f / (i_bpm * i_noteDivision))) % (i_timeSignatureNumerator * i_noteDivision);
		
		// If a pulse can be fired, 
        if (!b_pulseFired)
        {
            CheckForPulse();
        }

		// Timer to reset the pulse trigger
        if (b_pulseFired)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= (30f / (i_bpm * i_noteDivision))/SongAudio.pitch)
            {
                b_pulseFired = false;
                timer = 0f;
            }
        }
		
		// Timer to reset the input trigger
        if (b_beatFired)
        {
            timer += Time.fixedDeltaTime;

            if (timer >= (30f / (i_bpm * i_noteDivision)) / SongAudio.pitch)
            {
                b_beatFired = false;
                timer = 0f;
            }
        }
	}

    void Update()
    {
        //Check for input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!b_beatFired)
            {
                CheckInputAccuracy();
            }
        }
    }
	
	// Check if input is on the correct beat
	public void CheckInputAccuracy ()
	{
        foreach (var beat in beatList)
        {
            var correctBeat = beat;

            var beatDifference = f_beat - correctBeat;

            if (beatDifference > -.49f && beatDifference < 0f)
            {
                //Early
                HitAction(beat);
                Debug.Log("Hit Early: " + beat);
                break;
            }
            if (beatDifference > 0f && beatDifference < .49f)
            {
                //Late
                HitAction(beat);
                Debug.Log("Hit Late: " + beat);
                break;
            }
            if (beatDifference > 7.5f && beat == 0)
            {
                //Early on 1st beat
                HitAction(beat);
                Debug.Log("Hit Early: " + beat);
                break;
            }

            //If all checks fail,
            b_beatFired = false;
        }
	}
	
	/// <summary>
	/// Action to take when a correct beat is hit
	/// </summary>
	/// <param name='beat'>
	/// Beat that has been hit
	/// </param>
    public void HitAction(int beat)
    {
        b_beatFired = true;

        //Add a hit
        i_correctBeats++;
    }
	
	/// <summary>
	/// Check if BeatMachine should pulse
	/// </summary>
    public void CheckForPulse()
    {
        foreach (int beat in beatList)
        {
            int correctBeat = beat;

            float beatDifference = f_beat - correctBeat;

            if (beatDifference > 0f && beatDifference < f_errorAllowance)
            {
                //Late
                PulseAction(beat);
                break;
            }
            if (beatDifference > -f_errorAllowance && beatDifference < 0f)
            {
                //Early
                PulseAction(beat);
                break;
            }
            if (beatDifference > (8 - f_errorAllowance) && beat == 0)
            {
                //Early on 1st beat
                PulseAction(beat);
                break;
            }
            //If all checks fail,
            b_pulseFired = false;
        }
    }
	
	/// <summary>
	/// Action to take whenever the BeatMachine pulses
	/// </summary>
	/// <param name='beat'>
	/// Beat that must pulse
	/// </param>
	public void PulseAction(int beat)
	{
        //Stop the current iTween
        iTween.Stop(gameObject);

	    if (TestForPostitions)
	    {
            var newMarker = (GameObject)Instantiate(go_marker, new Vector3(PlayerGameObject.transform.position.x, PlayerGameObject.transform.position.y - 2, -1), Quaternion.identity);
            markers.Add(newMarker);
	    }

        

        //prevents scaling leakage from excessive iTween
        transform.localScale = Vector3.one;

        // Apply a scale punch that lasts for a bit less than an 8beat
        if (SongAudio.pitch <= 1)
        {
            iTween.PunchScale(gameObject, transform.localScale * SongAudio.pitch, .15f/SongAudio.pitch);
        }
        else
        {
            iTween.PunchScale(gameObject, transform.localScale * SongAudio.pitch, .15f/SongAudio.pitch);
        }

        b_pulseFired = true;

        if (f_beat > (8 - f_errorAllowance))
        {
            //CheckForSpeedup();

            //Debug.Log("Reset");

            //Clear the correctBeats count
            i_correctBeats = 0;
        }        
	}
	
	
    private void CheckForSpeedup()
    {
        //Raise or lower the song's speed(pitch) depending on player performance
        if (i_correctBeats >= Mathf.CeilToInt(beatList.Count * .75f))
        {
            //SongAudio.pitch += iTween.FloatUpdate(0, .125f, 2f);
            SongAudio.pitch += .0125f;
        }
        else if (i_correctBeats < Mathf.CeilToInt(beatList.Count * .25f))
        {
            SongAudio.pitch -= .0125f;
        }

        //Limit pitch shifting
        if (SongAudio.pitch >= 1.5f)
        {
            SongAudio.pitch = 1.5f;
        }
        else if (SongAudio.pitch <= .5f)
        {
            SongAudio.pitch = .5f;
        }
    }
}