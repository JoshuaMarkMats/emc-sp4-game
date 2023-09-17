using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : AutoDestroyPoolableObject
{
    [HideInInspector]
    public Rigidbody2D rigidBody;
    public Vector2 speed = new Vector2(200, 0);

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        rigidBody.velocity= speed;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        rigidBody.velocity = Vector2.zero;
    }
}
