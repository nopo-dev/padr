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

    [SerializeField] private Sprite _bg0;
    [SerializeField] private Sprite _bg1;

    public Coords Location;

    public void SetSprite() {
        GetComponent<SpriteRenderer>().sprite = (Location.X + Location.Y) % 2 == 0 ? _bg0 : _bg1;
    }

    public void SetCollider(bool state) {
        gameObject.GetComponent<PolygonCollider2D>().enabled = state;
    }
}