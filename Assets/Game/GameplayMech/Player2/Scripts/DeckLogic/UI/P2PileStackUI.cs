using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class P2PileStackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform container;
    [SerializeField] private Image cardTemplate;

    [Header("Visuals")]
    [SerializeField] private Sprite sprite;
    [Min(0)]
    [SerializeField] private int maxVisible = 8;
    [SerializeField] private Vector2 offsetPerCard = new Vector2(10f, 0f);

    private readonly List<Image> _images = new List<Image>();

    private void Reset()
    {
        container = transform as RectTransform;
    }

    private void Awake()
    {
        if (container == null)
            container = transform as RectTransform;

        if (cardTemplate != null)
            cardTemplate.gameObject.SetActive(false);
    }

    public void SetSprite(Sprite s)
    {
        sprite = s;
        ApplySprite();
    }

    public void SetCount(int count)
    {
        count = Mathf.Max(0, count);

        int visible = maxVisible > 0 ? Mathf.Min(count, maxVisible) : count;
        EnsureImageCount(visible);

        for (int i = 0; i < _images.Count; i++)
        {
            Image img = _images[i];
            bool active = i < visible;

            if (img != null)
                img.gameObject.SetActive(active);

            if (!active || img == null)
                continue;

            img.sprite = sprite;
            img.enabled = sprite != null;

            RectTransform rt = img.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = offsetPerCard * i;

            img.raycastTarget = false;
        }
    }

    private void ApplySprite()
    {
        for (int i = 0; i < _images.Count; i++)
        {
            Image img = _images[i];
            if (img == null)
                continue;

            img.sprite = sprite;
            img.enabled = sprite != null;
        }
    }

    private void EnsureImageCount(int count)
    {
        if (container == null)
            container = transform as RectTransform;

        while (_images.Count < count)
        {
            Image img = CreateImageInstance();
            _images.Add(img);
        }
    }

    private Image CreateImageInstance()
    {
        if (cardTemplate != null)
        {
            Image clone = Instantiate(cardTemplate, container);
            clone.name = "StackCard";
            clone.gameObject.SetActive(true);
            clone.raycastTarget = false;
            return clone;
        }

        GameObject go = new GameObject("StackCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(container, false);

        Image img = go.GetComponent<Image>();
        img.raycastTarget = false;
        return img;
    }
}