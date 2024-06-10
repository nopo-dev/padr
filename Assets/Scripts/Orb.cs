using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbType {
    Red, Green, Blue, Yellow, Purple, Gray
}

public class Orb : MonoBehaviour {

    [SerializeField] private Vector3 _selectScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private Vector3 _defaultScale = new Vector3(0.95f, 0.95f, 1f);
    [SerializeField] private float _trackingSpeed = 50f;
    public bool Selected {
        get { return _selected; }
        set {
            _selected = value;
            if (_selected) {
                transform.localScale = _selectScale;
                _renderer.sortingOrder = 1;
                Color c = _renderer.color;
                c.a = 0.7f;
                _renderer.color = c;
            } else {
                transform.localScale = _defaultScale;
                _renderer.sortingOrder = 0;
                Color c = _renderer.color;
                c.a = 1f;
                _renderer.color = c;
            }
        }
    }

    public Coords Location;

    private bool _selected;
    private SpriteRenderer _renderer;

    private void Start() {
        _selected = false;
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (_selected) {
            transform.position = Vector2.Lerp(transform.position, 
                Camera.main.ScreenToWorldPoint(Input.mousePosition), Time.deltaTime * _trackingSpeed);
        }
    }

    private void Move(Vector2 direction) {

    }
}
