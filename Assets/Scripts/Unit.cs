using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private OrbType _type1;
    [SerializeField] private OrbType _type2;

    public OrbType Type1 {
        get { return _type1; }
    }

    public OrbType Type2 {
        get {return _type2; }
    }

    [SerializeField] private int _attack;
    [SerializeField] private int _hp;
    [SerializeField] private int _shield;
}