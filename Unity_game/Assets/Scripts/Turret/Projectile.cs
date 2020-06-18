using Assets.Scripts;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform TargetUnit;

    public int Speed = 100;
    public int Damage = 10;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetUnit == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,
                TargetUnit.position,
                Speed * Time.deltaTime);
            Vector2 dir = TargetUnit.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || TargetUnit == null)
        {
            return;
        }

        if (collision.gameObject == TargetUnit.gameObject)
        {
            Destroy(gameObject);
            TargetUnit.GetComponent<Unit>().DecreaseHp(Damage);
        }
    }
}