using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour
{
    public Transform HpUiGroup;
    private int _hp = 5;
    public int Hp => _hp;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_hp == 0)
        {
            GlobalVariables.GameResult = "Game Over";

            Destroy(gameObject);
            Time.timeScale = 0;
            SceneManager.LoadScene("Scenes/GameResultScene", LoadSceneMode.Additive);
        }
    }

    public void DecreaseHp(int hpAmount)
    {
        _hp -= hpAmount;
        if (HpUiGroup.childCount != 0)
        {
            Destroy(HpUiGroup.GetChild(HpUiGroup.childCount - 1).gameObject);
        }
    }
}