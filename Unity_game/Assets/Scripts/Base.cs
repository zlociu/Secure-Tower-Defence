using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour
{
    public Transform LifeUiGroup;
    public int Hp { get; private set; } = 5;
    private SoundManager _soundManager;

    // Start is called before the first frame update
    void Start()
    {
        _soundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Hp == 0)
        {
            MusicManager.Stop();
            _soundManager.PlayBaseExplosionSound();
            GlobalVariables.GameResult = "Game Over";

            Destroy(gameObject);
            Time.timeScale = 0;
            SceneManager.LoadScene("Scenes/GameResultScene", LoadSceneMode.Additive);
        }
    }

    public void DecreaseHp(int hpAmount)
    {
        Hp -= hpAmount;
        _soundManager.PlayBaseDamageSound();
        if (LifeUiGroup.childCount != 0)
        {
            Destroy(LifeUiGroup.GetChild(LifeUiGroup.childCount - 1).gameObject);
        }
    }
}