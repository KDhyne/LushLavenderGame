using UnityEngine;
using System.Collections;

public class S_AnimationEngine : MonoBehaviour
{
    //#region AnimationStrings
    //public string anim_idle;
    //public string anim_walk;
    //public string anim_run;
    //public string anim_jump;
    //public string anim_fall;
    //#endregion

    private exSpriteAnimation spriteAnim;

    public exSpriteAnimState currentAnim;

    public bool b_canPlay = true;

    // Use this for initialization
	void Start () 
    {
        spriteAnim = (exSpriteAnimation)transform.GetComponent("exSpriteAnimation");
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    
    /// <summary>
    /// Plays a given animation.
    /// </summary>
    /// <param name="animationName">The animation to play</param>
    /// <param name="completeAnimation">True: Require the given animation to play completely. False: Allow the animation to be overridden.</param>
    public void PlaySpriteAnimation(string animationName, bool completeAnimation)
    {
        //Debug.Log("Looping?");
        
        //Store the current animation
        //exSpriteAnimState currentAnim = spriteAnim.GetAnimation(animationName);

        //Get the current animation length
        //float animLength = currentAnim.length;

        //Check to see if the animation is already playing, or if it is allowed to play
        if (!spriteAnim.IsPlaying(animationName))
        {
            //Play the animation
            spriteAnim.Play(animationName);
        }

        //If the animation is required to complete, only enable
        //playing a new animation after the current animation is finished
        //if (completeAnimation == true)
        //{
        //    Debug.Log("Waiting...");


        //    b_canPlay = false;

        //    if (spriteAnim.GetCurrentAnimation().time >= animLength)
        //    {
        //        b_canPlay = true;
        //    }
        //}
    }
}
