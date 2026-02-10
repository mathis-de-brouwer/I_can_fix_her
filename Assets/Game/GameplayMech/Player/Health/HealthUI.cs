using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image healthImage;
    [SerializeField] private Sprite[] healthSprites;


    private void Start()
    {
        UpdateHealthImage();
    }
    
    private void Update()
    {
        UpdateHealthImage();
    }

    private void UpdateHealthImage()
    {
        int index = Mathf.Clamp(health.CurrentHealth -1, 0, healthSprites.Length - 1);
        healthImage.sprite = healthSprites[index];
    }
}
