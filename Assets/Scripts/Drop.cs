using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IDropHandler
{
    [SerializeField] private string _requestedItem;
    public string RequestedItem => _requestedItem;

    public string CurrentItemName { get; private set; } = "";

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

        // Force the placed copy to be fully visible.
        Image image = copy.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;
            image.raycastTarget = false;
        }

        // Make the placed copy non-draggable.
        Drag dragComponent = copy.GetComponent<Drag>();
        if (dragComponent != null)
        {
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