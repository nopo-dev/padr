using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private GameObject[] _orbTypes;

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
    }

    public void MoveOrb(Coords c) {
        SetCurrentTile(c);
        Coords moveTo = _selectedOrb.GetComponent<Orb>().Location;
        _orbs[moveTo.X, moveTo.Y] = _orbs[c.X, c.Y];
        _orbs[c.X, c.Y].GetComponent<Orb>().Location = moveTo;
        _orbs[c.X, c.Y].transform.localPosition = new Vector2((float)moveTo.X, (float)moveTo.Y);

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

                int orbType = Random.Range(0, _orbTypes.Length);
                GameObject orb = Instantiate(_orbTypes[orbType], transform);
                orb.transform.localPosition =  position;
                orb.transform.localRotation = Quaternion.identity;
                orb.GetComponent<Orb>().Location = new Coords(col, row);
                _orbs[col, row] = orb;
            }
        }
    }
}
