using System;

using UnityEngine;
using System.Collections;

public class S_ActorBase : MonoBehaviour
{
    public enum ActorState
    {
        Alive,
        Dead
    }

    public int AttackValue = 1;
    public float InvincibilityTime = 1f;

    public int MaxHitPoints = 4;   //Set Hit Points
    public int CurrentHitPoints;    
    public bool CanBeHit = true;    //determines whether or not the actor can be hit
    public int ScoreValue = 1;      //Score Value

    public Transform ActorTransform;
    public Animator SpriteAnimator;
    public GameObject SpawnLocation;
    public ActorState CurrentActorState = ActorState.Alive;

	// Use this for initialization
    public virtual void Start()
    {
        //Cache the transform
        this.ActorTransform = transform;

        //Set the animation object
        this.SpriteAnimator = this.GetComponent<Animator>();
	
        //Set MaxHP
        CurrentHitPoints = MaxHitPoints;
    }
	
	// Update is called once per frame
	public virtual void Update ()
    {
        //ActorState checks
        switch (this.CurrentActorState)
        {
            case ActorState.Dead:
                //Gracefully destroy the actor
                this.DestroyActor();
                break;

            case ActorState.Alive:
                //HP check for death
                if (this.CurrentHitPoints <= 0)
                    this.CurrentActorState = ActorState.Dead;

                break;
        }
	}

    /// <summary>
    /// Handle actor destruction gracefully. Add actor scoreValue to total score.
    /// </summary>
    public virtual IEnumerator DestroyActor()
    {
        var manager = GameObject.Find("GUI Manager").GetComponent<S_GUI>();
        manager.i_score += this.ScoreValue;
        Debug.Log("Hit");
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
        
    }

    /// <summary>
    /// Subtract the specified number of hit points from this Actor's health.
    /// </summary>
    /// <param name="damageAmount">Number of hit points to subtract</param>
    /// <param name="invincibileTime">Time in seconds actor is invincible after hit</param>
    public virtual IEnumerator TakeDamage(int damageAmount, float invincibileTime = -1f)
    {
        if (Math.Abs(invincibileTime - (-1f)) < .01)
        {
            invincibileTime = InvincibilityTime;
        }

        //Break out immediately if actor can't be hit
        if (!this.CanBeHit)
            yield break;

        //subtract hit points
        this.CurrentHitPoints -= damageAmount;
        //Temporary invincibility
        this.CanBeHit = false;
        //Wait a given time, then make the actor hittable again
        yield return new WaitForSeconds(invincibileTime);
        this.CanBeHit = true;
    }
}
