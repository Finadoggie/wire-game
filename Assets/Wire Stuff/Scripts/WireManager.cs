using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WireManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        Wire.GroupWires();
    }
}
