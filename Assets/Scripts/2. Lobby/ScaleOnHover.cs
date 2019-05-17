using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    Increases the scale of a UI element on hover; decreases when the mouse leaves
*/
/// <summary>
/// Increases the scale of a UI element on hover; decreases when the mouse leaves.
/// </summary>
public class ScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // Public variables
    public Vector3 growthSize;

    // Private variables
    private RectTransform rectTransform;
    private Vector3 initialSize;
    private Button button;

    // Set listeners and references once the game starts.
	void Start () {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.localScale;
        button = GetComponent<Button>();

        // Disable if button not interactable
        if (!button.interactable)
            this.enabled = false;
    }

    /// <summary>
    /// Increases the scale of the UI element attached to this script.
    /// </summary>
    /// <remarks>
    /// Called when the user's mouse enters the UI element attached to this script.
    /// </remarks>
    public void OnPointerEnter(PointerEventData eventData) {
        rectTransform.localScale = initialSize + growthSize;
    }

    /// <summary>
    /// Decreases the scale of the UI element attached to this script.
    /// </summary>
    /// <remarks>
    /// Called when the user's mouse leaves the UI element attached to this script.
    /// </remarks>
    public void OnPointerExit(PointerEventData eventData) {
        ResetScale();
    }

    /// <summary>
    /// Resets the UI element attached to this script back to its default scale.
    /// </summary>
    public void ResetScale() {
        rectTransform.localScale = initialSize;
    }
}
