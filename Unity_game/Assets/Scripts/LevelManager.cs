using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _tiles;
        [SerializeField] private GameObject _base;
        [SerializeField] private GameObject _enemy;
        [SerializeField] private GameObject _turretChoice;
        private GameObject _waypointsGroup;
        private GameObject _unitsGroup;
        private GameObject _terrainTilesGroup;
        private GameObject _pathTilesGroup;

        private Vector2Int _spawnCoords;
        public float SpawnPeriod;

        private List<List<int>> _tileMap = new List<List<int>>
        {
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0},
            new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0},
            new List<int> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };

        private Bounds bounds;

        // Start is called before the first frame update
        void Start()
        {
            bounds = new Bounds(transform.position, Vector3.one);

            _waypointsGroup = new GameObject("waypoints");
            _unitsGroup = new GameObject("units");
            _terrainTilesGroup = new GameObject("terrain_tiles");
            _turretChoice.GetComponentInChildren<TurretCreation>().UnitsGroup = _unitsGroup;
            _pathTilesGroup = new GameObject("path_tiles");
            CreateLevel();
            CreateBase();
        }

        // Update is called once per frame
        void Update()
        {
            if (SpawnPeriod > 1f)
            {
                SpawnEnemy();
                SpawnPeriod = 0f;
            }

            SpawnPeriod += Time.deltaTime;
        }

        private void CreateLevel()
        {
            float edgeLength = _tiles[0].GetComponent<Renderer>().bounds.size[0];
            for (int y = 0; y < _tileMap.Count; y++)
            {
                for (int x = 0; x < _tileMap[y].Count; x++)
                {
                    GameObject tile = SpawnObject(
                        Instantiate(_tiles[_tileMap[y][x]]),
                        "tile_" + x + "_" + y,
                        new Vector3Int(x, y, 0)
                    );
                    if (_tileMap[y][x] == 0)
                    {
                        tile.transform.SetParent(_terrainTilesGroup.transform);
                    }
                    else if (_tileMap[y][x] == 1)
                    {
                        tile.transform.SetParent(_pathTilesGroup.transform);
                    }
                }
            }

            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            bounds = new Bounds(transform.position, Vector3.zero);
            foreach (Renderer renderer in _terrainTilesGroup.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            Debug.Log("The bounds of this model is " + bounds);
            transform.rotation = currentRotation;
            Camera.main.transform.position = bounds.center;
            Camera.main.GetComponent<CameraZoom>().MaxOrtho = bounds.center.x;
            Camera.main.GetComponent<CameraZoom>().ApplyOrthoLimits();
            Camera.main.GetComponent<CameraMovement>().Bounds = bounds;

            _turretChoice.GetComponentInChildren<TurretCreation>().TerrainTilesGroup = _terrainTilesGroup;
        }

        private void CreateBase()
        {
            Vector2Int startCoords = Vector2Int.down;
            float edgeLength = _tiles[0].GetComponent<Renderer>().bounds.size[0];

            // Gets start coordinates
            for (int y = 0; y < _tileMap.Count - 1; y++)
            {
                if (_tileMap[y][0] == 1)
                {
                    startCoords.x = 0;
                    startCoords.y = y;
                    _spawnCoords = startCoords;
                    _spawnCoords.x -= 1;
                    break;
                }
                else if (_tileMap[y][_tileMap[y].Count - 1] == 1)
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
            baseObject.AddComponent<Base>();

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
                if (upCoords != previousCoords && _tileMap[upCoords.y][upCoords.x] == 1)
                {
                    previousCoords = currentCoords;
                    currentCoords = upCoords;
                    coordsName = "up";
                }
                else if (downCoords != previousCoords && _tileMap[downCoords.y][downCoords.x] == 1)
                {
                    previousCoords = currentCoords;
                    currentCoords = downCoords;
                    coordsName = "down";
                }
                else if (leftCoords != previousCoords && _tileMap[leftCoords.y][leftCoords.x] == 1)
                {
                    previousCoords = currentCoords;
                    currentCoords = leftCoords;
                    coordsName = "left";
                }
                else if (rightCoords != previousCoords && _tileMap[rightCoords.y][rightCoords.x] == 1)
                {
                    previousCoords = currentCoords;
                    currentCoords = rightCoords;
                    coordsName = "right";
                }
                else
                {
                    baseCoords = currentCoords;
                }

                if (previousCoordsName != "" && previousCoordsName != coordsName)
                {
                    GameObject waypoint = SpawnObject(
                        new GameObject(),
                        "waypoint_" + waypointNum,
                        new Vector3Int(previousCoords.x, previousCoords.y, -2)
                    );
                    waypoint.transform.SetParent(_waypointsGroup.transform);
                    // waypoint.GetComponent<Renderer>().enabled = false;
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

        private void SpawnEnemy()
        {
            GameObject enemy = SpawnObject(
                Instantiate(_enemy),
                "enemy",
                new Vector3Int(_spawnCoords.x, _spawnCoords.y, -2)
            );
            enemy.transform.SetParent(_unitsGroup.transform);
            Unit unitScript = enemy.AddComponent<Unit>();
            unitScript.WaypointsGroup = _waypointsGroup.transform;
        }

        private GameObject SpawnObject(GameObject gameObject1, string name1, Vector3Int coordinates)
        {
            float edgeLength = _tiles[0].GetComponent<Renderer>().bounds.size[0];
            gameObject1.transform.position =
                new Vector3(edgeLength * coordinates.x, edgeLength * (_tileMap.Count - coordinates.y), coordinates.z);
            gameObject1.name = name1;
            return gameObject1;
        }
    }
}