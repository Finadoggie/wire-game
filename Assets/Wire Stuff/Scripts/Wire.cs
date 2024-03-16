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
    [SerializeField] private BoxCollider2D[] colliders;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public State myState;

    [SerializeField] private BoxCollider2D[] touchedColliders;

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

        colliders = GetComponents<BoxCollider2D>();
        connectedWires = new Wire[colliders.Length];

        touchedColliders = new BoxCollider2D[colliders.Length];

        #region Get Connected Wires

        

        #endregion

        id = wireCount++;
        gameObject.name = "wire " + id.ToString();

        wireList.Add(this);
    }

    private void findSiblings()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].OverlapCollider(WIRE_FILTER, touchedColliders);

            if (touchedColliders[0] != null) connectedWires[i] = touchedColliders[0].gameObject.GetComponent<Wire>();
        }
    }

    public static void GroupWires()
    {
        wireList.ForEach(wire => { wire.findSiblings(); });

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

        foreach (var group in wireGroups) foreach (var wire in group) Debug.Log(wire);
    }

    private static void ConnectWires(List<Wire> list, Wire wire, ref int location)
    {
        if (!wireList.Contains(wire)) return;

        list.Add(wire);
        wire.location = location;
        wireList.Remove(wire);

        //Debug.Log(wire.getSiblingDebug());

        foreach (Wire sibling in wire.connectedWires)
        {
            ConnectWires(list, sibling, ref location);
        }
    }

    private string getSiblingDebug()
    {
        string s = this.name + ": ";
        foreach (Wire sibling in this.connectedWires)
        {
            s += sibling.name + " ";
        }
        return s;
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
