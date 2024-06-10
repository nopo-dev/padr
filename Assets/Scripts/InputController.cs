using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    
    [SerializeField] private Board _board;

    private void Start() {

    }

    private Collider2D CheckTileCollider() {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        return hit.collider;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Collider2D col = CheckTileCollider();
            if (col != null) {
                Coords tileLocation = col.gameObject.GetComponent<BackgroundTile>().Location;
                _board.SelectOrb(tileLocation);
            }
        }
        if (Input.GetMouseButtonUp(0) && _board.OrbSelected) {
            _board.DeselectOrb();
        }
        if (Input.GetMouseButton(0) && _board.OrbSelected) {
            Collider2D col = CheckTileCollider();
            if (col != null) {
                Coords tileLocation = col.gameObject.GetComponent<BackgroundTile>().Location;
                _board.MoveOrb(tileLocation);
            }
        }
    }

    // todo: lock orb movement to board
    //       move orbs according to selected orb, not cursor
    //       ghost orb in current tile
    //       orb movement animation

    // todo: matches and combos
}
