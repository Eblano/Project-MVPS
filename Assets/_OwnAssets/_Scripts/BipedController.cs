using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class BipedController : MonoBehaviour
{
    private FullBodyBipedIK fbBipedIK;
    public enum BipedPosition { LHAND, RHAND, LSHOULD, RSHOULD };

    private void Start()
    {
        fbBipedIK = GetComponent<FullBodyBipedIK>();
    }

    public void SetBiped(BipedPosition bipedPosition, Transform target, float weight)
    {
        switch (bipedPosition)
        {
            case BipedPosition.LHAND:
                SetLeftHandBiped(target, weight);
                break;
            case BipedPosition.RHAND:
                SetRightHandBiped(target, weight);
                break;
            case BipedPosition.LSHOULD:
                SetLeftShouldersBiped(target, weight);
                break;
            case BipedPosition.RSHOULD:
                SetRightShouldersBiped(target, weight);
                break;
            default:
                break;
        }
    }

    public Transform GetBipedPos(BipedPosition bipedPosition)
    {
        switch (bipedPosition)
        {
            case BipedPosition.LHAND:
                return fbBipedIK.solver.leftHandEffector.target;
            case BipedPosition.RHAND:
                return fbBipedIK.solver.rightHandEffector.target;
            case BipedPosition.LSHOULD:
                return fbBipedIK.solver.leftShoulderEffector.target;
            case BipedPosition.RSHOULD:
                return fbBipedIK.solver.rightShoulderEffector.target;
            default:
                break;
        }

        return null;
    }

    private void SetLeftShouldersBiped(Transform target, float weight)
    {
        fbBipedIK.solver.leftShoulderEffector.target = target;
        fbBipedIK.solver.leftShoulderEffector.positionWeight = weight;
    }

    private void SetLeftHandBiped(Transform target, float weight)
    {
        fbBipedIK.solver.leftHandEffector.target = target;
        fbBipedIK.solver.leftHandEffector.positionWeight = weight;
    }

    private void SetRightShouldersBiped(Transform target, float weight)
    {
        fbBipedIK.solver.rightShoulderEffector.target = target;
        fbBipedIK.solver.rightShoulderEffector.positionWeight = weight;
    }

    private void SetRightHandBiped(Transform target, float weight)
    {
        fbBipedIK.solver.rightHandEffector.target = target;
        fbBipedIK.solver.rightHandEffector.positionWeight = weight;
    }

    //private void SetHeadBiped(Transform target, float weight)
    //{
    //    fbBipedIK.solver.headMapping.bone = target;
    //    fbBipedIK.solver.headMapping.maintainRotationWeight = weight;
    //}
}
