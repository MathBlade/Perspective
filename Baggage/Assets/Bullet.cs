using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] Vector2 defaultBulletVelocity;
    [SerializeField] LayerMask takesDamageOnHit;
    [SerializeField] int damageOnHit = 1;
    private void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        bulletVelocity = defaultBulletVelocity;

        this.UpdateAsObservable()
            .Where(_ => !IsOnScreen)
            .Subscribe(_ => Destroy(gameObject)).AddTo(this);
    }

    void Start() => rigidBody.velocity = defaultBulletVelocity;
    void OnTriggerEnter2D(Collider2D collision)
    {
        //layermask == (layermask | (1 << layer))
        var layer = collision.gameObject.layer;
        if (takesDamageOnHit != (takesDamageOnHit | (1 << layer)))
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var layer = collision.gameObject.layer;
        if (takesDamageOnHit != (takesDamageOnHit | (1 << layer))) return;
        Destroy(gameObject);
        var iHurt = collision.gameObject.GetComponent<IHurtOnColllision>();
        if (iHurt != null && iHurt.IsAlive) iHurt.TookDamage(damageOnHit);
    }

    bool IsOnScreen => thisRenderer.isVisible;

    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;
    Vector2 bulletVelocity;
    Renderer thisRenderer;
}
