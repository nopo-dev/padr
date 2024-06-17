using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitData")]
public class UnitData : ScriptableObject {

    public int attack;
    public int health;
    public int shield;

    public OrbType primaryType;
    public OrbType secondaryType;

}
