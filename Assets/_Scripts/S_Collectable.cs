using UnityEngine;

public class S_Collectable : MonoBehaviour
{
    public GameObject IdleParticleEffect;
    public GameObject CollectedParticleEffect;

	// Use this for initialization
	void Start ()
	{
	    Instantiate(IdleParticleEffect, this.transform.position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    /// <summary>
    /// Disable the renderer and instantiate the collected particle effect.
    /// Finally, destroy this collectable. 
    /// </summary>
    private void DestroyCollectable()
    {
        renderer.enabled = false;
        var particle = (GameObject)Instantiate(CollectedParticleEffect, this.transform.position, Quaternion.identity);
        Destroy(particle.gameObject, .5f);
        Destroy(gameObject);
    }
}
