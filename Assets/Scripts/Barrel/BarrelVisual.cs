using UnityEngine;

public class BarrelVisual : MonoBehaviour
{

    private Animator anim;

    public enum BarrelType
    {
        Barrel_Stand,
        Barrel_Diagonal_left
    }
    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            if (anim != null)
            {
                anim.SetTrigger("Explode");
                Destroy(collision.gameObject);
            }
        }
    }
}
