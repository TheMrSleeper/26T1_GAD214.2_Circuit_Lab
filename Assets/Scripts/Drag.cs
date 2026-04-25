using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    [Header("Images")]
    [SerializeField] private Image _raycastImage; // Transparent hitbox image
    [SerializeField] private Image _visualImage;  // Actual visible component image

    [Header("Drag Feedback")]
    [SerializeField] private float _dragAlpha = 0.5f;

    private Vector3 _startingPos;
    private float _originalVisualAlpha = 1f;

    private void Awake()
    {
        if (_raycastImage == null)
            _raycastImage = GetComponent<Image>();

        if (_visualImage != null)
            _originalVisualAlpha = _visualImage.color.a;
    }

    private void Start()
    {
        _startingPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_raycastImage != null)
            _raycastImage.raycastTarget = false;

        SetVisualAlpha(_dragAlpha);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_raycastImage != null)
            _raycastImage.raycastTarget = true;

        SetVisualAlpha(_originalVisualAlpha);

        transform.localPosition = _startingPos;
    }

    public void ForceFullVisualAlpha()
    {
        SetVisualAlpha(1f);
    }

    private void SetVisualAlpha(float alpha)
    {
        if (_visualImage == null)
            return;

        Color c = _visualImage.color;
        c.a = alpha;
        _visualImage.color = c;
    }
}