using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerEventTest : MonoBehaviour
{
    [SerializeField] private VRTK_ControllerEvents controllerEventsL;
    [SerializeField] private VRTK_ControllerEvents controllerEventsR;
    [SerializeField] private Transform playerCamHead;
    [SerializeField] private Transform playerLPC;
    [SerializeField] private float speed;

    private void Update()
    {
        Debug.Log("Left Joy: " + controllerEventsL.GetAxis(VRTK_ControllerEvents.Vector2AxisAlias.TouchpadTwo));
        JoyController(controllerEventsL.GetAxis(VRTK_ControllerEvents.Vector2AxisAlias.TouchpadTwo));
    }
    
    public void JoyController(Vector2 controllerAxis)
    {
        if (controllerAxis.y > 0.3)
        {
            Vector3 forwardDir = playerCamHead.forward;
            forwardDir.y = 0;
            forwardDir = forwardDir.normalized;

            playerLPC.Translate(forwardDir * speed * Time.deltaTime);
        }
        else if (controllerAxis.y < -0.3)
        {
            Vector3 forwardDir = playerCamHead.forward;
            forwardDir.y = 0;
            forwardDir = forwardDir.normalized;

            playerLPC.Translate(-forwardDir * speed * Time.deltaTime);
        }

        if (controllerAxis.x < -0.3)
        {
            Vector3 rightDir = playerCamHead.right;
            rightDir.y = 0;
            rightDir = rightDir.normalized;

            playerLPC.Translate(-rightDir * speed * Time.deltaTime);
        }
        else if (controllerAxis.x > 0.3)
        {
            Vector3 rightDir = playerCamHead.right;
            rightDir.y = 0;
            rightDir = rightDir.normalized;

            playerLPC.Translate(rightDir * speed * Time.deltaTime);
        }
    }
}
