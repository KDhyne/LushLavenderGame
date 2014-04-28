using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour 
{

	public bool b_activated;

	// Use this for initialization
	void Start () 
	{
		b_activated = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (b_activated)
		{
			renderer.material.color = Color.green;
		}
		else
		{
			renderer.material.color = Color.red;
		}
	}

	void OnTriggerEnter (Collider otherObj)
	{
		if (otherObj.tag == "Projectile")
		{
			b_activated = !b_activated;
		}
	}
}
