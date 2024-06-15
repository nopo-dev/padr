using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TextField {
    Combo, Red, Green, Blue, Yellow, Purple, Gray
}

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject[] _textFields;

    private string[] _textTemplates = { " combo", " dmg", " block"};

    public void UpdateTextField(TextField field, int value) {
        TextMeshProUGUI tmp = _textFields[(int)field].GetComponent<TextMeshProUGUI>();
        switch(field) {
            case TextField.Combo:
                tmp.text = value +_textTemplates[0];
                break;
            case TextField.Gray:
                tmp.text = Int32.Parse(tmp.text.Substring(0, 1)) + value +_textTemplates[2];
                break;
            default:
                tmp.text = Int32.Parse(tmp.text.Substring(0, 1)) + value +_textTemplates[1];
                break;
        }
    }

    public void ResetTextFields() {
        for (int i = 0; i < _textFields.Length; i++) {
            TextMeshProUGUI tmp = _textFields[i].GetComponent<TextMeshProUGUI>();
            switch(i) {
                case 0:
                    tmp.text = 0 +_textTemplates[0];
                    break;
                case 6:
                    tmp.text = 0 +_textTemplates[2];
                    break;
                default:
                    tmp.text = 0 +_textTemplates[1];
                    break;

            }
        }
    }

    private void Start() {
        ResetTextFields();
    }

}