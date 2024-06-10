using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Coords {
    public int X;
    public int Y;

    public Coords(int x, int y) {
        X = x;
        Y = y;
    }

    public override string ToString() {
        return "( " + X + ", " + Y + " )";
    }
}

public class BackgroundTile : MonoBehaviour {

    public Coords Location;

    public void SetCollider(bool state) {
        gameObject.GetComponent<PolygonCollider2D>().enabled = state;
    }
}