using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Firing Settings")]
    [SerializeField] private float initialShootDelay = 2.5f;
    [SerializeField] private float shootInterval = 1.0f;
    [SerializeField] private Rigidbody2D bulletPrefab;
    // [SerializeField] private Color bulletColor = Color.red;
    [SerializeField] private bool showLaserBeam = true;
    [SerializeField] private float laserDuration = 0.15f;

    public Rigidbody2D BulletPrefab { get => bulletPrefab; set => bulletPrefab = value; }

    private float detectionTimer = 0f;
    private float shootTimer = 0f;
    private bool isFirstShotFired = false;

    private LineRenderer lineRenderer;
    private float laserTimer = 0f;
    private EnemyDetector enemyDetector;

    private void Awake()
    {
        enemyDetector = GetComponent<EnemyDetector>();
    }

    void Update()
    {
        HandleLaserTimer();
    }

    public void PerformShooting(Vector3 targetPosition)
    {
        if (enemyDetector != null && enemyDetector.IsDead)
        {
            ResetTimers();
            return;
        }

        if (!isFirstShotFired)
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= initialShootDelay)
            {
                ShootAtTarget(targetPosition);
                isFirstShotFired = true;
                shootTimer = 0f;
            }
        }
        else
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                shootTimer = 0f;
                ShootAtTarget(targetPosition);
            }
        }
    }

    public void ResetTimers()
    {
        detectionTimer = 0f;
        shootTimer = 0f;
        isFirstShotFired = false;
    }

    private void ShootAtTarget(Vector3 targetPosition)
    {
        Vector2 origin = transform.position;
        Vector2 dir = ((Vector2)targetPosition - origin).normalized;
        if (dir == Vector2.zero) dir = Vector2.up;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        if (bulletPrefab != null)
        {
            Rigidbody2D spawnedBullet = Instantiate<Rigidbody2D>(bulletPrefab, origin, rotation);
            BulletBase bulletScript = spawnedBullet.GetComponent<BulletBase>();
            if (bulletScript != null)
            {
                bulletScript.IsEnemyBullet = true;
                //bulletScript.SetColor(bulletColor);
            }
            else
            {
                SpriteRenderer sr = spawnedBullet.GetComponentInChildren<SpriteRenderer>();
                //if (sr != null) sr.color = bulletColor;
            }
        }

        if (showLaserBeam)
        {
            DrawLaserBeam(origin, targetPosition);
        }
    }

    private void DrawLaserBeam(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.08f;
                lineRenderer.endWidth = 0.04f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.sortingOrder = 10;
            }
        }

       // lineRenderer.startColor = bulletColor;
        //lineRenderer.endColor = bulletColor;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
        laserTimer = laserDuration;
    }

    private void HandleLaserTimer()
    {
        if (lineRenderer != null && lineRenderer.enabled)
        {
            laserTimer -= Time.deltaTime;
            if (laserTimer <= 0)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}