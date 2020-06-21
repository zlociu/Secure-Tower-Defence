using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTurretUi : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (ShowTurretCreationUi.ClearTurretUi())
        {
            FindObjectOfType<SoundManager>().PlayButtonSound();
        }
    }
}
