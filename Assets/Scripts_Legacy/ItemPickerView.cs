using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickerView
{
    private Transform _t;
    //private Mesh _currentModel;
    //private Material _currentMaterial;

    private GameObject _container;

    private MeshRenderer renderer;
    private MeshFilter filter;
    
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

    // public void OnUpdate()
    // {
    //     renderer.material = _currentMaterial;
    // }

    public void SetCurrentModel(GameItem_DATA itemdata)
    {
        renderer.material = itemdata.mat;
        filter.mesh = itemdata.model;
    }

    public void TurnOff()
    {
        _container.SetActive(false);
    }
    
    public void TurnOn()
    {
        _container.SetActive(true);
    }
    
}
