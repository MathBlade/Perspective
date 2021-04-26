using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Turret : MonoBehaviour
{
    [SerializeField] float DEFAULT_RATE_OF_FIRE_IN_SECONDS = 2;
    [SerializeField] GameObject bullet;

    private void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
        rateOfFire = DEFAULT_RATE_OF_FIRE_IN_SECONDS;
    }
    
    void Start()
    {
        timeOfLastFire = DateTime.Now;

        this.UpdateAsObservable()
            .Where(_ => !IsOnScreen)
            .Subscribe(_ => timeOfLastFire = DateTime.Now);

        this.UpdateAsObservable()
           .Where(_ => IsOnScreen)
           .Where(_ => DateTime.Now - timeOfLastFire > TimeSpan.FromSeconds(rateOfFire))
           .Subscribe(_ => FireBullet());
    }

    void FireBullet()
    {
        timeOfLastFire = DateTime.Now;
        Instantiate(bullet, transform);
    }

    bool IsOnScreen => thisRenderer.isVisible;

    DateTime timeOfLastFire;
    Renderer thisRenderer;
    float rateOfFire;
}
