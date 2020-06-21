using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Fields;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject _lifeUiGroup;
        [SerializeField] private GameObject _moneyUi;
        [SerializeField] private GameObject _waveUi;

        private PrefabManager _prefabManager;
        private MapManager _mapManager;

        private GameObject _unitsGroup;

        private List<Dictionary<string, EnemyWave>> _waves;
        private Dictionary<string, float> _enemySpawnPeriods;
        private int _currentWaveNumber = 1;
        private int _waveCount;
        private bool _waitForNextWave = true;

        private float _pausePeriod = 0f;
        public int Money { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            _prefabManager = FindObjectOfType<PrefabManager>();
            _mapManager = FindObjectOfType<MapManager>();
            _mapManager.Setup();
            SetupVariables();
            _mapManager.CreateBase(_lifeUiGroup);
            IncreaseMoney(GlobalVariables.CurrentLevel.startingMoney);
            _waveUi.GetComponent<Text>().text = "Wave " + _currentWaveNumber + "/" + _waveCount;
        }

        // Update is called once per frame
        void Update()
        {
            if (_waves.Count == 0 && _unitsGroup.transform.childCount == 0)
            {
                MusicManager.Stop();
                FindObjectOfType<SoundManager>().PlayVictorySound();
                GlobalVariables.GameResult = "Victory";
                Time.timeScale = 0;
                Destroy(this);
                SceneManager.LoadScene("Scenes/GameResultScene", LoadSceneMode.Additive);
                return;
            }

            if (_waitForNextWave && _pausePeriod < 5f)
            {
                _pausePeriod += Time.deltaTime;
                return;
            }

            if (_waitForNextWave)
            {
                _pausePeriod = 0f;
                _enemySpawnPeriods.Clear();
                foreach (KeyValuePair<string, EnemyWave> keyValue in _waves.First())
                {
                    _enemySpawnPeriods.Add(keyValue.Key, keyValue.Value.spawnTime);
                }

                _waitForNextWave = false;
                return;
            }

            if (_waves.First().Count == 0)
            {
                if (_currentWaveNumber < _waveCount)
                {
                    _currentWaveNumber++;
                }

                _waveUi.GetComponent<Text>().text = "Wave " + _currentWaveNumber + "/" + _waveCount;
                _waves.RemoveAt(0);
                _waitForNextWave = true;
                return;
            }

            _enemySpawnPeriods = _enemySpawnPeriods.ToDictionary(p => p.Key, p => p.Value + Time.deltaTime);

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            foreach (string key in _waves.First().Keys.ToList())
            {
                if (_enemySpawnPeriods[key] >= _waves.First()[key].spawnTime)
                {
                    SpawnEnemy(key.Replace("_", ""));
                    _waves.First()[key].amount--;
                    if (_waves.First()[key].amount <= 0)
                    {
                        _waves.First().Remove(key);
                    }

                    _enemySpawnPeriods[key] = 0f;
                }
            }
        }

        private void SetupVariables()
        {
            Money = 0;

            _waves = GlobalVariables.CurrentLevel.wavesToDictList();
            _waveCount = _waves.Count;
            _enemySpawnPeriods = new Dictionary<string, float>();

            _unitsGroup = new GameObject("units");
            CreateTurret.UnitsGroup = _unitsGroup;
            CreateTurret.LevelManagerVar = this;

            foreach (EnemyModel enemyModel in GlobalVariables.EnemyParams)
            {
                _prefabManager.SetEnemyPrefab(enemyModel);
            }
        }

        private void SpawnEnemy(string enemyName)
        {
            GameObject enemy = _mapManager.SpawnObject(
                Instantiate(_prefabManager.GetEnemyPrefab(enemyName)),
                "enemy " + enemyName,
                new Vector3Int(_mapManager.SpawnCoords.x, _mapManager.SpawnCoords.y, -2)
            );
            enemy.transform.SetParent(_unitsGroup.transform);
            enemy.GetComponent<Unit>().WaypointsGroup = _mapManager.WaypointsGroup.transform;
            enemy.GetComponent<Unit>().LevelManager = this;
            enemy.SetActive(true);
        }

        public void IncreaseMoney(int inc)
        {
            Money += inc;
            _moneyUi.GetComponent<Text>().text = Money.ToString();
        }

        public void DecreaseMoney(int dec)
        {
            Money -= dec;
            if (Money < 0)
            {
                Money = 0;
            }

            _moneyUi.GetComponent<Text>().text = Money.ToString();
        }
    }
}