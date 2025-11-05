using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Chest : MonoBehaviour, IInteractuable
{
    [Header("Inventory")]
    [SerializeField] GameObject[] _chestInventory;

    [Header("DropValues")]
    [SerializeField] float _maxDropRange;
    [SerializeField] float _minDropRange;
    [SerializeField] float _dropSpeed;
    [SerializeField] float _dropWaitTime = 1f;

    [Header("View Values")]
    [SerializeField] Animator _cuteAnimator;
    [SerializeField] string _animIsOpenBoolName = "IsOpen";

    static bool _canOpen = true;

    private void Start()
    {
        AnimatorSetValues();
    }

    public void Interact()
    {
        if (!RealityChangeManager.Instance.CuteWorld || _chestInventory.Length <= 0 || !_canOpen) return;
        StartCoroutine(DropItemsRoutine(_chestInventory));
    }

    IEnumerator DropItemsRoutine(GameObject[] _items)
    {
        _canOpen = false;
        AnimatorSetValues();

        foreach (var item in _items)
        {
            yield return new WaitForSeconds(_dropWaitTime);
            var itemInst = Instantiate(item, transform.position, Quaternion.identity, transform);

            var x = GetRandomOustideRange(-_maxDropRange, -_minDropRange, _minDropRange, _maxDropRange);
            var y = GetRandomOustideRange(-_maxDropRange, -_minDropRange, _minDropRange, _maxDropRange);

            Vector3 finalPos = new Vector3(itemInst.transform.position.x + x, itemInst.transform.position.y + y);

            while (Vector2.Distance(itemInst.transform.position, finalPos) > .01f)
            {
                var dir = (finalPos - itemInst.transform.position).normalized;
                itemInst.transform.position += dir * _dropSpeed * Time.deltaTime;
                yield return null;
            }

            itemInst.transform.position = finalPos;
        }
    }

    float GetRandomOustideRange(float min1, float max1, float min2, float max2)
    {
        if (Random.value < .5f)
            return Random.Range(min1, max1); //Lado izquierdo
        else
            return Random.Range(min2, max2); //Lado derecho

    }

    void AnimatorSetValues()
    {
        _cuteAnimator.SetBool(_animIsOpenBoolName, !_canOpen);
    }
}
