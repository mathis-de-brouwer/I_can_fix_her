using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ResultIconEntryUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;

    public void Setup(Sprite sprite, int count)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }

        if (countText != null)
            countText.text = count > 1 ? $"x{count}" : string.Empty;
    }
}