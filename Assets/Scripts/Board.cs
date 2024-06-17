using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState {
    Interactable, Uninteractable
}

public class Board : MonoBehaviour {

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private GameObject[] _orbPrefabs;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private CombatManager _combatManager;

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
    public BoardState State;

    private void Start() {
        _tiles = new GameObject[_width, _height];
        _orbs = new GameObject[_width, _height];
        InitializeGrid();
        State = BoardState.Uninteractable;
        _fadeTime = 1f / _orbs[0, 0].GetComponent<Orb>().FadeSpeed;
        _fallTime = 1f / _orbs[0, 0].GetComponent<Orb>().FallSpeed;
        StartCoroutine(DelayAllowInteraction());
    }

    private IEnumerator DelayAllowInteraction() {
        yield return new WaitForSeconds(_fallTime + 0.05f);
        State = BoardState.Interactable;
    }

    private float _fallTime;
    public void SelectOrb(Coords c) {
        _selectedOrb = _orbs[c.X, c.Y];
        _selectedOrb.GetComponent<Orb>().State = OrbState.Selected;
        OrbSelected = true;
        SetCurrentTile(c);

        CreateGhostOrb(c);
    }

    private void CreateGhostOrb(Coords c) {
        _ghostOrb = Instantiate(_orbPrefabs[(int)_selectedOrb.GetComponent<Orb>().Type - 1], transform);
        _ghostOrb.transform.localPosition = new Vector2((float)c.X, (float)c.Y);
        
        _ghostOrb.GetComponent<Orb>().State = OrbState.Ghost;
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
        _selectedOrb.GetComponent<Orb>().State = OrbState.Unmatched;
        OrbSelected = false;
        
        _currentTile.GetComponent<BackgroundTile>().SetCollider(true);
        _currentTile = null;

        Coords currentPos = _selectedOrb.GetComponent<Orb>().Location;
        _selectedOrb.transform.localPosition = new Vector2((float)currentPos.X, (float)currentPos.Y);

        Destroy(_ghostOrb);

        if (_orbMoved) {
            CheckForMatches();
            _orbMoved = false;
        }
    }

    private bool _orbMoved = false;
    private Vector2 _upLeft = Vector2.left + Vector2.up;

