using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallucinationManager : MonoBehaviour
{
    [Header("UI Values")]
    [SerializeField] Image _hallucinationBar;

    [Header("HallucinationValues")]
    [SerializeField] float _maxHallucination;
    [SerializeField] float _hallucinationIncreaseSpeedModifier = 2f;
    [SerializeField] float _ammountToChangeReality = 20f;
    [Range(0f, 30f)][SerializeField] float _modifyAmmount = 5f;
    [SerializeField] float _decreaseAmmount = 5f;

    public float _hallucinationAmmount;
    public List<PinkMushroom> _allMushrooms = new List<PinkMushroom>();

    public float AmmountToChangeReality { get { return _ammountToChangeReality; } }
    public float HallucinationAmmount { get { return _hallucinationAmmount; } set { _hallucinationAmmount = value; } }
    public float MaxHallucination { get { return _maxHallucination; } }
    public bool canDecrease = true;

    private void Update()
    {
        if(DetectAnyNearMushroom()) StartIncrease();
        if (_hallucinationAmmount > 0 && !DetectAnyNearMushroom()) StartDecrease();

        UpdateHallucinationBar();
    }

    bool DetectAnyNearMushroom()
    {
        bool isNearAnyFungus = false;

        foreach (PinkMushroom mush in _allMushrooms)
        {
            if (mush.IsTargetOnRadius(transform.position) && mush.HaveAnySpores)
            {
                isNearAnyFungus = true;
                break;
            }
        }

        return isNearAnyFungus;
    }

    public void StartIncrease()
    {
        IncreaseHallucinationInTime(_modifyAmmount);
    }
    public void StartDecrease()
    {
        if (!canDecrease) return;
        DecreaseHallucinationInTime(_decreaseAmmount);
    }

    public void DecraseHallucination(float ammount)
    {

        if (_hallucinationAmmount < 0f)
        {
            _hallucinationAmmount = 0f;
            return;
        }

        _hallucinationAmmount -= ammount;
        if (_hallucinationAmmount <= _ammountToChangeReality) RealityChangeManager.Instance.ToggleCuteWorld(false);
    }

    public void IncreaseHallucinationInTime(float ammount)
    {
        if (_hallucinationAmmount >= _maxHallucination)
        {
            _hallucinationAmmount = _maxHallucination;
            return;
        }

        _hallucinationAmmount += (ammount * _hallucinationIncreaseSpeedModifier) * Time.deltaTime;
        if (_hallucinationAmmount > _ammountToChangeReality) RealityChangeManager.Instance.ToggleCuteWorld(true);
    }

    void DecreaseHallucinationInTime(float ammount)
    {
        if (_hallucinationAmmount < 0f)
        {
            _hallucinationAmmount = 0f;
            return;
        }

        _hallucinationAmmount -= ammount * Time.deltaTime;
        if (_hallucinationAmmount <= _ammountToChangeReality) RealityChangeManager.Instance.ToggleCuteWorld(false);
    }

    void UpdateHallucinationBar()
    {
        if (_hallucinationBar == null) return;

        var x = _hallucinationAmmount / _maxHallucination;
        _hallucinationBar.fillAmount = x;
    }
}
