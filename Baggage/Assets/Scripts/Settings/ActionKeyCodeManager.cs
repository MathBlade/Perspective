using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ActionKeyCodeManager : MonoBehaviour
{
    [SerializeField] GameObject actionKeyComboPrefab;

    private void Start()
    {
        MessageBroker.Default.Receive<ActionKeyCodeCombo>().Subscribe(combo => DisableRebindingForAllBut(combo));
    }

    void OnEnable()
    {
        if (!keyCodeCombos.Any()) SpawnKeyCodeCombos();
    }

    void DisableRebindingForAllBut(ActionKeyCodeCombo combo)
    {
        keyCodeCombos.Where(aCombo => aCombo != combo).ToList().ForEach(aCombo => aCombo.DisableRebinding());
    }

    void SpawnKeyCodeCombos() => CurrentInputs.RemappableKeyMaps.ForEach(keymap => SpawnKeyCodeCombo(keymap));

    void SpawnKeyCodeCombo(KeyMap map)
    {
        var actionComboObject = GameObject.Instantiate(actionKeyComboPrefab, transform);
        actionComboObject.name = map.keyAction.ToString();
        var combo = actionComboObject.GetComponent<ActionKeyCodeCombo>();
        combo.Initialize(map);
        keyCodeCombos.Add(combo);
    }


    List<ActionKeyCodeCombo> keyCodeCombos = new List<ActionKeyCodeCombo>();
}
