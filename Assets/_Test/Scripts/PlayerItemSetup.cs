using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerItemSetup : MonoBehaviour
{
    [SerializeField] private Transform lHandTrans;
    [SerializeField] private Transform rHandTrans;
    [SerializeField] private Transform lThighTrans;
    [SerializeField] private Transform rThighTrans;
    [SerializeField] private Transform lThighSideTrans;
    [SerializeField] private Transform rThighSideTrans;
    [SerializeField] private Transform stomach;

    [SerializeField] private GameObject pistolPref;
    [SerializeField] private GameObject pistolFannyPref;
    [SerializeField] private GameObject riflePref;
    [SerializeField] private GameObject magazinePref;

    public void SpawnItem(AccessoriesHandler.Item item, AccessoriesHandler.Position position)
    {
        Transform spawnTrans = null;
        GameObject itemPref = null;

        switch (item)
        {
            case AccessoriesHandler.Item.PISTOL:
                itemPref = pistolPref;
                break;
            case AccessoriesHandler.Item.PISTOL_FANNY:
                itemPref = pistolFannyPref;
                break;
            case AccessoriesHandler.Item.RIFLE_HANDBAG:
                itemPref = riflePref;
                break;
            case AccessoriesHandler.Item.MAGAZINE:
                itemPref = magazinePref;
                break;
            default:
                break;
        }

        switch (position)
        {
            case AccessoriesHandler.Position.L_HAND:
                spawnTrans = lHandTrans;
                break;
            case AccessoriesHandler.Position.R_HAND:
                spawnTrans = rHandTrans;
                break;
            case AccessoriesHandler.Position.L_THIGH:
                spawnTrans = lThighTrans;
                break;
            case AccessoriesHandler.Position.R_THIGH:
                spawnTrans = rThighTrans;
                break;
            case AccessoriesHandler.Position.L_THIGHSIDE:
                spawnTrans = lThighSideTrans;
                break;
            case AccessoriesHandler.Position.R_THIGHSIDE:
                spawnTrans = rThighSideTrans;
                break;
            case AccessoriesHandler.Position.STOMACH:
                spawnTrans = stomach;
                break;
            default:
                break;
        }

        if(itemPref == null)
        {
            GameObject holsterItem = Instantiate(itemPref, spawnTrans);
            NetworkServer.Spawn(holsterItem);
            holsterItem.GetComponent<SyncParent>().RpcSetParent(spawnTrans.gameObject);
        }
    }

    public void ProcessAccessories(List<Accessory> accessories)
    {
        foreach(Accessory acc in accessories)
        {
            SpawnItem((AccessoriesHandler.Item)acc.GetItem(), (AccessoriesHandler.Position)acc.GetPosition());
        }
    }
}
