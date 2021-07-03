using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedOffsetTransform : MonoBehaviour
{
    [SerializeField] private Transform objetToUpdate;
    [SerializeField] private Transform targetObject;
    [SerializeField] private float offsetDistance;

    // Update is called once per frame
    void FixedUpdate()
    {
        objetToUpdate.position = targetObject.position + offsetDistance * Vector3.up;
    }
}
