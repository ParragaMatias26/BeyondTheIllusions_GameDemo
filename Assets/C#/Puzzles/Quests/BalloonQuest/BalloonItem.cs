using System.Collections;
using UnityEngine;

public class BalloonItem : MonoBehaviour
{
    [SerializeField] BalloonQuest clownEntity;
    [SerializeField] Transform endPos;

    public float flySpeed = 3f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    private bool canIddle = false;

    private Vector3 startPosition;
    PickeableObject myPickeableObject;

    private void Start()
    {
        canIddle = false;
        myPickeableObject = gameObject.GetComponent<PickeableObject>();

        transform.position = clownEntity.transform.position;

        myPickeableObject.OnDrop += () =>
        {
            myPickeableObject.canPickup = false;
            canIddle = false;

            GoToEndPos();
        };
    }
    private void Update()
    {
        if(!myPickeableObject.isPickedUp && canIddle) BalloonMovement();
    }
    public void GoToEndPos() 
    {
        StartCoroutine(GoToPos_Execute());    
    }
    IEnumerator GoToPos_Execute() 
    {
        while(Vector3.Distance(transform.position, endPos.position) > .05f) 
        {
            var dir = (endPos.position - transform.position).normalized;
            transform.position += dir * (flySpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = endPos.position;

        canIddle = true;
        myPickeableObject.canPickup = true;
    }
    void BalloonMovement()
    {
        float newY = endPos.position.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(endPos.position.x, newY, 0f);
    }
}
