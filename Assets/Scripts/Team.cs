using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

    public GameObject[] Units = new GameObject[_teamSize];

    [SerializeField] private UIManager _uiManager;

    private static int _teamSize = 6;

    private void Start() {
        PlaceUnits();
    }

    private void PlaceUnits() {
        for (int i = 0; i < _teamSize; i++) {
            if (Units[i] == null)   continue;

            Vector3 position = new Vector3((float)i + 0.1f * i, 0, 0);
            GameObject unit = Instantiate(Units[i], transform);
            unit.transform.localPosition = position;
            unit.transform.localRotation = Quaternion.identity;
        }
    }

    public void CalculateUnitDamage(OrbType t, int numConnected) {
        for (int i = 0; i < _teamSize; i++) {
            if (Units[i] == null)   continue;
            Debug.Log("unit " + i);
            Units[i].GetComponent<Unit>().CalculateDamage(t, numConnected);
        }
    }

    public void ResetUnitDamageUI() {
        for (int i = 0; i < _teamSize; i++) {
            if (Units[i] == null)   continue;
            Units[i].GetComponent<Unit>().ResetDamage();
        }
    }
}