using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesHandler : MonoBehaviour
{
    [SerializeField] private List<Accessory> accessories;
    [SerializeField] private GameObject accessoryPnlPrefab;
    public static AccessoriesHandler instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        accessories = new List<Accessory>();
    }

    public void OnAddAccesory()
    {
        Accessory newAccesory = new Accessory(Accessory.Item.DEFAULT, Accessory.Position.DEFAULT);
        accessories.Add(newAccesory);
        GameObject pnlAccessory = Instantiate(accessoryPnlPrefab);
        pnlAccessory.GetComponent<AccessoryPanel>().InitialisePanel(ref newAccesory, transform);
    }

    public void OnDeleteAccessory(Accessory removeAcc)
    {
        accessories.Remove(removeAcc);
    }
}
