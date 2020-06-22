using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private PrefabManager _prefabManager;

    private readonly List<List<int>> _tileMap = GlobalVariables.CurrentLevel.MapToList();
    private GameObject _terrainTilesGroup;
    private GameObject _pathTilesGroup;
    private Bounds _bounds;
    private Vector2Int _spawnCoords;
    public Vector2Int SpawnCoords => _spawnCoords;
    public GameObject WaypointsGroup { get; private set; }

    // Start is called before the first frame update
    private void CreateLevel()
    {
        for (int y = 0; y < _tileMap.Count; y++)
        {
            for (int x = 0; x < _tileMap[y].Count; x++)
            {
                GameObject tile = SpawnObject(
                    Instantiate(_prefabManager.GetTilePrefab(_tileMap[y][x])),
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

    public void Setup()
    {
        _prefabManager = FindObjectOfType<PrefabManager>();
        foreach (KeyValuePair<int, string> entry in GlobalVariables.CurrentLevel.TilesToDict())
        {
            Debug.Log(entry.Key + " " + entry.Value);
            _prefabManager.SetTilePrefab(entry.Key, entry.Value);
        }

        _terrainTilesGroup = new GameObject("terrain_tiles");
        _pathTilesGroup = new GameObject("path_tiles");
        WaypointsGroup = new GameObject("waypoints");
        _bounds = new Bounds(transform.position, Vector3.one);

        CreateLevel();
    }

    public void CreateBase(GameObject lifeUiGroup)
    {
        Vector2Int startCoords = Vector2Int.down;

        // Gets start coordinates (left, right)
        for (int y = 0; y < _tileMap.Count - 1 && startCoords == Vector2Int.down; y++)
        {
            if (_tileMap[y][0] == 0)
            {
                startCoords.x = 0;
                startCoords.y = y;
                _spawnCoords = startCoords;
                _spawnCoords.x -= 1;
            }
            else if (_tileMap[y][_tileMap[y].Count - 1] == 0)
            {
                startCoords.x = _tileMap[y].Count - 1;
                startCoords.y = y;
                _spawnCoords = startCoords;
                _spawnCoords.x += 1;
            }
        }

        // Gets start coordinates (up, down)
        for (int x = 0; x < _tileMap.First().Count - 1 && startCoords == Vector2Int.down; x++)
        {
            if (_tileMap[0][x] == 0)
            {
                startCoords.x = x;
                startCoords.y = 0;
                _spawnCoords = startCoords;
                _spawnCoords.y -= 1;
            }
            else if (_tileMap[_tileMap.Count - 1][x] == 0)
            {
                startCoords.x = x;
                startCoords.y = _tileMap.Count - 1;
                _spawnCoords = startCoords;
                _spawnCoords.y += 1;
            }
        }

        GameObject startWaypoint = SpawnObject(
            new GameObject(),
            "waypoint_0",
            new Vector3Int(startCoords.x, startCoords.y, -2)
        );
        startWaypoint.transform.SetParent(WaypointsGroup.transform);

        SpawnObject(
            new GameObject(),
            "spawn_point",
            new Vector3Int(_spawnCoords.x, _spawnCoords.y, 0)
        );

        Vector2Int baseCoords = FindBaseCoordinates(startCoords);

        GameObject baseObject = SpawnObject(
            Instantiate(_prefabManager.BasePrefab),
            "base",
            new Vector3Int(baseCoords.x, baseCoords.y, -2)
        );
        Base baseComponent = baseObject.AddComponent<Base>();
        baseComponent.LifeUiGroup = lifeUiGroup.transform;
        for (int i = 0; i < baseComponent.Hp; i++)
        {
            GameObject lifeUi = SpawnObject(Instantiate(_prefabManager.LifePrefab), "life" + i, Vector3Int.zero);
            lifeUi.transform.SetParent(lifeUiGroup.transform);
            Vector3 position = Vector3Int.zero;
            position.x = (i + 1) * 49;
            position.z = lifeUiGroup.transform.localPosition.z;
            lifeUi.transform.localPosition = position;
        }

        GameObject baseWaypoint = SpawnObject(
            new GameObject(),
            "waypoint_base",
            new Vector3Int(baseCoords.x, baseCoords.y, -2)
        );
        baseWaypoint.transform.SetParent(WaypointsGroup.transform);
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
                waypoint.transform.SetParent(WaypointsGroup.transform);
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

    public GameObject SpawnObject(GameObject objectPrefab, string name1, Vector3Int coordinates)
    {
        float edgeLength = _prefabManager.GetTilePrefab(0).GetComponent<Renderer>().bounds.size[0];
        objectPrefab.transform.position =
            new Vector3(edgeLength * coordinates.x, edgeLength * (_tileMap.Count - coordinates.y), coordinates.z);
        objectPrefab.name = name1;
        return objectPrefab;
    }
}
