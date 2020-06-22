using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ShowGameResult : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Text>().text = GlobalVariables.GameResult;
    }
}
