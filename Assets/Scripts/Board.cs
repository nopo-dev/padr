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

    private BackgroundTile[,] _tiles;
    private GameObject[,] _orbs;

    private void Start() {
        _tiles = new BackgroundTile[_width, _height];
        _orbs = new GameObject[_width, _height];
        InitializeGrid();
    }

    private void InitializeGrid() {
        for (int col = 0; col < _width; col++) {
            for (int row = 0; row < _height; row++) {
                Vector2 position = new Vector2((float)col, (float)row);
                GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity, transform);
                tile.name = "( " + col + ", " + row + " )";

                int orbType = Random.Range(0, _orbTypes.Length);
                GameObject orb = Instantiate(_orbTypes[orbType], position, Quaternion.identity, transform);
                orb.name = tile.name;
                _orbs[col, row] = orb;
            }
        }
    }
}
