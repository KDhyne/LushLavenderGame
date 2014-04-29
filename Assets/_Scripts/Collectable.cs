using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public GameObject IdleParticleEffect;
    public GameObject CollectedParticleEffect;
    public AudioClip CollectedAudioClip;

    private GameObject idleParticle;

	// Use this for initialization
	void Start ()
	{
	    idleParticle = (GameObject)Instantiate(IdleParticleEffect, this.transform.position, Quaternion.identity);
	}

    public abstract void OnTriggerEnter(Collider otherObj);

    /// <summary>
    /// Disable the renderer and instantiate the collected particle effect.
    /// Finally, destroy this collectable. 
    /// </summary>
    public void DestroyCollectable()
    {
        //Hide and explode
        renderer.enabled = false;
        var particle = (GameObject)Instantiate(CollectedParticleEffect, this.transform.position, Quaternion.identity);
        //TODO: Add SoundManager.PlaySFX(CollectedAudioClip);


        //Destroy the particles
        Destroy(idleParticle);
        Destroy(particle.gameObject, .5f);
        
        //Destroy this gameobject
        Destroy(gameObject);
    }
}
