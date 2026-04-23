using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    [SerializeField] private float _dragAlpha = 0.5f;

    private Image _image;
    private Vector3 _startingPos;

    private void Start()
    {
        _image = GetComponent<Image>();
        _startingPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;

        SetAlpha(_dragAlpha);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;

        // Restore full opacity
        SetAlpha(1f);

        // Return to original position
        _image.rectTransform.localPosition = _startingPos;
    }

    private void SetAlpha(float alpha)
    {
        Color c = _image.color;
        c.a = alpha;
        _image.color = c;
    }
}