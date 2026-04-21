using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    [SerializeField] private Sprite _placedSprite;
    public Sprite PlacedSprite => _placedSprite;

    public bool WasDroppedCorrectly { get; set; }

    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private LayoutElement _layoutElement;

    private Transform _originalParent;
    private int _originalSiblingIndex;
    private GameObject _placeholder;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _layoutElement = GetComponent<LayoutElement>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (_layoutElement == null)
            _layoutElement = gameObject.AddComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        WasDroppedCorrectly = false;

        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();

        CreatePlaceholder();

        transform.SetParent(_canvas.transform, true);
        transform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.35f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (WasDroppedCorrectly)
        {
            if (_placeholder != null)
                Destroy(_placeholder);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_originalParent as RectTransform);
            gameObject.SetActive(false);
            return;
        }

        transform.SetParent(_originalParent, false);
        transform.SetSiblingIndex(_originalSiblingIndex);
        _rectTransform.localScale = Vector3.one;
        _canvasGroup.alpha = 1f;

        if (_placeholder != null)
            Destroy(_placeholder);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_originalParent as RectTransform);
    }

    private void CreatePlaceholder()
    {
        _placeholder = new GameObject("Placeholder", typeof(RectTransform), typeof(LayoutElement));
        _placeholder.transform.SetParent(_originalParent, false);
        _placeholder.transform.SetSiblingIndex(_originalSiblingIndex);

        RectTransform placeholderRect = _placeholder.GetComponent<RectTransform>();
        placeholderRect.localScale = Vector3.one;

        LayoutElement placeholderLayout = _placeholder.GetComponent<LayoutElement>();
        placeholderLayout.preferredWidth = _layoutElement.preferredWidth > 0 ? _layoutElement.preferredWidth : _rectTransform.rect.width;
        placeholderLayout.preferredHeight = _layoutElement.preferredHeight > 0 ? _layoutElement.preferredHeight : _rectTransform.rect.height;
        placeholderLayout.flexibleWidth = 0;
        placeholderLayout.flexibleHeight = 0;
    }
}