using UnityEngine;

public class CameraMan : MonoBehaviour
{
    private Transform cameraManTransform;
    private GameObject cameraTarget;

    public float VerticalPosition;
    public bool IsFollowingPlayer;

    public float VerticalAdjustment = 0f;
    public float AdjustmentSpeed;

	public GameObject TransitionStill;

    // Use this for initialization
    void Start()
    {
        cameraManTransform = Camera.main.transform;
        cameraTarget = GameObject.Find("Player");
		Invoke("TransitionIn", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFollowingPlayer)
            return;

        iTween.MoveUpdate(cameraManTransform.gameObject, new Vector3(cameraTarget.transform.position.x + 25f, VerticalPosition + this.VerticalAdjustment, -10f), .6f);
        
        VerticalPan();
    }

    /// <summary>
    /// Move the camera up or down based on the player's location on screen. 
    /// </summary>
    public void VerticalPan()
    {
        var playerScreenPos = Camera.main.WorldToScreenPoint(cameraTarget.transform.position);
        var ratio = playerScreenPos.y / Camera.main.pixelHeight;

        if (ratio < 0.3f) // if we're below our safe frame, lower VerticalAdjustment
        {
            if (ratio < 0.1f)
                this.VerticalAdjustment -= Time.deltaTime * AdjustmentSpeed * 3f;

            else if (ratio < 0f)  //Lower VerticalAdjustment dramatically if the player is completely below the screen
                this.VerticalAdjustment -= Time.deltaTime * AdjustmentSpeed * 10f;

            else
                this.VerticalAdjustment -= Time.deltaTime * AdjustmentSpeed * 1.5f;
        }
            
        else if (ratio > 0.5f) // if we're above our safe frame, raise VerticalAdjustment    
            this.VerticalAdjustment += Time.deltaTime * AdjustmentSpeed;
        
        //else
            //VerticalAdjustment = iTween.FloatUpdate(cameraTarget.transform.position.y, 0f, 2f);
    }

	public void TransitionIn()
	{
		TransitionStill.transform.localPosition = new Vector3(0f,0f,1f);

		Vector3 targetPosition  = new Vector3(
			transform.localPosition.x - 100f,
			transform.localPosition.y,
			1f);

		iTween.MoveTo(TransitionStill, iTween.Hash("islocal", true, "position", targetPosition, "time", 5f));
	}

	public void TransitionOut()
	{
		Vector3 targetPosition  = new Vector3(0f,0f,1f);

		iTween.MoveTo(TransitionStill, iTween.Hash("islocal", true, "position", targetPosition, "time", 3f));
	}
}
