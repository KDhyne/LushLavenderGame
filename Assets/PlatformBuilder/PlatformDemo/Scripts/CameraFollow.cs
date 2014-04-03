using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float distance = 3.0f;
	public float height = 3.0f;
	//float damping = 5.0f;
	
	
	void Update () 
	{
		if (target != null)
		{
			transform.position = new Vector3(target.transform.position.x, target.transform.position.y + height, distance);
		}
	}
}