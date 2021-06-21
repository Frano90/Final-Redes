using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LobbySelectorData : ScriptableObject
{
    public Sprite portrait;
    public Team team;
    
    public enum Team
    {
        cat,
        rat,
        undefined
    }
}
