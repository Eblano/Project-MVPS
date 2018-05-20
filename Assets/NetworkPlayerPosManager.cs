using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SealTeam4
{
    public class NetworkPlayerPosManager : NetworkBehaviour
    {
        public static NetworkPlayerPosManager localInstance;

        private void Start()
        {
            if (localInstance == null && isLocalPlayer)
            {
                localInstance = this;
            }
        }

        [Command]
        // Add calibration point of 
        public void CmdAddCalibrationPointToGameManager(string playerName, Vector3 pointData)
        {
            if (GameManager.instance.calibrationMode && NetworkServer.active)
            {
                // If player not in data list
                if (!GameManager.instance.playerVectorCalibDataList.Exists(x => x.playerName == playerName))
                {
                    // Add new entry of player
                    GameManager.instance.playerVectorCalibDataList.Add(new PlayerVectorCalibData(playerName));
                    // Add the name and first point to the data
                    GameManager.instance.playerVectorCalibDataList.Find(x => x.playerName == playerName).point1 = pointData;
                }
                // If player exist but missing second point data
                else if (GameManager.instance.playerVectorCalibDataList.Find(x => x.playerName == playerName).point2 == Vector3.zero)
                {
                    // Add the second point to the data
                    GameManager.instance.playerVectorCalibDataList.Find(x => x.playerName == playerName).point2 = pointData;
                }
                // If both points exists
                else
                {
                    PlayerVectorCalibData playerVCalibData = 
                        GameManager.instance.playerVectorCalibDataList.Find(x => x.playerName == playerName);

                    playerVCalibData.point1 = pointData;
                    playerVCalibData.point2 = Vector3.zero;
                }
            }
        }

        [ClientRpc]
        // Calibrate vector of a player based on 4 points
        public void RpcCalibratePlayerVector()
        {
            Debug.Log("Calibrate");
            PlayerVectorCalibData referenceData = new PlayerVectorCalibData();
            PlayerVectorCalibData calibData = new PlayerVectorCalibData();

            foreach (PlayerVectorCalibData data in GameManager.instance.playerVectorCalibDataList)
            {
                if(data.point2 != null)
                {
                    if(referenceData.playerName == string.Empty)
                    {
                        referenceData.playerName = data.playerName;
                        referenceData.point1 = data.point1;
                        referenceData.point2 = data.point2;
                    }
                    else
                    {
                        calibData.playerName = data.playerName;
                        calibData.point1 = data.point1;
                        calibData.point2 = data.point2;
                    }
                }
            }

            if (referenceData.playerName != string.Empty && calibData.playerName != string.Empty)
            {
                Debug.Log("Calibration Target Player Name: " + calibData.playerName);
                Debug.Log("Current Player Name: " + GameManager.instance.localPlayerName);
                // If local player is the player that needs to be calibrated
                if (calibData.playerName == GameManager.instance.localPlayerName)
                {
                    Vector3 translationVector = calibData.point1 + referenceData.point1;
                    Quaternion rotationVector =
                    Quaternion.FromToRotation(
                        calibData.point2 - calibData.point1,
                        referenceData.point2 - referenceData.point1
                        );

                    // Apply the above 2 vector to the corresponding players
                    Transform localPlayerControllerT = GameObject.Find("LocalPlayerController(Clone)").transform;
                    localPlayerControllerT.Translate(translationVector);
                    localPlayerControllerT.Rotate(rotationVector.eulerAngles);

                    GameManager.instance.playerVectorCalibDataList.Clear();
                }
            }
        }
    }
}