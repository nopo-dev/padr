using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Unit : MonoBehaviour {
    
    public static Dictionary<OrbType, Color> OrbColors = new Dictionary<OrbType, Color> {
        { OrbType.Red, new Color(0.8867924f, 0.2216981f, 0.2595712f, 1f) },
        { OrbType.Green, new Color(0.2659309f, 0.6792453f, 0.2659309f, 1f) },
        { OrbType.Blue, new Color(0.3349057f, 0.3349057f, 1f, 1f) },
        { OrbType.Yellow, new Color(0.8679245f, 0.7847762f, 0.2824849f, 1f) },
        { OrbType.Purple, new Color(0.6415094f, 0.2087932f, 0.6415094f, 1f) },
        { OrbType.Gray, new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f) }
    };

    [SerializeField] private UnitData _unitData;

    private Image _primaryImage;
    private Image _secondaryImage;

    private TextMeshProUGUI _primaryDamage;
    private TextMeshProUGUI _secondaryDamage;

    private void Start() {
        _primaryImage = transform.GetChild(0).GetComponent<Image>();
        _secondaryImage = transform.GetChild(1).GetComponent<Image>();
        _primaryDamage = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _secondaryDamage = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        SetUI();
        ResetDamage();
    }

    private void SetUI() {
        if (_unitData.primaryType != OrbType.None)  {
            _primaryImage.color = OrbColors[_unitData.primaryType];
            _primaryDamage.color = _primaryImage.color;
        }
        else
            _primaryImage.enabled = false;

        if (_unitData.secondaryType != OrbType.None) {
            _secondaryImage.color = OrbColors[_unitData.secondaryType];
            _secondaryDamage.color = _secondaryImage.color;
        }
        else
            _secondaryImage.enabled = false;
    }

    public int[] TotalDamage {
        get { return _totalDamage; }
    }
    [SerializeField] private int[] _totalDamage = new int[2];

    public void ResetDamage() {
        _totalDamage = new int[2];
        _primaryDamage.text = "";
        _secondaryDamage.text = "";
        _primaryDamage.enabled = false;
        _secondaryDamage.enabled = false;
    }

    public void CalculateDamage(OrbType t, int numConnected) {
        if (t == _unitData.primaryType) {
            AddDamage(0, numConnected * _unitData.attack);
        }
        if (t == _unitData.secondaryType) {
            AddDamage(1, numConnected * _unitData.attack / 2);
        }
    }

    private void AddDamage(int slot, int amount) {
        _totalDamage[slot] += amount;

        if (slot == 0) {
            _primaryDamage.text = _totalDamage[slot] + "";
            if (_primaryDamage.enabled == false)
                _primaryDamage.enabled = true;
        } else if (slot == 1) {
            _secondaryDamage.text = _totalDamage[slot] + "";
            if (_secondaryDamage.enabled == false)
                _secondaryDamage.enabled = true;
        }
    }
}