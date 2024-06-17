using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    [SerializeField] private Team _team;

    public void CalculateMatchDamage(OrbType t, int numConnected) {
        _team.CalculateUnitDamage(t, numConnected);
    }

    public void ResetUnitDamageUI() {
        _team.ResetUnitDamageUI();
    }
}