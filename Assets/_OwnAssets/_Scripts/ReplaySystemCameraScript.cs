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

    [SerializeField] public static ReplaySystemCameraScript instance;

    public bool MouseActive;
    private Transform cam;
    public GraphicRaycaster grc;

    [Header("XZ Movement")]
    [SerializeField] private float xzSens = 5f;
    [SerializeField] private float xzAmtToMove = 2f;

    [Header("Y Movement")]
    [SerializeField] private float ySens = 1f;
    [SerializeField] private float targetYvalue = 0f;
    [SerializeField] private float yAmtToMove = 1f;

    [Header("Pan Movement")]
    [SerializeField] private float panAmt = 2f;
    [SerializeField] private float panBounceSpd = 1f;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        grc = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        cam = this.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            MouseActive = true;
        }
        else
        {
            MouseActive = false;
        }

        if (MouseActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            grc.enabled = false;

            Rotate();
            Zoom();
            Pan();
        }
        else
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
        float targetXvalue = Input.GetAxis("Horizontal") * xzAmtToMove;
        float targetZvalue = Input.GetAxis("Vertical") * xzAmtToMove;

        // Lerps the camera for smoother panning
        //Lerps from its own position to the next position given a vector3 which is defined by the input axes

        this.transform.position =
            Vector3.Lerp(this.transform.position, (this.transform.position + ((transform.forward * targetZvalue) + (transform.right * targetXvalue))), xzSens * Time.deltaTime);
    }

    private void Rotate()
    {
        float yRotValue = Input.GetAxis("Mouse X") * panAmt;
        float xRotValue = Input.GetAxis("Mouse Y") * panAmt;

        // Rotates Camera Container
        transform.Rotate(0, yRotValue, 0);
        
        // Rotates Camera
        cam.localRotation *= Quaternion.Euler(-xRotValue, 0, 0);
        if (cam.localRotation.eulerAngles.x > 70 && cam.localRotation.eulerAngles.x < 300)
        {
            cam.localRotation = Quaternion.Slerp(cam.localRotation, Quaternion.Euler(0, 0, 0), panBounceSpd * Time.deltaTime);
        }

        if (cam.localRotation.eulerAngles.y == 180)
        {
            cam.localRotation = Quaternion.Euler(cam.localRotation.eulerAngles.x, 0, 0);
        }
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            targetYvalue += yAmtToMove;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            targetYvalue -= yAmtToMove;
        }

        this.transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, targetYvalue, transform.position.z), ySens * Time.deltaTime);
    }
}
