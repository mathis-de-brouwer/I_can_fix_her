using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class P1ItemOptionUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text multiplierText;
    [SerializeField] private Button button;

    [SerializeField] private float disabledAlpha = 0.45f;

    [Header("Controller Selection Scale")]
    [SerializeField] private float selectedIconScale = 1.15f;

    P1RewardOffer _offer;
    Action<P1RewardOffer> _onClicked;

    Vector3 _iconBaseScale;

    public P1RewardOffer Offer => _offer;

    void Awake()
    {
        if (icon != null)
            _iconBaseScale = icon.rectTransform.localScale;
    }

    public void Setup(P1RewardOffer offer, Action<P1RewardOffer> onClicked)
    {
        _offer = offer;
        _onClicked = onClicked;

        gameObject.SetActive(true);
        SetDimmed(false);
        SetSelected(false);

        ApplyVisuals(offer);

        bool valid = offer != null && offer.Prefab != null;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
            button.interactable = valid;
        }

        if (!valid)
            SetDimmed(true);
    }

    void ApplyVisuals(P1RewardOffer offer)
    {
        Sprite offerIcon = null;
        string offerName = offer?.Prefab != null ? offer.Prefab.name : "Unknown";
        string offerDetails = string.Empty;

        if (offer == null)
        {
            SetVisuals(null, "Unknown", string.Empty);
            return;
        }

        switch (offer.Type)
        {
            case P1RewardOfferType.NewPassive:
            {
                PassiveItems passive = offer.Prefab != null ? offer.Prefab.GetComponent<PassiveItems>() : null;
                PassiveItemsScriptableObjects data = passive != null ? passive.passiveItemsData : null;

                offerIcon = data != null ? data.Icon : null;
                offerName = data != null && !string.IsNullOrEmpty(data.ItemName) ? data.ItemName : offerName;
                offerDetails = data != null ? $"x{data.Multiplier}" : string.Empty;
                break;
            }

            case P1RewardOfferType.NewWeapon:
            {
                WeaponController wc = offer.Prefab != null ? offer.Prefab.GetComponent<WeaponController>() : null;
                WeaponScriptableObject data = wc != null ? wc.weaponData : null;

                offerIcon = data != null ? data.Icon : null;
                offerName = data != null && !string.IsNullOrEmpty(data.WeaponName) ? data.WeaponName : (data != null ? data.name : offerName);
                offerDetails = data != null ? $"Lv {data.Level}" : string.Empty;
                break;
            }

            case P1RewardOfferType.WeaponUpgrade:
            {
                WeaponController wc = offer.Prefab != null ? offer.Prefab.GetComponent<WeaponController>() : null;
                WeaponScriptableObject data = wc != null ? wc.weaponData : null;

                offerIcon = data != null ? data.Icon : null;

                string baseName = data != null && !string.IsNullOrEmpty(data.WeaponName) ? data.WeaponName : (data != null ? data.name : "Weapon");
                offerName = $"Upgrade: {baseName}";

                if (data != null && data.NextLevelPrefab != null)
                {
                    WeaponController nextWc = data.NextLevelPrefab.GetComponent<WeaponController>();
                    WeaponScriptableObject nextData = nextWc != null ? nextWc.weaponData : null;
                    offerDetails = nextData != null ? $"-> Lv {nextData.Level}" : "-> Next";
                }
                else
                {
                    offerDetails = "Max";
                }

                break;
            }

            case P1RewardOfferType.PassiveUpgrade:
                {
                    PassiveItems passive = offer.Prefab != null ? offer.Prefab.GetComponent<PassiveItems>() : null;
                    PassiveItemsScriptableObjects data = passive != null ? passive.passiveItemsData : null;

                    offerIcon = data != null ? data.Icon : null;

                    string baseName = data != null && !string.IsNullOrEmpty(data.ItemName) ? data.ItemName : offerName;
                    offerName = $"Upgrade: {baseName}";
                    offerDetails = data != null && data.NextLevelPrefab != null ? "-> Next" : "Max";
                    break;
                }
        }

        SetVisuals(offerIcon, offerName, offerDetails);
    }

    void SetVisuals(Sprite sprite, string title, string details)
    {
        if (icon != null)
            icon.sprite = sprite;

        if (nameText != null)
            nameText.text = title ?? string.Empty;

        if (multiplierText != null)
            multiplierText.text = details ?? string.Empty;
    }

    public void SetMouseInputEnabled(bool enabled)
    {
        if (button != null)
            button.interactable = enabled;
    }

    public void SetDimmed(bool dimmed)
    {
        if (icon != null)
        {
            Color c = icon.color;
            c.a = dimmed ? disabledAlpha : 1f;
            icon.color = c;
        }
    }

    public void SetSelected(bool selected)
    {
        if (icon == null) return;

        Vector3 scale = _iconBaseScale;
        if (selected)
            scale *= selectedIconScale;

        icon.rectTransform.localScale = scale;
    }

    public void SimulateClick()
    {
        OnClick();
    }

    void OnClick()
    {
        _onClicked?.Invoke(_offer);
    }

    public bool TryBuildTooltip(out Sprite iconSprite, out string title, out string body)
    {
        iconSprite = null;
        title = string.Empty;
        body = string.Empty;

        if (_offer == null || _offer.Prefab == null)
            return false;

        switch (_offer.Type)
        {
            case P1RewardOfferType.NewPassive:
            case P1RewardOfferType.PassiveUpgrade:
            {
                PassiveItems passive = _offer.Prefab.GetComponent<PassiveItems>();
                PassiveItemsScriptableObjects data = passive != null ? passive.passiveItemsData : null;
                if (data == null)
                    return false;

                iconSprite = data.Icon;
                title = !string.IsNullOrEmpty(data.ItemName) ? data.ItemName : data.name;
                body = data.Description;
                return true;
            }

            case P1RewardOfferType.NewWeapon:
            case P1RewardOfferType.WeaponUpgrade:
            {
                WeaponController wc = _offer.Prefab.GetComponent<WeaponController>();
                WeaponScriptableObject data = wc != null ? wc.weaponData : null;
                if (data == null)
                    return false;

                iconSprite = data.Icon;
                title = !string.IsNullOrEmpty(data.WeaponName) ? data.WeaponName : data.name;
                body = data.Description;
                return true;
            }

            default:
                return false;
        }
    }
}