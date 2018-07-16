using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using SealTeam4;

public class PlayerSizeCalibration : MonoBehaviour
{
    [SerializeField] private float scalePercIncrement = 0.05f;
    [SerializeField] private Transform ulArmBone, llArmBone, urArmBone, lrArmBone;
    [SerializeField] private Transform lhandRef, headRef;
    [SerializeField] private float fitRadius = 0.005f;
    [SerializeField] private VRIK vrIK;
    private VRIK.References vrIKRefs;
    private PlayerInteractionSync interactionSync;
    private float heightScale, armScale;
    [SerializeField] private int breakCounter = 20;
    public static PlayerSizeCalibration instance;

    private void Start()
    {
        interactionSync = GetComponent<PlayerInteractionSync>();

        if (!interactionSync.isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        vrIKRefs = vrIK.references;
        ulArmBone = vrIKRefs.leftUpperArm;
        llArmBone = vrIKRefs.leftForearm;
        urArmBone = vrIKRefs.rightUpperArm;
        lrArmBone = vrIKRefs.rightForearm;
    }

    private void AdjustHeight(int multiplier)
    {
        heightScale = transform.localScale.y + (scalePercIncrement * multiplier);
        transform.localScale = new Vector3(heightScale, heightScale, heightScale);
    }

    private void AdjustArms(int multiplier)
    {
        armScale = llArmBone.localScale.y + (scalePercIncrement * multiplier);
        ulArmBone.localScale = llArmBone.localScale = urArmBone.localScale = lrArmBone.localScale = new Vector3(armScale, armScale, armScale);
    }

    private bool WithinDistance(Vector3 a, Vector3 b, float comparison)
    {
        return (a - b).sqrMagnitude < comparison * comparison;
    }

    public void CalibrateArmAndHeight()
    {
        Vector3 headPos = headRef.position;
        Vector3 handPos = lhandRef.position;

        Vector3 lHandReal = interactionSync.GetLHandPos();
        Vector3 headReal = interactionSync.GetHeadPos();

        float prevMag = 0;
        int counter = 0;

        // While the head scale does not fit
        while (!WithinDistance(headPos, headReal, fitRadius))
        {
            if(counter >= breakCounter)
            {
                break;
            }

            float currSqrMag = (headPos - headReal).sqrMagnitude;

            // If the current magnitude is greater than the previous magnitude
            if (currSqrMag > prevMag * prevMag)
            {
                AdjustHeight(-1);
            }
            else
            {
                AdjustHeight(1);
            }

            prevMag = currSqrMag;
            counter++;
        }

        prevMag = 0;
        counter = 0;

        // While the lhand scale does not fit
        while (!WithinDistance(handPos, lHandReal, fitRadius))
        {
            if (counter >= breakCounter)
            {
                break;
            }

            float currSqrMag = (handPos - lHandReal).sqrMagnitude;

            // If the current magnitude is greater than the previous magnitude
            if (currSqrMag > prevMag * prevMag)
            {
                AdjustArms(-1);
            }
            else
            {
                AdjustArms(1);
            }

            prevMag = currSqrMag;
            counter++;
        }
    }
}
