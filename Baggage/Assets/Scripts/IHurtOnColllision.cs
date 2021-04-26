using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtOnColllision 
{
    void HitDeadlyObstacle();
    void TookDamage(int amount);

    bool IsAlive { get; }
}
