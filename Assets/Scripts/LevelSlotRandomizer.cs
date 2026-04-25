using System.Collections.Generic;
using UnityEngine;

public class LevelSlotRandomizer : MonoBehaviour
{
    [Header("Drop Slots")]
    [SerializeField] private Drop[] _dropSlots;

    [Header("Required Components")]
    [SerializeField] private string _battery = "battery";
    [SerializeField] private string _lamp = "lamp";
    [SerializeField] private string _switch = "switch";

    [Header("Optional Components")]
    [SerializeField] private string[] _optionalComponents = { "resistor", "fuse" };

    [Header("Randomization")]
    [SerializeField] private bool _randomizeOnEnable = true;

    private void OnEnable()
    {
        if (_randomizeOnEnable)
        {
            RandomizeSlots();
        }
    }

    public void RandomizeSlots()
    {
        if (_dropSlots == null || _dropSlots.Length != 4)
        {
            Debug.LogWarning($"{name}: Expected exactly 4 drop slots.");
            return;
        }

        List<string> assignedItems = new List<string>
        {
            _battery,
            _lamp,
            _switch
        };

        // Fourth slot can be battery, lamp, resistor, or fuse.
        // Switch is intentionally excluded so there is never more than one switch.
        List<string> fourthSlotOptions = new List<string>
        {
            _battery,
            _lamp
        };

        fourthSlotOptions.AddRange(_optionalComponents);

        string fourthItem = fourthSlotOptions[Random.Range(0, fourthSlotOptions.Count)];
        assignedItems.Add(fourthItem);

        Shuffle(assignedItems);

        for (int i = 0; i < _dropSlots.Length; i++)
        {
            _dropSlots[i].SetRequestedItem(assignedItems[i]);
        }
    }

    private void Shuffle(List<string> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int randomIndex = Random.Range(i, items.Count);
            (items[i], items[randomIndex]) = (items[randomIndex], items[i]);
        }
    }
}