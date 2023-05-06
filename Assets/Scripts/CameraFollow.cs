using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Makes the camera follow a game object, designed to follow the car and doesn't rotate with the object
 */ 
public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform objectToTrack;
    // How far behind the object the camera should be
    [SerializeField] private float zOffset = -7.0f;
    // How far above the object the camera should be
    [SerializeField] private float yOffset = 4.0f;
    // The angle the camera looks (x axis rotation) i.e. how much is the camera looking up or down
    [SerializeField] private float lookRotation = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        getCamPos();
    }

    // Update is called once per frame
    void Update()
    {
        if(objectToTrack != null){
            getCamPos();
        }
    }

    private void getCamPos()
    {
        var posToTrack = objectToTrack.position;
        var rotation = Quaternion.Euler(new Vector3(lookRotation, 0, 0));

        // Offset the camera
        posToTrack.z += zOffset;
        posToTrack.y += yOffset;

        transform.rotation = rotation;
        transform.position = posToTrack;
    }
}
