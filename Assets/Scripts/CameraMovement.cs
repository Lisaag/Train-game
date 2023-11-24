using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject cameraPointsParent = null;
    [SerializeField] float movementSpeed = 1f;
    private int currentIndex = 4;
    private Vector2 dest = default;

    private int cameraPointCount = 0;

    private bool isCameraMoving;

    // Start is called before the first frame update
    void Awake()
    {
        cameraPointCount = cameraPointsParent.transform.childCount;
    }

    private void FixedUpdate()
    {
        if (isCameraMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(dest.x, dest.y, transform.position.z), movementSpeed);

            if (Vector2.Distance(transform.position, dest) < 0.001f)
            {
                isCameraMoving = false;
            }
        }
    }

    public void MoveCamera(int direction)
    {
        if (currentIndex + direction < 1 || currentIndex + direction == cameraPointCount - 1)
        {
            Debug.Log("Cannot move camera");
            currentIndex += direction;
        }
        else
        {
            currentIndex += direction;
            dest = cameraPointsParent.transform.GetChild(currentIndex).transform.position;
            isCameraMoving = true;
            Debug.Log("MoveCamera towards " + dest);
        }
    }
}
