using UnityEngine;

public class CameraMan : MonoBehaviour
{
    private Transform cameraManTransform;
    private GameObject cameraTarget;

    public float VerticalPosition;
    public bool IsFollowingPlayer;

    private float verticalAdjustment = 0f;

    // Use this for initialization
    void Start()
    {
        cameraManTransform = Camera.main.transform;
        cameraTarget = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsFollowingPlayer)
            return;

        iTween.MoveUpdate(cameraManTransform.gameObject,
                    new Vector3(cameraTarget.transform.position.x + 15f, VerticalPosition + verticalAdjustment, -10f), .6f);

        //TODO: un-psuedocode this block
        //Move the camera up or down based on the player's location on screen.
        this.SafeFrameCheck();
    }

    void SafeFrameCheck()
    {
        var screenPos = Camera.main.WorldToScreenPoint(cameraTarget.transform.position);
        var ratio = screenPos.y / Camera.main.pixelHeight;

        if (ratio < 0.3f) // if we're below our safe frame
            verticalAdjustment -= .1f;

        if (ratio > 0.7f) // if we're above our safe frame, return false       
            verticalAdjustment += .1f;
    }

    void OnGUI()
    {
        //GUI.Box(playerSafeFrame, "Safe Frame");
    }
}
