using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public GameObject IdleParticleEffect;
    public GameObject CollectedParticleEffect;
    public AudioClip CollectedAudioClip;

    private GameObject idleParticle;

    public bool Collected;
    public bool PartOfGroup;

	// Use this for initialization
	void Start ()
	{
	    idleParticle = (GameObject)Instantiate(IdleParticleEffect, this.transform.position + new Vector3(0,0,-5f), Quaternion.identity);
	}

    public virtual void OnTriggerEnter(Collider otherObj)
    {
        if (Collected)
            return;

        if (otherObj.tag == "Player")
        {
            this.ApplyCollectedEffect(otherObj.GetComponent<Player>());

            if (PartOfGroup)
                this.gameObject.transform.parent.GetComponent<CollectableGroup>().DestroyGroup();

            else
                this.DestroyCollectable();
        }
            
    }

    /// <summary>
    /// Do whatever the collectable does. A template for child classes.
    /// </summary>
    public abstract void ApplyCollectedEffect(Player player);

    /// <summary>
    /// Disable the renderer and instantiate the collected particle effect.
    /// Finally, destroy this collectable. 
    /// </summary>
    public void DestroyCollectable()
    {
        this.Collected = true;

        //Hide and explode
        renderer.enabled = false;
        var particle = (GameObject)Instantiate(CollectedParticleEffect, this.transform.position + new Vector3(0, 0, -5f), Quaternion.identity);
        
        //TODO: Add SoundManager.PlaySFX(CollectedAudioClip);

        //Destroy the particles
        Destroy(idleParticle);
        Destroy(particle.gameObject, .5f);
        
        //Destroy this gameobject
        Destroy(gameObject);
    }
}
