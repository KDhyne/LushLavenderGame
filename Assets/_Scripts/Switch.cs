using UnityEngine;

public class Switch : MonoBehaviour 
{

	public bool Activated;

	// Use this for initialization
	void Start () 
	{
		this.Activated = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    this.renderer.material.color = this.Activated ? Color.green : Color.red;
	}

    void OnTriggerEnter (Collider otherObj)
	{
		if (otherObj.tag == "Projectile")
		{
			this.Activated = !this.Activated;
		}
	}
}
