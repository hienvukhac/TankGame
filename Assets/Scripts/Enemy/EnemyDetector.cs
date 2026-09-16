using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private Transform playerTarget;
    
    [Header("Chỉ số máu")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;


    [SerializeField] private MonoBehaviour[] scriptToDisableOnDeath;
    
    [Header("Thanh máu ")]
    [Tooltip("Image Type = Filled, Fill Method = Horizontal")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private GameObject healthBarRoot;
 
    [Header("Tốc độ giảm mượt")]
    [SerializeField] private float smoothSpeed = 1.5f;


    private float respawnDelay = 6f;

 
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    private Vector3 respawnPosition;
    private Coroutine smoothCoroutine;
    public Transform PlayerTarget => playerTarget;

    private bool isDead;
    public bool IsDead => isDead;


    private void Awake()
    {
        currentHealth = maxHealth;
        respawnPosition = this.transform.position;

        col = this.GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if(healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
        }
    }

    public void TakeDamage(float damage)
    {
         if (isDead) return; 
 
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
 
        float targetFill = currentHealth / maxHealth;
 
        if (smoothCoroutine != null)
            StopCoroutine(smoothCoroutine);
 
        smoothCoroutine = StartCoroutine(SmoothHealthBar(targetFill));
        Debug.Log("HP hiện tại" + currentHealth);
        if (currentHealth <= 0f)
            Die();
    }

    private IEnumerator SmoothHealthBar(float targetFill)
    {
        while (!Mathf.Approximately(healthBarFill.fillAmount, targetFill))
        {
            healthBarFill.fillAmount = Mathf.MoveTowards(
                healthBarFill.fillAmount,
                targetFill,
                smoothSpeed * Time.deltaTime
            );
            yield return null;
        }
 
        healthBarFill.fillAmount = targetFill; 
    }
 
    private void Die()
    {
        isDead = true;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        if(spriteRenderer != null) spriteRenderer.enabled = false;
        if(col != null) col.enabled = false;
        if(healthBarRoot != null) healthBarRoot.SetActive(false);

        foreach(MonoBehaviour script in scriptToDisableOnDeath)
        {
            if(script != null) script.enabled =false;
        }

        yield return new WaitForSeconds(respawnDelay);

        respawnPosition = this.transform.position;
        currentHealth = maxHealth;

        if(healthBarFill != null) healthBarFill.fillAmount = 1f;
        if(spriteRenderer != null) spriteRenderer.enabled = true;
        if(col != null) col.enabled = true;
        if(healthBarRoot != null) healthBarRoot.SetActive(true);

        isDead = false;

    }
    public void FindPlayerTarget(ref Rigidbody2D bulletPrefab)
    {
        if (playerTarget == null)
        {
            TankController hero = FindFirstObjectByType<TankController>();
            if (hero != null)
            {
                playerTarget = hero.transform;
                if (bulletPrefab == null)
                {
                    bulletPrefab = hero.BulletPrefab;
                }
            }
            else
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
            }
        }
    }

    public bool IsPlayerInScanRange()
    {
        if (playerTarget == null) return false;
        return Vector2.Distance(transform.position, playerTarget.position) <= detectionRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}