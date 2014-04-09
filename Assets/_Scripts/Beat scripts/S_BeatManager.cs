using UnityEngine;
using System.Collections;

public class S_BeatManager : MonoBehaviour 
{
    int i_bpm = 0;

    float f_beatTime;
    float f_quarterBeatTime;
    float f_measureTime;

    float f_timer = 0;

    public AudioSource audSource;

    public int[] ar_beatArray;
    int counter = 0;

	// Use this for initialization
	void Start () 
    {
        i_bpm = 149;

        audSource = (AudioSource)GetComponent("AudioSource");

        f_beatTime = (60f / i_bpm);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        f_quarterBeatTime = f_beatTime/4;
        Debug.Log(f_quarterBeatTime.ToString());

        f_timer += Time.deltaTime;

        if (f_timer >= f_quarterBeatTime / audSource.pitch)
        {
            Debug.Log("Check for beat");
            if (ar_beatArray[counter] == 1)
            {
                iTween.Stop(this.gameObject);
                //iTween.PunchScale(this.gameObject, iTween.Hash("name", "BeatPunch", "amount", this.gameObject.transform.localScale));
                iTween.PunchScale(this.gameObject, this.gameObject.transform.localScale/2, (f_beatTime / 2f));
                Debug.Log("Beat: ");
            }
            counter++;
            f_timer = 0;
        }

        if (counter >= ar_beatArray.Length)
        {
            counter = 0;
        }
	}
}
