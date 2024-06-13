using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    
    [SerializeField] private Board _board;
    [SerializeField] private GameObject _followerPrefab;

    private Bounds _bounds;

    private GameObject _follower = null;

    private void Start() {
        Vector3 boundsCenter = new Vector3(_board.transform.position.x + _board.Width * 0.5f - 0.5f,
            _board.transform.position.y + _board.Height * 0.5f - 0.5f, 0);
        Vector3 boundsSize = new Vector3(_board.Width - 0.1f, _board.Height - 0.1f);
        _bounds = new Bounds(boundsCenter, boundsSize);
    }

    private Collider2D CheckTileCollider() {
        Vector2 worldPoint;
        if (_follower == null) {
            worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else {
            worldPoint = _follower.transform.position;
        }
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        return hit.collider;
    }

    [SerializeField] private float _followerSpeed = 25f;

    private void MoveFollower() {
        Vector3 moveTo = _bounds.ClosestPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 direction = moveTo - _follower.transform.position;
        _follower.transform.position += direction * Time.deltaTime * _followerSpeed;
    }

    private void Update() {
        if (_follower != null && Input.GetMouseButton(0)) {
            MoveFollower();
        }
        if (Input.GetMouseButtonDown(0)) {
            Collider2D col = CheckTileCollider();
            if (col != null) {
                Coords tileLocation = col.gameObject.GetComponent<BackgroundTile>().Location;
                _board.SelectOrb(tileLocation);
                _follower = Instantiate(_followerPrefab,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            }
        }
        if (Input.GetMouseButtonUp(0) && _board.OrbSelected) {
            _board.DeselectOrb();
            Destroy(_follower); 
        }
        if (Input.GetMouseButton(0) && _board.OrbSelected) {
            Collider2D col = CheckTileCollider();
            if (col != null) {
                Coords tileLocation = col.gameObject.GetComponent<BackgroundTile>().Location;
                _board.MoveOrb(tileLocation);
            }
        }
    }
}
