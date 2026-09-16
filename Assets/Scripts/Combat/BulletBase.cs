using UnityEngine;
using UnityEngine.InputSystem;

public class BulletBase : MonoBehaviour
{
    [SerializeField] private float _speed = 10f, liveTime = 3;
    [SerializeField] private bool isEnemyBullet = false;

    [SerializeField] private float damage = 10f;


    private EnemyDetector enemyDetector;

    public bool IsEnemyBullet
    {
        get => isEnemyBullet;
        set => isEnemyBullet = value;
    }

    private SpriteRenderer sr;
    private Rigidbody2D rg;

    void Start()
    {
        rg = this.GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, liveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rg.linearVelocity = this.transform.up * _speed;
    }

    public void SetColor(Color color)
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnemyBullet && collision.CompareTag("Enemy"))
        {
            enemyDetector = collision.GetComponent<EnemyDetector>();
            if(enemyDetector != null){
                enemyDetector.TakeDamage(damage);
            }
            Destroy(this.gameObject);
        }
        else if (isEnemyBullet && collision.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }else if(!isEnemyBullet && collision.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
    
}
