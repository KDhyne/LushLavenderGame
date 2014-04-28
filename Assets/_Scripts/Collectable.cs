using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject IdleParticleEffect;
    public GameObject CollectedParticleEffect;

    private GameObject idleParticle;

	// Use this for initialization
	void Start ()
	{
	    idleParticle = (GameObject)Instantiate(IdleParticleEffect, this.transform.position, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    /// <summary>
    /// Disable the renderer and instantiate the collected particle effect.
    /// Finally, destroy this collectable. 
    /// </summary>
    public void DestroyCollectable()
    {
        //Hide and explode
        renderer.enabled = false;
        var particle = (GameObject)Instantiate(CollectedParticleEffect, this.transform.position, Quaternion.identity);
        
        //Destroy the particles
        Destroy(idleParticle);
        Destroy(particle.gameObject, .5f);
        
        //Destroy this gameobject
        Destroy(gameObject);
    }
}
