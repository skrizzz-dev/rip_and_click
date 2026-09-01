using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] int killed_demons;
    public int total_demons;
    public Text demonsText;

    void Start()
    {
        killed_demons = PlayerPrefs.GetInt("demons");
        total_demons = PlayerPrefs.GetInt("total_demons");
    }
    public void ButtonClick()
    {
        killed_demons++;
        total_demons++;
        PlayerPrefs.SetInt("demons", killed_demons);
        PlayerPrefs.SetInt("total_demons", total_demons);
    }

    public void ToAchievements()
    {
        SceneManager.LoadScene(1);
    }
    void Update()
    {
        demonsText.text = killed_demons.ToString();
    }
}
