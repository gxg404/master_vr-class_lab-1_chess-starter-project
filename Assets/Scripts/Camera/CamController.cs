using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private float rotationSpeed = 4f;
    private float movementSpeed = 0.5f;
    private float zoomSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handlePanning();
    }

    private void handleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            // NOTE: delta time does not need to be used because "Mouse X/Y" returns the difference form last frame, so delta time would break
            // currently has the bug, that you can pan 180° up and end up with a head-down camera view...
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.RotateAround(transform.position, Vector3.up, rotationX);
            transform.RotateAround(transform.position, transform.right, -rotationY);
        }
    }

    private void handlePanning()
    {
        // Space used for testing purposes on laptops that don't have a scroll weel
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.Space))
        {
            float movementX = Input.GetAxis("Mouse X") * movementSpeed;
            float movementY = Input.GetAxis("Mouse Y") * movementSpeed;

            transform.Translate(new Vector3(movementX, movementY, 0), Space.Self);
        }
    }

    // TODO: check if it's working properly; couldn't be tested on macbook-trackpad
    private void handleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            transform.position = transform.position + transform.forward * scroll * zoomSpeed;
        }
    }
}
