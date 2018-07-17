using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using SealTeam4;

public class PlayerSizeCalibration : MonoBehaviour
{
    [SerializeField] private float scalePercIncrement = 0.01f;
    [SerializeField] private Transform ulArmBone, llArmBone, urArmBone, lrArmBone;
    [SerializeField] private Transform lhandRef, headRef;
    [SerializeField] private float fitRadius = 0.02f;
    [SerializeField] private VRIK vrIK;
    private VRIK.References vrIKRefs;
    private PlayerInteractionSync interactionSync;
    private float heightScale, armScale;
    [SerializeField] private int breakCounter = 10;
    public static PlayerSizeCalibration instance;

    private Vector3 originalHead;
    private Vector3 originalHand;

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
        originalHead = transform.localScale;
        originalHand = llArmBone.localScale;
    }

    public void AdjustHeight(int multiplier)
    {
        heightScale = transform.localScale.y + (scalePercIncrement * multiplier);
        transform.localScale = new Vector3(heightScale, heightScale, heightScale);
    }

    public void AdjustArms(int multiplier)
    {
        armScale = llArmBone.localScale.y + (scalePercIncrement * multiplier);
        ulArmBone.localScale = llArmBone.localScale = urArmBone.localScale = lrArmBone.localScale = new Vector3(armScale, armScale, armScale);
    }

    private bool WithinDistance(Vector3 a, Vector3 b, float comparison)
    {
        Debug.Log("Magnitude"+(a - b).magnitude);
        return (a - b).magnitude < comparison;
    }

    public void ResetArmAndHeight()
    {
        transform.localScale = originalHead;
        ulArmBone.localScale = llArmBone.localScale = urArmBone.localScale = lrArmBone.localScale = originalHand;
    }

    public IEnumerator CalibrateArmAndHeight()
    {
        Vector3 headPos = headRef.position;
        Vector3 handPos = lhandRef.position;

        Vector3 lHandReal = interactionSync.GetLHandPos().position;
        Vector3 headReal = interactionSync.GetHeadPos().position;

        Debug.Log("Hand: " + lHandReal);
        Debug.Log("Head: " + headReal);

        float prevSqrMag = 0;
        int counter = 0;

        bool flip = false;
        
        // While the lhand scale does not fit
        while (!WithinDistance(handPos, lHandReal, fitRadius))
        {
            if (counter >= breakCounter)
            {
                Debug.Log("Arm Counter Broke");
                break;
            }

            handPos = lhandRef.position;
            float currSqrMag = (lHandReal - handPos).sqrMagnitude;

            Debug.Log("Arm Curr: " + currSqrMag + "Prev: " + prevSqrMag + ", Is CurrMag more than PrevMag:" + (currSqrMag > prevSqrMag));

            // If the current magnitude is greater than the previous magnitude
            if (currSqrMag > prevSqrMag)
            {
                flip = !flip;
            }

            if (flip)
            {
                AdjustArms(-1);
                Debug.Log("Decrease Size(Arm)");
            }
            else
            {
                AdjustArms(1);
                Debug.Log("Increase Size(Arm)");
            }

            prevSqrMag = currSqrMag;
            counter++;

            yield return new WaitForSeconds(0.01f);
        }
        
        prevSqrMag = 0;
        counter = 0;

        // While the head scale does not fit
        while (!WithinDistance(headPos, headReal, fitRadius))
        {
            if (counter >= breakCounter)
            {
                Debug.Log("Head Counter Broke");
                break;
            }

            headPos = headRef.position;
            float currSqrMag = (headReal - headPos).sqrMagnitude;

            Debug.Log("Height Curr: " + currSqrMag + "Prev: " + prevSqrMag + "Is CurrMag more than PrevMag:" + (currSqrMag > prevSqrMag));

            // If the current magnitude is greater than the previous magnitude
            if (currSqrMag > prevSqrMag)
            {
                flip = !flip;
            }

            if (flip)
            {
                AdjustHeight(-1);
                Debug.Log("Decrease Size(Height)");
            }
            else
            {
                AdjustHeight(1);
                Debug.Log("Increase Size(Height)");
            }

            prevSqrMag = currSqrMag;
            counter++;

            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }
}
