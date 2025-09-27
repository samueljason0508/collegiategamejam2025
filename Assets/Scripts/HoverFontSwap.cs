using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UniversalHoverFontSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text and Fonts")]
    public TMP_Text buttonText;       // Drag the TextMeshPro child here
    public TMP_FontAsset normalFont;  // Default font
    public TMP_FontAsset hoverFont;   // Font when hovered

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        // If no text assigned, try to find TextMeshPro child automatically
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null && hoverFont != null)
        {
            buttonText.font = hoverFont;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null && normalFont != null)
        {
            buttonText.font = normalFont;
        }
    }
}
