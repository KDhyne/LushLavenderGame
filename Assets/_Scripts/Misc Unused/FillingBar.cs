using UnityEngine;
using System.Collections;
using System.IO;

public class FillingBar : MonoBehaviour 
{
	
	public float powerLevel;
	public float powerMax = 100;
	public float ratio = 1;
	public GameObject bar;
	
	public bool delayOff = true;
	public float healthTimer;
	
	// Use this for initialization
	void Start () 
	{
		powerLevel = powerMax;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!delayOff)
		{
			healthTimer -= Time.deltaTime;
			
			if (healthTimer <= 0) 
			{
				delayOff = true;
				healthTimer = 1f;
			}
		}		
		
		if(Input.GetKeyDown(KeyCode.A))
		{
			powerLevel -= 3;
			healthTimer = 1f;
			delayOff = false;			
		}
		else if(powerLevel < powerMax && delayOff)
		{
			powerLevel += .5f;
		}
		
		if(powerLevel > powerMax)
			powerLevel = powerMax;
		if (powerLevel <= 0)
			powerLevel = 0;
		
		iTween.ScaleUpdate(bar, new Vector3(20*(powerLevel/powerMax), 1, 1), .5f);
		
	}
	
	void OnGUI()
	{
		UnityEngine.GUI.Label(new Rect(20, 20, 100, 30),  ((int)powerLevel).ToString());
		//GUI.Label(new Rect(Screen.width/2 - bar.transform.position.x, Screen.cameraHeight/2 - bar.transform.position.y, 100, 30), powerLevel.ToString());
	}
}