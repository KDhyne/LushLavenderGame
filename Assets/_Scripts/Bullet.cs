using UnityEngine;
using System.Collections;
using System.IO;

public class Bullet : MonoBehaviour 
{
    //States
    enum BulletState
    {
        Held,
        Fired,
        PowerShot
    }

    //Cache the transform
    Transform T_bulletTransform;
    //Target position
    Transform T_targetTransform;
    //Player
    Player player;
    //Player position
    Transform T_playerTransform;
    //Explosion Prefab
    public GameObject go_explosion;
    //ChargeEffect
    public GameObject go_chargeEffect;
    //ChargeAuraEffect
    public GameObject go_chargeAuraEffect;
    //Power bar
    GameObject go_powerBar;
    //Trail Renderer
    Renderer TR_trail;
    //Tracks the number of bounces
    int i_bounceCount = 0;
    //Max number of bounces allowed
    public int i_maxBounceCount = 3;
    //The path that the bullet follows when shot
    Vector3 v2_trajectory;
    //Bullet's current speed
    float f_bulletSpeed = 0;
    //Maximum launch velocity
    public float f_bulletMaxSpeed = .75f;
    //How fast the bullet speed rises
    public float f_chargeSpeed = 3.5f;
    //Gravity factor. Bigger = more gravity
    public float f_gravity;
    //Hold onto the current PlayerState and initial state
    BulletState St_currentBulletState = BulletState.Held;
    //array of names of objects to shake when bullets explode
    public string[] stuffToShake;


    bool b_powerShot;

    //How much damage the bullet deals
    public int i_bulletDamage = 1;
    
    //Local particle effect variables
    GameObject charge;
    GameObject chargeAura;

	// Use this for initialization
	void Start() 
    {
        //Cache the transform
        T_bulletTransform = transform;
        //Get the trail renderer
        TR_trail = (Renderer)this.GetComponent("TrailRenderer");
        //Get the target object's transform
        T_targetTransform = GameObject.Find("Target Prefab").transform;

        this.player = (Player)GameObject.Find("Player").GetComponent("Player");
        //Get the Player transform
        T_playerTransform = GameObject.Find("Player").transform;
        //Get the power bar
        go_powerBar = GameObject.Find("Power Bar");
        //Start the power bar scale at 0
        go_powerBar.transform.localScale = new Vector3(1, 1, 0);

        //Create one charge effect 
        charge = (GameObject)Instantiate(go_chargeEffect, T_bulletTransform.position, Quaternion.identity);
        chargeAura = (GameObject)Instantiate(go_chargeAuraEffect, T_bulletTransform.position, Quaternion.identity);

	}
	
