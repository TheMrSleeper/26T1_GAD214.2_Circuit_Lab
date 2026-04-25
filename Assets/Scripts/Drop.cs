using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IDropHandler
{
    [SerializeField] private string _requestedItem;
    [SerializeField] private TMP_Text _guideLabel;
    public string RequestedItem => _requestedItem;

    public string CurrentItemName { get; private set; } = "";

    public void SetRequestedItem(string itemName)
    {
        _requestedItem = itemName;
        CurrentItemName = "";

        if (_guideLabel != null)
        {
            _guideLabel.text = FormatItemName(itemName);
        }
    }

    private string FormatItemName(string itemName)
    {
        switch (itemName)
        {
            case "battery": return "Battery";
            case "lamp": return "Lamp";
            case "switch": return "Switch";
            case "resistor": return "Resistor";
            case "fuse": return "Fuse";
            default: return itemName;
        }
    }

    // Optional parent for spawned items. If left empty, this drop zone is used.
    [SerializeField] private Transform _spawnParent;

    private void Awake()
    {
        if (_spawnParent == null)
        {
            _spawnParent = transform;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Drag draggedItem = eventData.pointerDrag.GetComponent<Drag>();

        if (draggedItem == null)
            return;

        // Remember what item is now in this slot.
        CurrentItemName = draggedItem.ItemName;

        // Accept any draggable item, regardless of correctness.
        ReplaceDroppedCopy(eventData.pointerDrag);
    }

    private void ReplaceDroppedCopy(GameObject sourceObject)
    {
        // Remove any item already placed in this slot.
        for (int i = _spawnParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_spawnParent.GetChild(i).gameObject);
        }

        // Spawn the new placed copy.
        GameObject copy = Instantiate(sourceObject, _spawnParent);

        RectTransform copyRect = copy.GetComponent<RectTransform>();
        copyRect.position = transform.position;

        // Disable the cloned hitbox so the placed copy does not block future drops.
        Image image = copy.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = false;
        }

        // Restore only the visible art to full opacity, then remove dragging.
        Drag dragComponent = copy.GetComponent<Drag>();
        if (dragComponent != null)
        {
            dragComponent.ForceFullVisualAlpha();
            Destroy(dragComponent);
        }
    }

    public bool IsCorrect()
    {
        return !string.IsNullOrEmpty(CurrentItemName) && CurrentItemName == _requestedItem;
    }

    public bool HasItem()
    {
        return !string.IsNullOrEmpty(CurrentItemName);
    }
}