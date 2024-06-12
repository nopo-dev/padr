using System;
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
    [SerializeField] private float _moveSpeed = 25f;
    public bool Selected {
        get { return _selected; }
        set {
            _selected = value;
            if (_selected) {
                transform.localScale = _selectScale;
                _renderer.sortingOrder = 2;
                Color c = _renderer.color;
                c.a = 0.7f;
                _renderer.color = c;
            } else {
                transform.localScale = _defaultScale;
                // if (_renderer == null)  _renderer = GetComponent<SpriteRenderer>();
                _renderer.sortingOrder = 0;
                Color c = _renderer.color;
                c.a = 1f;
                _renderer.color = c;
            }
        }
    }

    public OrbType Type;

    public Coords Location;

    private bool _selected;
    private bool _isMoving;
    private Vector2 _moveTo;
    private IEnumerator _moveCoroutine;
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

    public void Move(Vector2 direction) {
        _renderer.sortingOrder = 1;
        if (_isMoving && _moveCoroutine != null) {
            _isMoving = false;
            StopCoroutine(_moveCoroutine);
            _renderer.sortingOrder = 0;
            transform.localPosition = _moveTo;
        }
        _moveTo = new Vector2(transform.localPosition.x, transform.localPosition.y) + direction;

        _moveCoroutine = MoveOrb(direction);
        StartCoroutine(_moveCoroutine);
    }

    private float diagonalOffset = (float)Math.Sqrt(2) * 0.5f;
    private float quarterPi = (float)Math.PI * 0.25f;
    private Vector2 _upLeft = Vector2.left + Vector2.up;
    private Vector2 _downRight = Vector2.right + Vector2.down;
    public bool Reverse = false;

    private IEnumerator MoveOrb(Vector2 direction) {
        _isMoving = true;
        Vector2 startPos = transform.localPosition;
        float tStart = 0f, tEnd = 0f;
        float xOffset = 0f, yOffset = 0f;

        if (direction == Vector2.right && !Reverse || (direction == Vector2.left && Reverse)) {
            tStart = quarterPi * -4f;
            tEnd = 0f;
            xOffset = (direction == Vector2.right) ? 1f : -1f;
        } else if (direction == Vector2.up && !Reverse || (direction == Vector2.down && Reverse)) {
            tStart = quarterPi * -2f;
            tEnd = -tStart;
            yOffset = (direction == Vector2.up) ? 1f : -1f;
        } else if (direction == Vector2.left && !Reverse || (direction == Vector2.right && Reverse)) {
            tStart = 0f;
            tEnd = quarterPi * 4f;
            xOffset = (direction == Vector2.right) ? 1f : -1f;
        } else if (direction == Vector2.down && !Reverse || (direction == Vector2.up && Reverse)) {
            tStart = quarterPi * 2f;
            tEnd = quarterPi * 6f;
            yOffset = (direction == Vector2.up) ? 1f : -1f;
        } else if (direction == Vector2.one && !Reverse || (direction == -Vector2.one && Reverse)) {
            tStart = -quarterPi * 3f;
            tEnd = quarterPi;
            xOffset = (direction == Vector2.one) ? diagonalOffset : -diagonalOffset;
            yOffset = (direction == Vector2.one) ? diagonalOffset : -diagonalOffset;
        } else if (direction == -Vector2.one && !Reverse || (direction == Vector2.one && Reverse)) {
            tStart = quarterPi;
            tEnd = quarterPi * 5f;
            xOffset = (direction == Vector2.one) ? diagonalOffset : -diagonalOffset;
            yOffset = (direction == Vector2.one) ? diagonalOffset : -diagonalOffset;
        } else if (direction == _upLeft && !Reverse || (direction == _downRight && Reverse)) {
            tStart = -quarterPi;
            tEnd = quarterPi * 3f;
            xOffset = (direction == _upLeft) ? -diagonalOffset : diagonalOffset;
            yOffset = (direction == _upLeft) ? diagonalOffset : -diagonalOffset;
        } else if (direction == _downRight && !Reverse || (direction == _upLeft && Reverse)) {
            tStart = quarterPi * 3f;
            tEnd = quarterPi * 7f;
            xOffset = (direction == _upLeft) ? -diagonalOffset : diagonalOffset;
            yOffset = (direction == _upLeft) ? diagonalOffset : -diagonalOffset;
        }

        if (!Reverse)
        {
            for (float t = tStart; t <= tEnd; t += Time.deltaTime * _moveSpeed) {
                transform.localPosition = new Vector2(startPos.x + 0.5f * ((float)Math.Cos(t) + xOffset),
                    startPos.y + 0.5f * ((float) Math.Sin(t) + yOffset));
                yield return null;
            }
        } else {
            for (float t = tEnd; t >= tStart; t -= Time.deltaTime * _moveSpeed) {
                transform.localPosition = new Vector2(startPos.x + 0.5f * ((float)Math.Cos(t) + xOffset),
                    startPos.y + 0.5f * ((float) Math.Sin(t) + yOffset));
                yield return null;
            }
        }

        transform.localPosition = _moveTo;
        _renderer.sortingOrder = 0;
        _isMoving = false;
    }

    private IEnumerator MoveUp() {
        yield return null;
    }
}
