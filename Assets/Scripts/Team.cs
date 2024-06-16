using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour {

    public GameObject[] Units = new GameObject[_teamSize];

    private static int _teamSize = 6;

    private void Start() {
        PlaceUnits();
    }

    private void PlaceUnits() {
        for (int i = 0; i < _teamSize; i++) {
            Vector3 position = new Vector3((float)i + 0.1f * i, 0, 0);
            GameObject unit = Instantiate(Units[i], transform);
            unit.transform.localPosition = position;
            unit.transform.localRotation = Quaternion.identity;
        }
    }

}