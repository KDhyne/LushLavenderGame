using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S_BeatObject : MonoBehaviour 
{
    public AudioSource songAudio;


    int i_bpm = 0;
    public int i_measureCounter = 0;
    int rhythmBeatsPerMeasure = 8;
    public float songTime = 0;
    float f_beatInterval = 0;
    public float f_eightBeatInterval = 0;
    public int i_beatDelay;

    int beats = 0;
    int totalBeats = 0;
    public int graceBeats = 0;
    public int correctBeats = 0;
    //public bool hittable = false;
    public List<int> li_rhythmBeatList = new List<int>();

    
	public GameObject[] hitMarkers;
    



	// Use this for initialization
	void Start () 
    {
        i_bpm = 149;

        rhythmBeatsPerMeasure = 8;

        songAudio = (AudioSource)GetComponent("AudioSource");

        //noteTime = 0;

        //quarter note interval - 4 to make a measure
        f_beatInterval = (60f / i_bpm);

        //eigth note interval - 8 to make 
        f_eightBeatInterval = f_beatInterval / 2;

        InvokeRepeating("BeatAction", ((f_eightBeatInterval * i_beatDelay) - 0.1f), f_eightBeatInterval);

        for (int i = 0; i < hitMarkers.Length; i++)
		{
			if (li_rhythmBeatList.Contains(i+1))
            {
                hitMarkers[i].renderer.material.color = Color.blue;
            }
            else
            {
                hitMarkers[i].renderer.material.color = Color.gray;
            }
		}

	}
	
	// Update is called once per frame
	void Update () 
    {
        songTime = songAudio.time;

        //Debug.Log(songAudio.time);
        //noteTime = (songAudio.time * i_bpm * (2) / 60);
        //Debug.Log(noteTime);

        //Detect Hits
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckInputTiming();
        }
	}

    void CheckInputTiming()
    {
        //Get the current song time
        float inputTime = songAudio.time;

        Debug.Log("Song Time: " + inputTime);

        //Add 1 to the measure counter to offset the delay
        float timeCorrectedMeasure = i_measureCounter; //((1/8) * i_beatDelay);

        float elapsedMeasureTime = (f_beatInterval * timeCorrectedMeasure * 4);

        //Get the index of the last element in the list in case we need to wrap around
        int lastIndex = li_rhythmBeatList.Count - 1;

        foreach (int rhythmBeat in li_rhythmBeatList)
	    {
            //Store this in a 
            int tempRhythmBeat = rhythmBeat;

            //Calculate the time of the expected input based on elapsed measures and target rhythm beat
            float expectedInputTime = elapsedMeasureTime + ((tempRhythmBeat - 1) * f_eightBeatInterval);

            Debug.Log("Input Time: " + expectedInputTime);

            float timeDifference = inputTime - expectedInputTime;

            Debug.Log("Difference: " + timeDifference);
			
            //Determine whether the player was early or late
			if (timeDifference < 0)
			{
                Debug.Log("Early on: " + tempRhythmBeat);
			}
            else if (timeDifference > 0)
            {
                //Wrap around to beat 1 if too late on last beat
                if (tempRhythmBeat == li_rhythmBeatList[lastIndex]) //8)
                {

                    Debug.Log("Checking for wrap around");

                    //Correct for the eighth beat's expected input
                    float eighthBeatExpectedInputTime = elapsedMeasureTime + (7 * f_eightBeatInterval);

                    //Find the difference again
                    float newtimeDifference = inputTime - eighthBeatExpectedInputTime;

                    //If the time difference is greater than a sixteenth note,
                    //count it as an early 1st beat.
                    if (newtimeDifference > (f_eightBeatInterval / 2))
                    {
                        Debug.Log(newtimeDifference);

                        Debug.Log("Wrapping around to 1");

                        //But only if 1 is in the rhythm beat array in the first place
                        if (li_rhythmBeatList.Contains(1))
                        {
                            tempRhythmBeat = 1;
                            Debug.Log("Early on: " + tempRhythmBeat);
                            HitBeat(tempRhythmBeat);
                            break;
                        }
                    }
                    else if (Mathf.Abs(timeDifference) < (f_eightBeatInterval / 2))
                    {
                        Debug.Log("Late on: " + tempRhythmBeat);

                        //Debug.Log("Hit: " + rhythmBeat);
                        HitBeat(tempRhythmBeat);
                        //HitBeat(tempRhythmBeat);
                        break;
                    }
                }
                else //Not the last note
                {
                    Debug.Log("Late on: " + tempRhythmBeat);
                }                
            }
			
            if (Mathf.Abs(timeDifference) < (f_eightBeatInterval/2))
            {
                //Debug.Log("Hit: " + rhythmBeat);
                HitBeat(tempRhythmBeat);
                //HitBeat(tempRhythmBeat);
                break;
            }
            else
            {
                MissBeat(tempRhythmBeat);
            }
        }
    }

    private void MissBeat(int tempRhythmBeat)
    {
        hitMarkers[tempRhythmBeat - 1].renderer.material.color = Color.blue;
        correctBeats--;
    }

    private void HitBeat(int tempRhythmBeat)
    {
        hitMarkers[tempRhythmBeat - 1].renderer.material.color = Color.green;
        correctBeats++;
    }

    void BeatAction()
    {
        totalBeats++;
        beats++;
        
        if (beats > rhythmBeatsPerMeasure)
        {
            beats = 1;
            i_measureCounter++;

            //reset the debug visualizer
            for (int i = 0; i < hitMarkers.Length; i++)
            {
                if (li_rhythmBeatList.Contains(i + 1))
                {
                    hitMarkers[i].renderer.material.color = Color.blue;
                }
            }

            //Adjust song time
            if (correctBeats == (li_rhythmBeatList.Count - graceBeats))
            {
                AdjustTempo(1);
            }
            else if (correctBeats < 0)
            {
                AdjustTempo(-1);
            }

            correctBeats = 0;

            //Adjust InvokeRepeating function
			
        }

        //Do the following for each rhythm beat
        foreach (var rhythmBeat in li_rhythmBeatList)
        {
            //If the current beat is a rhythm beat
            if (beats == rhythmBeat)
            {
                //prevents scaling leakage from excessive iTween
                this.transform.localScale = Vector3.one;

                //Stop the current iTween and apply a scale punch that lasts for a bit less than an 8beat
                iTween.Stop(this.gameObject);
                iTween.PunchScale(this.gameObject, this.gameObject.transform.localScale / 2, (f_eightBeatInterval / 1.2f));
            }
        }
    }

    private void AdjustTempo(int sign)
    {
        songAudio.pitch = songAudio.pitch + (sign * 0.125f);
        f_eightBeatInterval /= songAudio.pitch;
        CancelInvoke("BeatAction");
        InvokeRepeating("BeatAction", 0, f_eightBeatInterval);
    }
}
