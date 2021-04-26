using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DeadlyObstacle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //MessageBroker.Default.Publish(new HitDeadlyObstacleMessage());
        var iHurt = other.gameObject.GetComponent<IHurtOnColllision>();
        if (iHurt != null && iHurt.IsAlive) iHurt.HitDeadlyObstacle();
    }
}

//public class HitDeadlyObstacleMessage { }
