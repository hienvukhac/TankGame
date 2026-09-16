using UnityEngine;

[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(EnemyDetector))]
[RequireComponent(typeof(EnemyCombat))]
public class EnemyController : MonoBehaviour
{
    private EnemyPatrol patrol;
    private EnemyDetector detector;
    private EnemyCombat combat;

    void Awake()
    {
        patrol = this.GetComponent<EnemyPatrol>();
        detector = this.GetComponent<EnemyDetector>();
        combat = this.GetComponent<EnemyCombat>();
    }

    void Start()
    {
        Rigidbody2D bulletPref = combat.BulletPrefab;
        detector.FindPlayerTarget(ref bulletPref);
        combat.BulletPrefab = bulletPref;
    }

    void Update()
    {
        if (detector.PlayerTarget == null)
        {
            Rigidbody2D bulletPref = combat.BulletPrefab;
            detector.FindPlayerTarget(ref bulletPref);
            combat.BulletPrefab = bulletPref;
            if (detector.PlayerTarget == null) return;
        }
        
        if (detector.IsPlayerInScanRange())
        {
            combat.PerformShooting(detector.PlayerTarget.position);
        }
        else
        {
            combat.ResetTimers();
        }

        patrol.ChangeAnimation();
    }

    void FixedUpdate()
    {
        if (detector.IsPlayerInScanRange())
        {
            patrol.Stop();
        }
        else
        {
            patrol.Move();
        }
    }
}