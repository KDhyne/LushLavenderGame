using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour
{

    private GameObject mainCamera;

    private float initialPositionX;
    private float initialPositionY;

    private List<GameObject> childElements = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
	    mainCamera = GameObject.Find("Main Camera");
        initialPositionX = mainCamera.transform.position.x;
        initialPositionY = mainCamera.transform.position.y;

	    for (int i = 0; i < transform.childCount; i++)
	    {
	        childElements.Add(transform.GetChild(i).gameObject);
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var camPosition = mainCamera.transform.position;

        //transform.position = new Vector3(camPosition.x, mainCamera.camera.ViewportToWorldPoint(new Vector3(0,0,0)).y, transform.position.z);

	    foreach (var childElement in childElements)
	    {
	        var position = childElement.transform.localPosition;

	        childElement.transform.localPosition = new Vector3(
                GetHoriziontalPosition(position.z),
                position.y,
                position.z);
	    }
	}

    float GetHoriziontalPosition(float z)
    {
        if (z >= 0)
        {
			//Let's not get weird with super high speeds as z approaches 0
			if (z < 1)
			{
				z = 1;
			}
		
			//If the object is in the background (z is +) have it move z times faster than the player
            return (initialPositionX - transform.position.x) * 1/z;
        }
		
		//If the object is in the foreground (z is negative) have it move z times faster than the player
		return (initialPositionX - transform.position.x) * -z;
    }
}