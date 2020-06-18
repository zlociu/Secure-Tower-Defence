using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        public Transform WaypointsGroup;

        private int _waypointIndex = 0;
        private int _speed = 40;
        private int _hp = 30;
        [SerializeField] private AudioClip _monsterSoundClip;
        private SoundManager _soundManager;
        private LevelManager _levelManager;
        public LevelManager LevelManager { set => _levelManager = value; }

        // Start is called before the first frame update
        void Start()
        {
            _soundManager = FindObjectOfType<SoundManager>();
            _soundManager.AddAudioSource(gameObject.GetInstanceID(), _monsterSoundClip);
        }

        // Update is called once per frame
        void Update()
        {
            if (_hp <= 0)
            {
                _destroy();
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

        private void _destroy()
        {
            _soundManager.RemoveAudioSource(gameObject.GetInstanceID());
            _levelManager.IncreaseMoney(10);
            Destroy(gameObject);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "base")
            {
                collision.gameObject.GetComponent<Base>().DecreaseHp(1);
                _destroy();
            }
        }

        public void DecreaseHp(int amount)
        {
            _hp -= amount;
            _soundManager.PlaySound(gameObject.GetInstanceID());
        }
    }
}