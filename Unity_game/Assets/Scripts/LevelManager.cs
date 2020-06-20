using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Fields;
using Assets.Scripts.Turret;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        private Dictionary<int, GameObject> _tiles;
        [SerializeField] private GameObject _pathTilePrefab;
        [SerializeField] private GameObject _terrainTilePrefab;
        [SerializeField] private GameObject _base;
        [SerializeField] private GameObject _enemyPrefab;
        private Dictionary<string, GameObject> _enemyPrefabs;
        [SerializeField] private GameObject _lifeUiGroup;
        [SerializeField] private GameObject _lifePrefab;
        [SerializeField] private GameObject _moneyUi;
        [SerializeField] private GameObject _waveUi;
        private GameObject _waypointsGroup;
        private GameObject _unitsGroup;
        private GameObject _terrainTilesGroup;
        private GameObject _pathTilesGroup;

        private List<Dictionary<string, EnemyWave>> _waves;
        private Dictionary<string, float> _enemySpawnPeriods;
        private int _currentWaveNumber = 1;
        private int _waveCount;
        private bool _waitForNextWave = false;

        private Vector2Int _spawnCoords;
        private float _pausePeriod = 0f;
        public int Money { get; private set; }

        private readonly List<List<int>> _tileMap = GlobalVariables.CurrentLevel.MapToList();

        private Bounds _bounds;

        // Start is called before the first frame update
        void Start()
        {
            Money = 0;
            SetupVariables();
            CreateLevel();
            CreateBase();
            IncreaseMoney(GlobalVariables.CurrentLevel.startingMoney);
            _waveUi.GetComponent<Text>().text = "Wave " + _currentWaveNumber + "/" + _waveCount;
        }

        // Update is called once per frame
        void Update()
        {
            if (_waves.Count == 0 && _unitsGroup.transform.childCount == 0)
            {
                GlobalVariables.GameResult = "Victory";
                Time.timeScale = 0;
                Destroy(gameObject);
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
                _currentWaveNumber++;
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
            _waves = GlobalVariables.CurrentLevel.wavesToDictList();
            _waveCount = _waves.Count;
            _enemySpawnPeriods = new Dictionary<string, float>();
            foreach (string key in _waves.First().Keys)
            {
                _enemySpawnPeriods.Add(key, 0);
            }

            SetupTilePrefabs();

            _bounds = new Bounds(transform.position, Vector3.one);

            _waypointsGroup = new GameObject("waypoints");
            _unitsGroup = new GameObject("units");
            _terrainTilesGroup = new GameObject("terrain_tiles");
            CreateTurret.UnitsGroup = _unitsGroup;
            CreateTurret.LevelManagerVar = this;
            _pathTilesGroup = new GameObject("path_tiles");
            SetupEnemyPrefabs();
        }

        private void SetupEnemyPrefabs()
        {
            _enemyPrefabs = new Dictionary<string, GameObject>();
            GameObject prefabs = new GameObject("enemy_prefabs");
            foreach (EnemyModel enemyModel in GlobalVariables.EnemyParams)
            {
                GameObject prefab = Instantiate(_enemyPrefab);
                prefab.GetComponent<SpriteRenderer>().sprite =
                    ResourceUtil.LoadSprite(enemyModel.texture);
                _enemyPrefabs.Add(enemyModel.name, prefab);
                Unit unit = prefab.GetComponent<Unit>();
                unit.Hp = enemyModel.hitPoints;
                unit.Speed = enemyModel.speed;
                unit.MoneyReward = enemyModel.moneyReward;
                unit.MonsterSoundClip = ResourceUtil.LoadSound(enemyModel.sound);

                prefab.SetActive(false);
                prefab.name = "prefab_" + enemyModel.name;
                prefab.transform.parent = prefabs.transform;
            }
        }

        private void SetupTilePrefabs()
        {
            _tiles = new Dictionary<int, GameObject>();
            GameObject prefabs = new GameObject("tile_prefabs");
            foreach (KeyValuePair<int, string> entry in GlobalVariables.CurrentLevel.TilesToDict())
            {
                GameObject prefab = entry.Key != 0 ? _terrainTilePrefab : _pathTilePrefab;
                prefab = Instantiate(prefab);
                prefab.GetComponent<SpriteRenderer>().sprite =
                    ResourceUtil.LoadSprite(entry.Value);
                _tiles.Add(entry.Key, prefab);
                prefab.SetActive(false);
                prefab.name = "prefab_" + entry.Key;
                prefab.transform.parent = prefabs.transform;
            }
        }

        private void CreateLevel()
        {
            for (int y = 0; y < _tileMap.Count; y++)
            {
                for (int x = 0; x < _tileMap[y].Count; x++)
                {
                    GameObject tile = SpawnObject(
                        Instantiate(_tiles[_tileMap[y][x]]),
                        "tile_" + x + "_" + y,
                        new Vector3Int(x, y, 0)
                    );
                    tile.SetActive(true);
                    if (_tileMap[y][x] == 1)
                    {
                        tile.transform.SetParent(_terrainTilesGroup.transform);
                    }
                    else if (_tileMap[y][x] == 0)
                    {
                        tile.transform.SetParent(_pathTilesGroup.transform);
                    }
                }
            }

            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _bounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer tileRenderer in _terrainTilesGroup.GetComponentsInChildren<Renderer>())
            {
                _bounds.Encapsulate(tileRenderer.bounds);
            }

            Debug.Log("The bounds of this model is " + _bounds);
            transform.rotation = currentRotation;
            Camera.main.transform.position = _bounds.center;
            Camera.main.GetComponent<CameraZoom>().MaxOrtho = _bounds.center.x * 1.5f;
            Camera.main.GetComponent<CameraZoom>().ApplyOrthoLimits();
            Camera.main.GetComponent<CameraMovement>().Bounds = _bounds;
        }

        private void CreateBase()
        {
            Vector2Int startCoords = Vector2Int.down;

            // Gets start coordinates
            for (int y = 0; y < _tileMap.Count - 1; y++)
            {
                if (_tileMap[y][0] == 0)
                {
                    startCoords.x = 0;
                    startCoords.y = y;
                    _spawnCoords = startCoords;
                    _spawnCoords.x -= 1;
                    break;
                }
                else if (_tileMap[y][_tileMap[y].Count - 1] == 0)
                {
                    startCoords.x = _tileMap[y].Count - 1;
                    startCoords.y = y;
                    _spawnCoords = startCoords;
                    _spawnCoords.x += 1;
                    break;
                }
            }

            GameObject startWaypoint = SpawnObject(
                new GameObject(),
                "waypoint_0",
                new Vector3Int(startCoords.x, startCoords.y, -2)
            );
            startWaypoint.transform.SetParent(_waypointsGroup.transform);

            SpawnObject(
                new GameObject(),
                "spawn_point",
                new Vector3Int(_spawnCoords.x, _spawnCoords.y, 0)
            );

            Vector2Int baseCoords = FindBaseCoordinates(startCoords);

            GameObject baseObject = SpawnObject(
                Instantiate(_base),
                "base",
                new Vector3Int(baseCoords.x, baseCoords.y, -2)
            );
            Base baseComponent = baseObject.AddComponent<Base>();
            baseComponent.HpUiGroup = _lifeUiGroup.transform;
            for (int i = 0; i < baseComponent.Hp; i++)
            {
                GameObject lifeUi = SpawnObject(Instantiate(_lifePrefab), "life" + i, Vector3Int.zero);
                lifeUi.transform.SetParent(_lifeUiGroup.transform);
                Vector3 position = Vector3Int.zero;
                position.x = (i + 1) * 49;
                position.z = _lifeUiGroup.transform.localPosition.z;
                lifeUi.transform.localPosition = position;
            }

            GameObject baseWaypoint = SpawnObject(
                new GameObject(),
                "waypoint_base",
                new Vector3Int(baseCoords.x, baseCoords.y, -2)
            );
            baseWaypoint.transform.SetParent(_waypointsGroup.transform);
        }

        private Vector2Int FindBaseCoordinates(Vector2Int startCoords)
        {
            Vector2Int previousCoords = startCoords;
            Vector2Int currentCoords = startCoords;

            // Sets next current coordinates
            if (Math.Abs(startCoords.x) < 0.1f)
            {
                currentCoords.x += 1;
            }
            else if (Math.Abs(startCoords.x - (_tileMap[startCoords.y].Count - 1)) < 0.1)
            {
                currentCoords.x -= 1;
            }
            else if (Math.Abs(startCoords.y) < 0.1f)
            {
                currentCoords.y += 1;
            }
            else if (Math.Abs(startCoords.y - (_tileMap.Count - 1)) < 0.1)
            {
                currentCoords.y -= 1;
            }

            Vector2Int baseCoords = Vector2Int.down;
            int tries = 0;
            int waypointNum = 1;
            string coordsName = "";
            while (baseCoords == Vector2Int.down)
            {
                string previousCoordsName = coordsName;
                Vector2Int upCoords = new Vector2Int(currentCoords.x, currentCoords.y + 1);
                Vector2Int downCoords = new Vector2Int(currentCoords.x, currentCoords.y - 1);
                Vector2Int leftCoords = new Vector2Int(currentCoords.x - 1, currentCoords.y);
                Vector2Int rightCoords = new Vector2Int(currentCoords.x + 1, currentCoords.y);
                if (upCoords != previousCoords && _tileMap[upCoords.y][upCoords.x] == 0)
                {
                    previousCoords = currentCoords;
                    currentCoords = upCoords;
                    coordsName = "up";
                }
                else if (downCoords != previousCoords && _tileMap[downCoords.y][downCoords.x] == 0)
                {
                    previousCoords = currentCoords;
                    currentCoords = downCoords;
                    coordsName = "down";
                }
                else if (leftCoords != previousCoords && _tileMap[leftCoords.y][leftCoords.x] == 0)
                {
                    previousCoords = currentCoords;
                    currentCoords = leftCoords;
                    coordsName = "left";
                }
                else if (rightCoords != previousCoords && _tileMap[rightCoords.y][rightCoords.x] == 0)
                {
                    previousCoords = currentCoords;
                    currentCoords = rightCoords;
                    coordsName = "right";
                }
                else
                {
                    baseCoords = currentCoords;
                }

                if (previousCoordsName != coordsName)
                {
                    GameObject waypoint = SpawnObject(
                        new GameObject(),
                        "waypoint_" + waypointNum,
                        new Vector3Int(previousCoords.x, previousCoords.y, -2)
                    );
                    waypoint.transform.SetParent(_waypointsGroup.transform);
                    waypointNum += 1;
                }

                tries++;
                if (tries >= _tileMap.Count * _tileMap[0].Count)
                {
                    break;
                }
            }

            return baseCoords;
        }

        private void SpawnEnemy(string enemyName)
        {
            GameObject enemy = SpawnObject(
                Instantiate(_enemyPrefabs[enemyName]),
                "enemy",
                new Vector3Int(_spawnCoords.x, _spawnCoords.y, -2)
            );
            enemy.transform.SetParent(_unitsGroup.transform);
            enemy.GetComponent<Unit>().WaypointsGroup = _waypointsGroup.transform;
            enemy.GetComponent<Unit>().LevelManager = this;
            enemy.SetActive(true);
        }

        private GameObject SpawnObject(GameObject gameObject1, string name1, Vector3Int coordinates)
        {
            float edgeLength = _tiles[0].GetComponent<Renderer>().bounds.size[0];
            gameObject1.transform.position =
                new Vector3(edgeLength * coordinates.x, edgeLength * (_tileMap.Count - coordinates.y), coordinates.z);
            gameObject1.name = name1;
            return gameObject1;
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