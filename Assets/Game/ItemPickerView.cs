using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemPickerView
{
    private Transform _t;
    //private Mesh _currentModel;
    //private Material _currentMaterial;

    [SerializeField] private GameObject _container;

    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private MeshFilter filter;

    [SerializeField] private List<GameItem_DATA> itemsData = new List<GameItem_DATA>();

    public bool carrying;
    
    public ItemPickerView(Transform transform)
    {
        _t = transform;
        _container = new GameObject("PickUpContainer");
        _container.transform.SetParent(_t);
        _container.transform.position = Vector3.zero + Vector3.up;
        _container.transform.rotation = Quaternion.identity;

        renderer = _container.AddComponent<MeshRenderer>();
        filter = _container.AddComponent<MeshFilter>();
        
        

    }

    public void SetCurrentModel(GameItem_DATA.ItemType itemdata)
    {
        GameItem_DATA data = GetItemDataFromLocalRegistry(itemdata);

        if (data == null) return;
        
        renderer.material = data.mat;
        filter.mesh = data.model;
        TurnOn();
    }

    private GameItem_DATA GetItemDataFromLocalRegistry(GameItem_DATA.ItemType itemdata)
    {
        for (int i = 0; i < itemsData.Count; i++)
        {
            if (itemdata.Equals(itemsData[i].type))
            {
                return itemsData[i];
            }
        }

        return null;
    }

    public void TurnOff()
    {
        carrying = false;
        _container.SetActive(false);
    }
    
    public void TurnOn()
    {
        carrying = true;
        _container.SetActive(true);
    }

    public void ReleaseItem()
    {
        renderer.material = null;
        filter.mesh = null;
        TurnOff();
    }

    public void SetItemRegistry(GameController_FA gameController)
    {
        itemsData = gameController.GetGameItems;
    }
}
