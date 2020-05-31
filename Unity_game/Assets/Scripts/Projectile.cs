using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform Unit;

    private int _speed = 100;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Unit == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,
                Unit.position,
                _speed * Time.deltaTime);
            Vector2 dir = Unit.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == Unit.gameObject.name)
        {
            Destroy(gameObject);
            Unit.GetComponent<Unit>().DecreaseHp(1);
        }
    }
}