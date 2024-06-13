using System;
using System.Collections;
using System.Collections.Generic;
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

        CheckForMatches();
    }

    private Vector2 _upLeft = Vector2.left + Vector2.up;

    public void MoveOrb(Coords c) {
        SetCurrentTile(c);
        Coords moveTo = _selectedOrb.GetComponent<Orb>().Location;
        _orbs[moveTo.X, moveTo.Y] = _orbs[c.X, c.Y];
        _orbs[c.X, c.Y].GetComponent<Orb>().Location = moveTo;

        Vector2 direction = new Vector2(moveTo.X - c.X, moveTo.Y - c.Y);
        if (direction == Vector2.left || direction == Vector2.down ||
            direction == -Vector2.one || direction == _upLeft) {
            _orbs[c.X, c.Y].GetComponent<Orb>().Reverse = true;
            // _ghostOrb.GetComponent<Orb>().Reverse = true;
        } else {
            _orbs[c.X, c.Y].GetComponent<Orb>().Reverse = false;
            // _ghostOrb.GetComponent<Orb>().Reverse = false;
        }
        _orbs[c.X, c.Y].GetComponent<Orb>().Move(direction, true);

        _ghostOrb.GetComponent<Orb>().Location = c;
        //_ghostOrb.GetComponent<Orb>().Move(-direction);
        //_ghostOrb.transform.localPosition += new Vector3(-direction.x, -direction.y, 0);
        _ghostOrb.GetComponent<Orb>().Move(-direction, false);

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

                int orbType = UnityEngine.Random.Range(0, _orbPrefabs.Length);
                GameObject orb = Instantiate(_orbPrefabs[orbType], transform);
                orb.GetComponent<Orb>().Type = (OrbType)orbType;
                orb.transform.localPosition =  position;
                orb.transform.localRotation = Quaternion.identity;
                orb.GetComponent<Orb>().Location = new Coords(col, row);
                _orbs[col, row] = orb;
            }
        }
    }

    /*
        todo: matches
              combos
              initial board fill (no matches)
              skyfall (can have matches)

              can tune orb movement + diagonal window a bit if needed
    */

    // detecting matches:
    // an orb is part of a match if it is part of a set of orbs forming a row or column of
    // at least 3 orbs. a set of connected orbs composed of multiple such rows / columns
    // is counted as a single match

    private int _combo;

    // implementation could be optimized (lol)
    private void CheckForMatches() {
        _combo = 0;
        //_orbMatched = new int[_width, _height];
        //int[,] orbTypes = new int[_width, _height];

        // iterate through every orb
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                FloodFill(col, row);
                ValidateMatches();
                DeleteMatches();
            }
        }
    }

    private void DeleteMatches() {
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                if (_orbMatched[col, row] == 1) {
                    _orbs[col, row].GetComponent<Orb>().Matched = true;
                }
            }
        }
    }

    private int[,] _orbMatched;
    private bool[,] _orbVisited;
    // mark blob of connected orbs (perhaps a match)
    private void FloodFill(int col, int row) {
        if (_orbs[col, row] == null)
            return;

        // type of current orb
        OrbType t = _orbs[col, row].GetComponent<Orb>().Type;
        _orbMatched = new int[_width, _height];
        _orbVisited = new bool[_width, _height];

        Queue<int[]> orbsToCheck = new Queue<int[]>();
        int[] start = {col, row};
        orbsToCheck.Enqueue(start);

        _orbVisited[col, row] = true;

        int[,] delta = {{1, 0}, {-1, 0},
                        {0, 1}, {0, -1}};

        while (orbsToCheck.Count != 0) {
            int[] loc = orbsToCheck.Dequeue();
            _orbMatched[loc[0], loc[1]] = 1;

            for (int i = 0; i < 4; i++) {
                int[] nextLoc = {loc[0] + delta[i, 0], loc[1] + delta[i, 1]};

                if (IsConnected(nextLoc[0], nextLoc[1], t)) {
                    _orbVisited[nextLoc[0], nextLoc[1]] = true;
                    orbsToCheck.Enqueue(nextLoc);
                }
            }
        }
    }

    private bool IsConnected(int col, int row, OrbType t) {
        if (col < 0 || col >= _width || row < 0 || row >= _height)
            return false;
        if (_orbVisited[col, row])
            return false;
        if (_orbs[col, row].GetComponent<Orb>().Type != t)
            return false;
        
        return true;
    }

    private void ValidateMatches() {
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                if (!IsValid(col, row)) {
                    _orbMatched[col, row] = 0;
                }
            }
        }
    }

    // check whether any given orb is a valid part of a match
    private bool IsValid(int col, int row) {
        // check up
        int numStraight = 1;
        for (int r = row + 1; r < _height; r++) {
            if (_orbMatched[col, r] != 1) {
                break;
            }
            numStraight++;
            if (numStraight >= 3)   return true;
        }
        for (int r = row - 1; r >= 0; r--) {
            if (_orbMatched[col, r] != 1) {
                break;
            }
            numStraight++;
            if (numStraight >= 3)   return true;
        }

        // check right
        numStraight = 1;
        for (int c = col + 1; c < _width; c++) {
            if (_orbMatched[c, row] != 1) {
                break;
            }
            numStraight++;
            if (numStraight >= 3)   return true;
        }
        for (int c = col - 1; c >= 0; c--) {
            if (_orbMatched[c, row] != 1) {
                break;
            }
            numStraight++;
            if (numStraight >= 3)   return true;
        }

        return false;
    }
}