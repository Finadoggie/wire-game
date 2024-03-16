using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static PresetContactFilters;

public class Wire : MonoBehaviour
{
    public enum State { off, on }

    [SerializeField] private Wire[] connectedWires;
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public State myState;

    [SerializeField] private Collider2D[] touchedColliders;

    public static Color[] wireColors = {
        new Color(0x88 / 255f, 0f, 0f),
        new Color(1f, 0.5f, 0.5f)
    };

    public static List<Wire> wireList = new List<Wire>();
    public static Wire[][] wireGroups;
    public static int wireCount = 0;

    private int id = 0;
    private int location = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        colliders = GetComponents<Collider2D>();
        connectedWires = new Wire[colliders.Length];

        touchedColliders = new Collider2D[colliders.Length];

        #region Get Connected Wires

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].OverlapCollider(WIRE_FILTER, touchedColliders);

            if(touchedColliders[0] != null) connectedWires[i] = touchedColliders[0].gameObject.GetComponent<Wire>();
        }

        #endregion

        id = wireCount++;
        gameObject.name = "wire " + id.ToString();

        wireList.Add(this);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void GroupWires()
    {
        Wire currentWire;
        List<Wire[]> tempWireGroups = new List<Wire[]>();

        for (int i = 0; wireList.Count > 0; i++)
        {
            List<Wire> tempWireList = new List<Wire>();

            currentWire = wireList[wireList.Count - 1];

            ConnectWires(tempWireList, currentWire, ref i);
            
            tempWireGroups.Add(tempWireList.ToArray());
        }
        wireGroups = tempWireGroups.ToArray();
    }

    private static void ConnectWires(List<Wire> list, Wire wire, ref int location)
    {
        if (!wireList.Contains(wire)) return;

        list.Add(wire);
        wire.location = location;
        wireList.Remove(wire);

        foreach (Wire sibling in wire.connectedWires)
        {
            ConnectWires(list, sibling, ref location);
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region Set color

        spriteRenderer.color = wireColors[(int)myState];

        #endregion

        
    }

    public void SetGroupState(State state)
    {
        foreach (Wire wire in wireGroups[location])
        {
            wire.SetState(state);
        }
    }

    public void SetState(State state, Wire origin = null)
    {
        myState = state;
    }
}
