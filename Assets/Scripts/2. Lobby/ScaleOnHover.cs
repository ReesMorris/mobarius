using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Vector3 growthSize;

    private RectTransform rectTransform;
    private Vector3 initialSize;
    private Button button;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.localScale;
        button = GetComponent<Button>();

        // Disable if button not interactable
        if (!button.interactable)
            this.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        rectTransform.localScale = initialSize + growthSize;
    }

    public void OnPointerExit(PointerEventData eventData) {
        ResetScale();
    }

    public void ResetScale() {
        rectTransform.localScale = initialSize;
    }
}
