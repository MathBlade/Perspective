using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static HeartMessage;

public class HeartContainer : MonoBehaviour
{
    public const int DEFAULT_MAX_HEARTS = 4;

    [SerializeField] GameObject heartPrefab;
    
    void Start()
    {
        maxHearts = DEFAULT_MAX_HEARTS;
        CreateHearts();

        MessageBroker.Default.Receive<HealHeartsMessage>()
            .Subscribe(m => AddHeartPieces(m.AmountToHeal))
            .AddTo(this);

        MessageBroker.Default.Receive<DamageHeartsMessage>()
            .Subscribe(m => DepleteHeartPieces(m.DamageAmount))
            .AddTo(this);

        MessageBroker.Default.Receive<ResetHeartsMessage>()
            .Subscribe(_ => ResetHearts())
            .AddTo(this);
    }

    void ResetHearts()
    {
        DestroyAllHearts();
        CreateHearts();
    }

    void CreateHearts()
    {
        for(int i = hearts.Count; i < maxHearts; i++)
        {
            hearts.Add(GameObject.Instantiate(heartPrefab, transform).GetComponent<Heart>());
        }
    }

    void DestroyAllHearts()
    {
        hearts.ForEach(h => Destroy(h.gameObject));
        hearts.Clear();
    }

    void AddHeartPieces(int pieces)
    {
        for (int i= 0; i < hearts.Count; i++)
        {
            var heart = hearts[i];
            if (heart.IsFull) continue;
            int leftOverPieces = heart.Replenish(pieces);
            if (leftOverPieces == 0) break;
            else pieces = leftOverPieces;
        }
    }

    void DepleteHeartPieces(int pieces)
    {
        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            var heart = hearts[i];
            if (heart.IsEmpty) continue;
            int leftOverPieces = heart.Deplete(pieces);
            if (leftOverPieces == 0) break;
            else pieces = leftOverPieces;
        }

        if (hearts.All(h => h.IsEmpty)) MessageBroker.Default.Publish(new OutOfHeartsMessage());
    }

    List<Heart> hearts = new List<Heart>();
    int maxHearts = 4;
}
