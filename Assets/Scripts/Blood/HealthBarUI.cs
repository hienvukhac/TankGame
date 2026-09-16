using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private float smoothSpeed = 3f;
    private float _targetFillAmount = 1f;

    private void Start()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = 1f;
            _targetFillAmount = 1f;
        }
    }

    private void Update()
    {
        if (healthImage != null && healthImage.fillAmount != _targetFillAmount)
        {
            healthImage.fillAmount = Mathf.MoveTowards(
                healthImage.fillAmount,
                _targetFillAmount,
                smoothSpeed * Time.deltaTime
            );
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (maxHealth > 0)
        {
            _targetFillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }
}