	// Update is called once per frame
	void Update() 
    {
        switch (St_currentBulletState)
        {
            //The bullet is in the player's hands.
            case BulletState.Held:
            {
                //Make charge and aura follow player
                charge.transform.position = T_bulletTransform.position;
                chargeAura.transform.position = T_bulletTransform.position;
                //Disable the trail renderer
                TR_trail.enabled = false;
                //Get the target's position relative to the bullet
                if (Input.GetMouseButton(0))
                {
                    //Calculate the bullet's trajectory vector
                    v2_trajectory = (Vector2)T_targetTransform.position - new Vector2(T_bulletTransform.position.x, T_bulletTransform.position.y);
                    //Normalize the vector to allow for proper scaling
                    v2_trajectory.Normalize();
                    //Show direction and intensity of throw
                    Debug.DrawRay(T_bulletTransform.position, v2_trajectory * (15 / f_bulletMaxSpeed) * f_bulletSpeed, Color.red);
                }
                //Move the bullet with the player
                T_bulletTransform.position = (T_playerTransform.position + this.player.BulletOffset);
                //Tween the bullet speed between the default value and the max value
                f_bulletSpeed = iTween.FloatUpdate(f_bulletSpeed, f_bulletMaxSpeed, f_chargeSpeed);
                //Debug.Log(f_bulletSpeed);

                //Scale the power bar based on the bullet charge
                go_powerBar.transform.localScale = new Vector3(1, 1, ((15 / f_bulletMaxSpeed) * f_bulletSpeed));

                //Create an explosion on the power bar to indicate power shot timing
                if (f_bulletSpeed >= 2f && f_bulletSpeed <= 2.15f)
                {
                    GameObject explosion = (GameObject)Instantiate(go_explosion, go_powerBar.transform.position + (new Vector3(0f, 13f)), Quaternion.identity);
                    //Destroy the explosion after a set time
                    Destroy(explosion, .5f);
                }

                //Destroy the bullet if held too long
                if (f_bulletSpeed > f_bulletMaxSpeed - .0005f)
                {
                    Destroy(gameObject);
                    //Destroy the charge effect
                    Destroy(charge);
                    //Destroy the charge aura
                    Destroy(chargeAura);
                    //reset power bar scale
                    go_powerBar.transform.localScale = new Vector3(1, 1, 0);
                }
                //On mouse release, change to Fired state
                if (Input.GetMouseButtonUp(0))
                {
                    //Check bulletspeed within a certian range for a power shot
                    if (f_bulletSpeed >= 2f && f_bulletSpeed <= 2.25f)
                    {
                        St_currentBulletState = BulletState.PowerShot;
                        b_powerShot = true;
                        
                    }
                    //Else change to b_beatFired state to release the bullet
                    else
                    {
                        //Change to b_beatFired state to release the bullet
                        St_currentBulletState = BulletState.Fired;
                    }
                    //reset power bar scale
                    go_powerBar.transform.localScale = new Vector3(1, 1, 0);
                }
            }
            break;
            //The bullet is released and flying
            case BulletState.Fired:
            {
                //Destroy charge
                Destroy(charge);
                //Destroy the charge aura
                Destroy(chargeAura);

                //Vertical speed
                float speedY = 0;
                //Add gravity over time
                speedY -= (f_gravity * Time.deltaTime);
                //Adjust the vertical speed of the trajectory
                v2_trajectory = new Vector2(v2_trajectory.x, v2_trajectory.y += speedY);
                //Move the bullet
                T_bulletTransform.Translate(v2_trajectory * f_bulletSpeed);
                //enable the trail renderer
                TR_trail.enabled = true;

                //Delete object when max bounces reached
                if (i_bounceCount > i_maxBounceCount)
                {
                    DestroyBullet();
                }
                //Delete object when it goes above or below a certain cameraHeight
                else if (T_bulletTransform.position.y > 50f || T_bulletTransform.position.y < -5f)
                {
                    Destroy(gameObject);
                }
            }
            break;
            case BulletState.PowerShot:
            {
                //Destroy charge
                Destroy(charge);
                //Destroy the charge aura
                Destroy(chargeAura);

                //Vertical speed
                float speedY = 0;
                //Add gravity over time
                speedY -= (f_gravity * 1.5f * Time.deltaTime);
                //Adjust the vertical speed of the trajectory
                v2_trajectory = new Vector2(v2_trajectory.x, v2_trajectory.y += speedY);
                //Move the bullet
                T_bulletTransform.Translate(v2_trajectory * f_bulletSpeed * 2f);
                //enable the trail renderer
                TR_trail.enabled = true;

                //Change bullet and trail color
                TR_trail.material.color = Color.blue;
                renderer.material.color = Color.blue;
                go_explosion.particleSystem.startColor = Color.blue;
                
                //Create explosions only when the bullet renderer is enabled
                if (renderer.enabled)
                {
                    //Debug.Log("adding explosions");
                    GameObject explosion = ((GameObject)Instantiate(go_explosion, T_bulletTransform.position, Quaternion.identity));
                    //Destroy the explosion after a set time
                    Destroy(explosion, .5f);
                }

                //Delete object when max bounces + 1 reached
                if (i_bounceCount > (i_maxBounceCount + 1))
                {
                    DestroyBullet();
                }
                //Delete object when it goes above or below a certain cameraHeight
                else if (T_bulletTransform.position.y > 50f || T_bulletTransform.position.y < -5f)
                {
                    Destroy(gameObject);
                }
            }
            break;
            default:
                break;
        }
	}

    private void DestroyBullet()
    {
        //Make the bullet disappear
        renderer.enabled = false;
        //Create an explosion prefab
        var explosion = (GameObject)Instantiate(go_explosion, T_bulletTransform.position, Quaternion.identity);

        //Shake the things to shake
        //foreach (string objName in stuffToShake)
        //{
        //    GameObject go = GameObject.Find(objName);
        //    //iTween.PunchPosition(go, new Vector3(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f), 0) * 10, .25f);
        //}

        //Destroy the explosion after a set time
        Destroy(explosion.gameObject, .5f);
        //Remove this object from the scene
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider otherObj)
    {
        //If it hits a wall, change horizontal movement
        if (otherObj.tag == "Wall")
	    {
            v2_trajectory = new Vector2(-v2_trajectory.x, v2_trajectory.y);
            i_bounceCount++;
	    }
        //If it hits the Floor, change vertical movement
        else if (otherObj.tag == "Floor")
        {
            v2_trajectory = new Vector2(v2_trajectory.x, -(v2_trajectory.y/2f));
            i_bounceCount++;
        }
        //If it hits and enemy, deal damage to them and explode
        if (otherObj.tag == "Enemy")
        {
            //TODO: plug the new enemy script in here
            /*S_Enemy enemy = (S_Enemy)otherObj.GetComponent("S_Enemy");
            enemy.TakeDamage(i_bulletDamage);
            if (!b_powerShot)
            {
                DestroyBullet();
            }*/
        }
    }
}
