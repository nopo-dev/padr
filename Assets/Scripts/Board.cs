using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour {

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private GameObject[] _orbPrefabs;

    public int Width {
        get { return _width; }
        set { _width = value; }
    }

    public int Height {
        get { return _height; }
        set { _height = value; }
    }

    private GameObject[,] _tiles;
    private GameObject[,] _orbs;
    private GameObject _selectedOrb;
    private GameObject _currentTile;
    private GameObject _ghostOrb;

    public bool OrbSelected;

    private void Start() {
        _tiles = new GameObject[_width, _height];
        _orbs = new GameObject[_width, _height];
        InitializeGrid();
    }

    public void SelectOrb(Coords c) {
        _selectedOrb = _orbs[c.X, c.Y];
        _selectedOrb.GetComponent<Orb>().Selected = true;
        OrbSelected = true;
        SetCurrentTile(c);

        CreateGhostOrb(c);
    }

    private void CreateGhostOrb(Coords c) {
        _ghostOrb = Instantiate(_orbPrefabs[(int)_selectedOrb.GetComponent<Orb>().Type], transform);
        _ghostOrb.transform.localPosition = new Vector2((float)c.X, (float)c.Y);
        Color color = _ghostOrb.GetComponent<SpriteRenderer>().color;
        color.a = 0.25f;
        _ghostOrb.GetComponent<SpriteRenderer>().color = color;
    }

    private void SetCurrentTile(Coords c) {
        if (_currentTile != null) {
            _currentTile.GetComponent<BackgroundTile>().SetCollider(true);
        }
        _currentTile = _tiles[c.X, c.Y];
        _currentTile.GetComponent<BackgroundTile>().SetCollider(false);
    }

    public void DeselectOrb() {
        if (_selectedOrb == null)   return;
        _selectedOrb.GetComponent<Orb>().Selected = false;
        OrbSelected = false;
        
        _currentTile.GetComponent<BackgroundTile>().SetCollider(true);
        _currentTile = null;

        Coords currentPos = _selectedOrb.GetComponent<Orb>().Location;
        _selectedOrb.transform.localPosition = new Vector2((float)currentPos.X, (float)currentPos.Y);

        Destroy(_ghostOrb);
    }

    private Vector2 _upLeft = Vector2.left + Vector2.up;
    private Vector2 _downRight = Vector2.right + Vector2.down;

    public void MoveOrb(Coords c) {
        SetCurrentTile(c);
        Coords moveTo = _selectedOrb.GetComponent<Orb>().Location;
        _orbs[moveTo.X, moveTo.Y] = _orbs[c.X, c.Y];
        _orbs[c.X, c.Y].GetComponent<Orb>().Location = moveTo;

        Vector2 direction = new Vector2(moveTo.X - c.X, moveTo.Y - c.Y);
        if (direction == Vector2.left || direction == Vector2.down ||
            direction == -Vector2.one || direction == _upLeft) {
            _orbs[c.X, c.Y].GetComponent<Orb>().Reverse = true;
            _ghostOrb.GetComponent<Orb>().Reverse = true;
        } else {
            _orbs[c.X, c.Y].GetComponent<Orb>().Reverse = false;
            _ghostOrb.GetComponent<Orb>().Reverse = false;
        }
        _orbs[c.X, c.Y].GetComponent<Orb>().Move(direction);

        _ghostOrb.GetComponent<Orb>().Location = c;
        //_ghostOrb.GetComponent<Orb>().Move(-direction);
        _ghostOrb.transform.localPosition += new Vector3(-direction.x, -direction.y, 0);

        _selectedOrb.GetComponent<Orb>().Location = c;
        _orbs[c.X, c.Y] = _selectedOrb;
    }

    private void InitializeGrid() {
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                Vector2 position = new Vector2((float)col, (float)row);
                GameObject tile = Instantiate(_tilePrefab, transform);
                tile.transform.localPosition =  position;
                tile.transform.localRotation = Quaternion.identity;
                tile.GetComponent<BackgroundTile>().Location = new Coords(col, row);
                _tiles[col, row] = tile;

                int orbType = Random.Range(0, _orbPrefabs.Length);
                GameObject orb = Instantiate(_orbPrefabs[orbType], transform);
                orb.GetComponent<Orb>().Type = (OrbType)orbType;
                orb.transform.localPosition =  position;
                orb.transform.localRotation = Quaternion.identity;
                orb.GetComponent<Orb>().Location = new Coords(col, row);
                _orbs[col, row] = orb;
            }
        }
    }
}
