using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Heart : MonoBehaviour
{
    public const int MAX_HEART_PIECES = 4;

    private void Awake() => image = GetComponent<Image>();
   
    void Start()
    {
        filledHeartPieces.Subscribe(p => UpdateImage(p));
        filledHeartPieces.Value = MAX_HEART_PIECES;
    }

    void UpdateImage(int pieces)
    {
        var newFillAmount = (float)(pieces / (float)MAX_HEART_PIECES);
        image.fillAmount = newFillAmount;
    }

    public int Replenish(int heartPieces)
    {
        if (heartPieces < 0) throw new System.ArgumentOutOfRangeException("Can't add negative amount of pieces");
        if (IsFull) return heartPieces;
        while(heartPieces > 0 && !IsFull)
        {
            filledHeartPieces.Value++;
            heartPieces--;
        }

        return heartPieces;
    }

    public bool IsFull => filledHeartPieces.Value == MAX_HEART_PIECES;
    public bool IsEmpty => filledHeartPieces.Value == 0;

    public int Deplete(int heartPieces)
    {
        if (heartPieces < 0) throw new System.ArgumentOutOfRangeException("Can't subtract negative amount of pieces");
        if (IsEmpty) return heartPieces;
        while (heartPieces > 0 && !IsEmpty)
        {
            filledHeartPieces.Value--;
            heartPieces--;
        }

        return heartPieces;
    }

    ReactiveProperty<int> filledHeartPieces = new ReactiveProperty<int>();
    Image image;
}
