using UnityEngine;
using System.Collections;

public class RestartLevel : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}