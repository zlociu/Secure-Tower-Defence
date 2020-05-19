using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildTower : MonoBehaviour
{
    private static GameObject _turretUi = null;

    [SerializeField] private GameObject _turretUiPrefab;

    public void OnMouseDown()
    {
        if (_turretUi != null)
        {
            Destroy(_turretUi);
        }

        _turretUi = Instantiate(_turretUiPrefab);
        _turretUi.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        _turretUi.GetComponent<TurretCreation>().buildTile = transform;
        Vector3 uiPosition = transform.position;
        uiPosition.y += 10;
        uiPosition.z = -2;
        _turretUi.transform.position = uiPosition;
    }
}