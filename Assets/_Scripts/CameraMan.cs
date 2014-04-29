using UnityEngine;

public class CameraMan : MonoBehaviour
{
    private Transform cameraManTransform;
    private GameObject cameraTarget;

    public bool IsFollowingPlayer;

    // Use this for initialization
    void Start()
    {
        cameraManTransform = Camera.main.transform;
        cameraTarget = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFollowingPlayer)
        {
            iTween.MoveUpdate(cameraManTransform.gameObject,
                    new Vector3(cameraTarget.transform.position.x + 15f, 2.5f, -10f), .6f);
        }
    }
}
