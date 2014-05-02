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
        /*if (cameraTarget.transform.position.y >= CameraBounds.top - ScreenPadding)
        {
            verticalAdjustment++;
        }
        else if (cameraTarget.transform.position.y <= CameraBounds.bottom + ScreenPadding)
        {
            verticalAdjustment--;
        }*/
    }
}
