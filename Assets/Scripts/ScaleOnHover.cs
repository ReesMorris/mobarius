using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Vector3 growthSize;

    private RectTransform rectTransform;
    private Vector3 initialSize;

	// Use this for initialization
	void Start () {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        rectTransform.localScale = initialSize + growthSize;
    }

    public void OnPointerExit(PointerEventData eventData) {
        rectTransform.localScale = initialSize;
    }
}
