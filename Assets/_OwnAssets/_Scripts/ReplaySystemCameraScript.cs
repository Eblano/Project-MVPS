using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// User Instructions
/// Apply script to empty Game Object, named Camera Container, containing Camera
/// Create empty Game Object, name it "Origin"
/// If you dont, shit dont work
/// WASD / Arrow Keys for panning
/// Mouse for rotation
/// Scroll for zooming in and out
/// ctrl to disable rotation and enable cursor (toggle)
/// Backslash to reset camera position and rotation
/// </summary>

public class ReplaySystemCameraScript : MonoBehaviour
{
   


    public bool MouseActive;
    private Transform origin;
    private Transform cam;
    [SerializeField] private GraphicRaycaster grc;
    [SerializeField] [Range(0.1f, 1.0f)] private float sens = 0.2f;

    private void Start()
    {
        origin = GameObject.Find("Origin").transform;
        origin.position = this.transform.position;
        cam = this.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            MouseActive = !MouseActive;
        }

        if (!MouseActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            grc.enabled = false;

            Rotate();
            Zoom();
            Pan();
        }
        else if (MouseActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            grc.enabled = true;
        }
        

    }

    // I dont understand what this is but it solves all of life's problems
    private float ClampAngle(float angle, float from, float to)
    {
        if (angle > 180) angle = 360 - angle;
        angle = Mathf.Clamp(angle, from, to);
        if (angle < 0) angle = 360 + angle;


        return angle;
    }

    private void Pan()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");

        // Lerps the camera for smoother panning
        //Lerps from its own position to the next position given a vector3 which is defined by the input axes
        this.transform.position = Vector3.Lerp(this.transform.position, (this.transform.position + ((transform.forward * zAxisValue) + (transform.right * xAxisValue))), 0.5f);
    }

    private void Rotate()
    {
        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        //{
        float yRotValue = Input.GetAxis("Mouse X");
        float xRotValue = Input.GetAxis("Mouse Y");

        // Rotates Camera
        cam.localRotation *= Quaternion.Euler(-xRotValue, 0, 0);

        // Rotates Camera Container
        this.transform.Rotate(0, yRotValue, 0);

        // Clamps Angles
        float x = ClampAngle(cam.localRotation.eulerAngles.x, 10f, 89f);
        cam.localRotation = Quaternion.Euler(x, 360, 0);
        //}
    }

    private void Zoom()
    {
        float yTranslate = 0.0f;

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                yTranslate += 2;
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                yTranslate -= 2;
            }

            this.transform.position = Vector3.Slerp(this.transform.position, (this.transform.position + new Vector3(0, yTranslate, 0)), sens);
            //this.transform.Translate(0, yTranslate * 50 * Time.deltaTime, 0);
        }
    }

    private void Reset()
    {

        transform.SetPositionAndRotation(origin.position, Quaternion.identity);
        cam.SetPositionAndRotation(origin.position, Quaternion.Euler(10, 360, 0));
    }
}
