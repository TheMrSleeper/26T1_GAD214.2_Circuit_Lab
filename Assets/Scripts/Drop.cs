using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IDropHandler
{
    [SerializeField] private string _requestedItem;
    public string RequestedItem => _requestedItem;

    [SerializeField] private Image _displayImage;

    private bool _isFilled = false;
    public bool IsFilled => _isFilled;

    public void OnDrop(PointerEventData eventData)
    {
        Drag draggedItem = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<Drag>()
            : null;

        if (draggedItem == null)
            return;

        bool placed = PlaceItem(draggedItem);
        draggedItem.WasDroppedCorrectly = placed;
    }

    public bool PlaceItem(Drag draggedItem)
    {
        if (_isFilled)
            return false;

        if (draggedItem == null || draggedItem.ItemName != _requestedItem)
            return false;

        if (_displayImage != null)
        {
            Image draggedImage = draggedItem.GetComponent<Image>();
            _displayImage.sprite = draggedItem.PlacedSprite != null ? draggedItem.PlacedSprite : draggedImage.sprite;
            _displayImage.enabled = true;
            _displayImage.preserveAspect = true;
            _displayImage.color = Color.white;
        }

        _isFilled = true;
        return true;
    }
}