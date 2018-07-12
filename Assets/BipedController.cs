using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class BipedController : MonoBehaviour
{
    private BipedIK bipedIK;
    public enum BipedPosition { SPINE, LHAND, RHAND };

    private void Start()
    {
        bipedIK = GetComponent<BipedIK>();
    }

    public void SetBiped(BipedPosition bipedPosition, Transform target, float weight)
    {
        switch (bipedPosition)
        {
            case BipedPosition.SPINE:
                SetSpineBiped(target, weight);
                break;
            case BipedPosition.LHAND:
                SetLeftHandBiped(target, weight);
                break;
            case BipedPosition.RHAND:
                SetRightHandBiped(target, weight);
                break;
            default:
                break;
        }
    }

    private void SetLeftHandBiped(Transform target, float weight)
    {
        bipedIK.solvers.leftHand.target = target;
        bipedIK.solvers.leftHand.IKPositionWeight = weight;
    }

    private void SetRightHandBiped(Transform target, float weight)
    {
        bipedIK.solvers.rightHand.target = target;
        bipedIK.solvers.rightHand.IKPositionWeight = weight;
    }

    private void SetSpineBiped(Transform target, float weight)
    {
        bipedIK.solvers.spine.target = target;
        bipedIK.solvers.spine.IKPositionWeight = weight;
    }
}
