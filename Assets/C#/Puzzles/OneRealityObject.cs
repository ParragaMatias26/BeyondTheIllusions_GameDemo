using UnityEngine;

public class OneRealityObject : MonoBehaviour
{
    [SerializeField] Realitys _myReality;
    [SerializeField] protected GameObject _objectToModify;

    private void Start()
    {
        SetObjectReality();
    }

    protected void SetObjectReality()
    {
        switch (_myReality)
        {
            case Realitys.Cute:
                RealityChangeManager.OnCuteWorldEnabled += EnableObject;
                RealityChangeManager.OnCuteWorldDisabled += DisableObject;

                _objectToModify.SetActive(false);
                break;
            case Realitys.Dark:
                RealityChangeManager.OnCuteWorldDisabled += EnableObject;
                RealityChangeManager.OnCuteWorldEnabled += DisableObject;
                break;
        }
    }
    protected void EnableObject()
    {
        if (_objectToModify == null) return;
        _objectToModify.SetActive(true);
    }

    protected void DisableObject()
    {
        if (_objectToModify == null) return;
        _objectToModify.SetActive(false);
    }
}

public enum Realitys
{
    Cute,
    Dark
}
