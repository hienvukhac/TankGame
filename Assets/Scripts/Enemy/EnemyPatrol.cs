using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyDetector))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolDistance = 1f;
    [SerializeField] private bool moveHorizontally = true;

    [Header("Cấu hình Enemy khi bị bắn")]
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int numberOfFlash = 3;

    private Color originalColor;
    private Coroutine flashCoroutine;


    private EnemyDetector enemyDetector;
    private Vector2 startPos;
    private int direction = 1;
    private SpriteRenderer child;
    private Animator anim;
    private Rigidbody2D rig;

    public enum EnemyState
    {
        IDLE,
        ATTACK,
        DIE
    }

    void Awake()
    {
        rig = this.GetComponent<Rigidbody2D>();
        child = this.GetComponentInChildren<SpriteRenderer>();
        anim = this.GetComponentInChildren<Animator>();
        enemyDetector = GetComponent<EnemyDetector>();

        if (child != null)
        {
            originalColor = child.color;
        }
    }

    void Start()
    {
        startPos = this.transform.position;
    }

    public void Move()
    {
        if (moveHorizontally)
        {
            rig.linearVelocity = new Vector2(direction * moveSpeed, 0);

            if (child != null)
            {
                child.flipX = (direction < 0);
            }

            if (this.transform.position.x > startPos.x + patrolDistance)
            {
                direction = -1;
            }
            else if (this.transform.position.x < startPos.x - patrolDistance)
            {
                direction = 1;
            }
        }
        else
        {
            rig.linearVelocity = new Vector2(0, direction * moveSpeed);

            if (this.transform.position.y > startPos.y + patrolDistance)
            {
                direction = -1;
            }
            else if (this.transform.position.y < startPos.y - patrolDistance)
            {
                direction = 1;
            }
        }

        UpdateAnimation(true);
    }

    public void Stop()
    {
        rig.linearVelocity = Vector2.zero;
        UpdateAnimation(false);
    }

    private void TakeHit()
    {
        if (child == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < numberOfFlash; i++)
        {
            child.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            child.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        child.color = originalColor;
        flashCoroutine = null;
    }

    private void UpdateAnimation(bool isMoving)
    {
        if (anim != null)
        {
            anim.SetBool("isMoving", isMoving && rig.linearVelocity.magnitude > 0.1f);
        }
    }

    public void ChangeAnimation()
    {
        if (enemyDetector == null || anim == null)
        {
            return;
        }

        anim.SetBool("isDie", enemyDetector.IsDead);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            direction *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy"))
        {
            return;
        }

        if (collision.transform.IsChildOf(this.transform))
        {
            return;
        }

        if (collision.CompareTag("PlayerBullet"))
        {
            TakeHit();

            if (anim != null)
            {
                anim.SetTrigger("TakeDamage");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)startPos : this.transform.position;

        if (moveHorizontally)
        {
            Gizmos.DrawLine(center + Vector3.left * patrolDistance, center + Vector3.right * patrolDistance);
        }
        else
        {
            Gizmos.DrawLine(center + Vector3.down * patrolDistance, center + Vector3.up * patrolDistance);
        }
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (child != null)
        {
            child.color = originalColor;
        }
    }
}