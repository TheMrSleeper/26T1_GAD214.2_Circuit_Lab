using System.Collections.Generic;
using UnityEngine;

public class LevelSlotRandomizer : MonoBehaviour
{
    [Header("Drop Slots")]
    [SerializeField] private Drop[] _dropSlots;

    [Header("Required Components")]
    [SerializeField]
    private string[] _requiredComponents =
    {
        "battery",
        "lamp",
        "switch"
    };

    [Header("Optional Components")]
    [SerializeField]
    private string[] _optionalComponents =
    {
        "battery",
        "lamp",
        "resistor",
        "fuse"
    };

    [Header("Must Include At Least One Of")]
    [SerializeField] private bool _useAtLeastOneOfRule = false;

    [SerializeField]
    private string[] _atLeastOneOfComponents =
    {
        "lamp",
        "resistor"
    };

    [Header("Rules")]
    [SerializeField] private int _maxSwitches = 1;

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
        if (_dropSlots == null || _dropSlots.Length == 0)
        {
            Debug.LogWarning($"{name}: No drop slots assigned.");
            return;
        }

        if (_requiredComponents.Length > _dropSlots.Length)
        {
            Debug.LogWarning($"{name}: More required components than drop slots.");
            return;
        }

        List<string> assignedItems = new List<string>(_requiredComponents);

        while (assignedItems.Count < _dropSlots.Length)
        {
            string candidate = _optionalComponents[Random.Range(0, _optionalComponents.Length)];

            if (candidate == "switch" && CountItem(assignedItems, "switch") >= _maxSwitches)
                continue;

            assignedItems.Add(candidate);
        }

        EnforceAtLeastOneOfRule(assignedItems);

        Shuffle(assignedItems);

        for (int i = 0; i < _dropSlots.Length; i++)
        {
            _dropSlots[i].SetRequestedItem(assignedItems[i]);
        }
    }

    private void EnforceAtLeastOneOfRule(List<string> assignedItems)
    {
        if (!_useAtLeastOneOfRule || _atLeastOneOfComponents == null || _atLeastOneOfComponents.Length == 0)
            return;

        if (ContainsAny(assignedItems, _atLeastOneOfComponents))
            return;

        string requiredChoice = _atLeastOneOfComponents[Random.Range(0, _atLeastOneOfComponents.Length)];

        for (int i = assignedItems.Count - 1; i >= 0; i--)
        {
            if (!IsRequiredComponent(assignedItems[i]))
            {
                assignedItems[i] = requiredChoice;
                return;
            }
        }

        Debug.LogWarning($"{name}: Could not enforce at-least-one-of rule because all slots are required components.");
    }

    private bool ContainsAny(List<string> items, string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            if (items.Contains(options[i]))
                return true;
        }

        return false;
    }

    private bool IsRequiredComponent(string itemName)
    {
        for (int i = 0; i < _requiredComponents.Length; i++)
        {
            if (_requiredComponents[i] == itemName)
                return true;
        }

        return false;
    }

    private int CountItem(List<string> items, string itemName)
    {
        int count = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemName)
                count++;
        }

        return count;
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