    public void MoveOrb(Coords c) {
        _orbMoved = true;
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
                PlaceTile(col, row);
                SpawnRandomOrb(col, row, false);
            }
        }
    }

    private void PlaceTile(int col, int row) {
        Vector2 position = new Vector2((float)col, (float)row);

        GameObject tile = Instantiate(_tilePrefab, transform);
        tile.transform.localPosition = position;
        tile.transform.localRotation = Quaternion.identity;
        tile.GetComponent<BackgroundTile>().Location = new Coords(col, row);
        tile.GetComponent<BackgroundTile>().SetSprite();
        _tiles[col, row] = tile;
    }

    // needs to be optimized when not allowing matches, currently stuck in
    // while loop for a little too long when generating initial board
    private void SpawnRandomOrb(int col, int row, bool allowMatches) {
        Vector2 position = new Vector2((float)col, (float)row);
        int orbType = UnityEngine.Random.Range(1, _orbPrefabs.Length + 1);
        
        GameObject orb = Instantiate(_orbPrefabs[orbType - 1], transform);
        orb.GetComponent<Orb>().Type = (OrbType)orbType;
        orb.GetComponent<Orb>().Location = new Coords(col, row);
        if (!allowMatches) {
            while (IsValid(col, row, orb.GetComponent<Orb>().Type)) {
                orb.GetComponent<Orb>().Type = (OrbType)UnityEngine.Random.Range(1, 6);
            }
            orbType = (int)orb.GetComponent<Orb>().Type;
            Destroy(orb);
            orb = Instantiate(_orbPrefabs[orbType - 1], transform);
            orb.GetComponent<Orb>().Type = (OrbType)orbType;
            orb.GetComponent<Orb>().Location = new Coords(col, row);
        }
        position.y = GetLowestSpawnPos(col);
        _orbs[col, row] = orb;
        orb.transform.localPosition = position;
        orb.transform.localRotation = Quaternion.identity;
        orb.GetComponent<Orb>().State = OrbState.Skyfall;
    }

    private int _lastCol;
    private int _numSpawnedInCol;
    private float GetLowestSpawnPos(int col) {
        if (_lastCol != col)
            _numSpawnedInCol = 0;
        _lastCol = col;
        for (int row = 0; row < _height; row++) {
            if (_orbs[col, row] == null) {
                _numSpawnedInCol++;
                return (float)_height + _numSpawnedInCol - 1;
            }
        }
        return -1f;
    }
    
    private void SpawnSkyfall() {
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                if (_orbs[col, row] == null) {
                    SpawnRandomOrb(col, row, true);
                }
            }
        }
        StartCoroutine(CheckMatchesAfterSkyfall());
    }

    private IEnumerator CheckMatchesAfterSkyfall() {
        yield return new WaitForSeconds(_fallTime + 0.05f);
        _shouldSkyfall = false;
        StartCoroutine(CheckMatches());
    }

    private int _combo;
    private float _fadeTime;
    private void CheckForMatches() {
        _combo = 0;
        _shouldSkyfall = false;
        _combatManager.ResetUnitDamageUI();
        StartCoroutine(CheckMatches());
    }
    
    private bool _shouldSkyfall;
    private IEnumerator CheckMatches() {
        for (int row = 0; row < _height; row++) {
            for (int col = 0; col < _width; col++) {
                FloodFill(col, row);
                if (_matched) {
                    State = BoardState.Uninteractable;
                    _shouldSkyfall = true;
                    DeleteMatches();
                    yield return new WaitForSeconds(_fadeTime + 0.05f);
                } else {
                    yield return null;
                }
            }
        }
        UpdateUnmatchedOrbs();
        if (_shouldSkyfall) SpawnSkyfall();
        else State = BoardState.Interactable;
    }

    private void DeleteMatches() {
        int numConnected = 0;
        OrbType t = 0;
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                if (_orbMatched[col, row] == 1) {
                    t = _orbs[col, row].GetComponent<Orb>().Type;
                    _orbs[col, row].GetComponent<Orb>().State = OrbState.Matched;
                    numConnected++;
                }
            }
        }
        _combo++;
        _combatManager.CalculateMatchDamage(t, numConnected);
        _uiManager.UpdateTextField(TextField.Combo, _combo);
    }

    private void UpdateUnmatchedOrbs() {
        for (int col = 0; col < _width; col++) {
            int numMatchedInCol = 0;
            for (int row = 0; row < _height; row++) {
                if (_orbs[col, row] == null) {
                    numMatchedInCol++;
                } else {
                    if (numMatchedInCol == 0) continue;
                    _orbs[col, row].GetComponent<Orb>().Location.Y -= numMatchedInCol;
                    Coords newLoc = _orbs[col, row].GetComponent<Orb>().Location;

                    _orbs[newLoc.X, newLoc.Y] = _orbs[col, row];
                    _orbs[col, row] = null;
                    _orbs[newLoc.X, newLoc.Y].GetComponent<Orb>().State = OrbState.Skyfall;
                }
            }
        }
    }

    private int[,] _orbMatched;
    private bool[,] _orbVisited;
    private bool _matched;

    // flood fill, queue connected orbs if they are valid matches
    private void FloodFill(int col, int row) {
        _orbMatched = new int[_width, _height];
        _orbVisited = new bool[_width, _height];
        _matched = false;

        if (_orbs[col, row] == null)
            return;

        OrbType t = _orbs[col, row].GetComponent<Orb>().Type;
        if (!IsValid(col, row, t) || _orbs[col, row].GetComponent<Orb>().State == OrbState.Matched)
            return;
        
        _matched = true;
        Queue<int[]> orbsToCheck = new Queue<int[]>();
        int[] start = {col, row};
        orbsToCheck.Enqueue(start);
        _orbVisited[col, row] = true;

        int[,] delta = {{1, 0}, {-1, 0}, {0, 1}, {0, -1}};

        while (orbsToCheck.Count != 0) {
            int[] loc = orbsToCheck.Dequeue();
            _orbMatched[loc[0], loc[1]] = 1;

            for (int i = 0; i < 4; i++) {
                int[] nextLoc = {loc[0] + delta[i, 0], loc[1] + delta[i, 1]};

                if (IsConnected(nextLoc[0], nextLoc[1], t) && IsValid(nextLoc[0], nextLoc[1], t)) {
                    _orbVisited[nextLoc[0], nextLoc[1]] = true;
                    orbsToCheck.Enqueue(nextLoc);
                }
            }
        }
    }

    // check if next orb is connected
    private bool IsConnected(int col, int row, OrbType t) {
        if (col < 0 || col >= _width || row < 0 || row >= _height)
            return false;
        if (_orbVisited[col, row])
            return false;
        if (_orbs[col, row] == null)
            return false;
        if (_orbs[col, row].GetComponent<Orb>().Type != t)
            return false;
        
        return true;
    }

    // check whether any given orb is a valid part of a match
    // (must be connected in a horiz. or vert. line to 2+ orbs of the same type)
    private bool IsValid(int col, int row, OrbType t) {
        // check vertical
        int numStraight = 1;
        for (int r = row + 1; r < _height; r++) {
            if (_orbs[col, r] == null)  break;
            if (_orbs[col, r].GetComponent<Orb>().Type != t) break;
            numStraight++;
            if (numStraight >= 3)
                return true;
        }
        for (int r = row - 1; r >= 0; r--) {
            if (_orbs[col, r] == null)  break;
            if (_orbs[col, r].GetComponent<Orb>().Type != t) break;
            numStraight++;
            if (numStraight >= 3)
                return true;
        }

        // check horizontal
        numStraight = 1;
        for (int c = col + 1; c < _width; c++) {
            if (_orbs[c, row] == null)  break;
            if (_orbs[c, row].GetComponent<Orb>().Type != t) break;
            numStraight++;
            if (numStraight >= 3)
                return true;
        }
        for (int c = col - 1; c >= 0; c--) {
            if (_orbs[c, row] == null)  break;
            if (_orbs[c, row].GetComponent<Orb>().Type != t) break;
            numStraight++;
            if (numStraight >= 3)
                return true;
        }

        return false;
    }
}