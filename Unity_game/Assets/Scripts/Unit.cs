using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        public Transform WaypointsGroup;

        private int _waypointIndex = 0;
        private int _speed = 60;
        public int Hp = 3;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Hp == 0)
            {
                Destroy(gameObject);
            }

            if (_waypointIndex >= WaypointsGroup.childCount)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position,
                WaypointsGroup.GetChild(_waypointIndex).position,
                _speed * Time.deltaTime);
            Vector2 dir = WaypointsGroup.GetChild(_waypointIndex).position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (Vector2.Distance(transform.position, WaypointsGroup.GetChild(_waypointIndex).position) < 0.1f)
            {
                _waypointIndex++;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "base")
            {
                collision.gameObject.GetComponent<Base>().Hp -= 1;
                Destroy(gameObject);
            }
        }
    }
}