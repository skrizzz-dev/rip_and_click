using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AcMenu : MonoBehaviour
{
    public int total_demons;
    [SerializeField] Button firstAch;
    [SerializeField] bool isFirst;

    void Start()
    {
        total_demons = PlayerPrefs.GetInt("total_demons");
        isFirst = PlayerPrefs.GetInt("isFirst") == 1 ? true : false;
        if (total_demons >= 100 && !isFirst)
        {
            firstAch.interactable = true;
        }
        else
        {
            firstAch.interactable = false;
        }
    }

    public void GetFirst()
    {
        int demons = PlayerPrefs.GetInt("demons");
        demons += 10;
        PlayerPrefs.SetInt("demons", demons);
        isFirst = true;
        PlayerPrefs.SetInt("isFirst", isFirst ? 1 : 0);
    }

    public void toMenu()
    {
        SceneManager.LoadScene(0);
    }
}
