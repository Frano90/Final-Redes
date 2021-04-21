using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameItem", menuName = "GameItems")]
public class GameItem_DATA : ScriptableObject
{
    public ItemType type;
    public Mesh model;
    public Material mat;
    
    public enum ItemType
    {
        cheese
    }
}
