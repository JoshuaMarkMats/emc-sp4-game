using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumber : AutoDestroyPoolableObject, UILookAtCam
{
    private Transform cam;
    [SerializeField]
    private TextMeshPro damageText;
    [SerializeField]
    private Vector3 Offset = new(0f, 1f, 0f);
    [SerializeField]
    private Vector3 randomizePositionAmount = new(1f, 0f, 1f);
   
    private enum DamageNumberType
    {
        PLAYER,
        ENEMY
    }

    [SerializeField]
    private DamageNumberType damageNumberType;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        transform.position = startPosition + new Vector3(Random.Range(-randomizePositionAmount.x, randomizePositionAmount.x), Random.Range(-randomizePositionAmount.y, randomizePositionAmount.y), Random.Range(-randomizePositionAmount.z, randomizePositionAmount.z)) + Offset;
    }

    public void SetValue (int value)
    {
        switch (damageNumberType)
        {
            case DamageNumberType.PLAYER:
                damageText.text = "-" + value.ToString();
                break;
            case DamageNumberType.ENEMY:
                damageText.text = value.ToString();
                break;
        }    
        
    }
}
