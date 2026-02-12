using TMPro;
using UnityEngine;

public sealed class P2ChargesUI : MonoBehaviour
{
    [SerializeField] private P2Charges charges;
    [SerializeField] private TMP_Text chargesText;
    [SerializeField] private string format = "{0:0.0} / {1:0}";

    private void Reset()
    {
        chargesText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (charges == null || chargesText == null)
            return;

        chargesText.text = string.Format(format, charges.currentCharges, charges.maxCharges);
    }
}
