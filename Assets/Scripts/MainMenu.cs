using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public int demons;
    public int totalDemons;
    private int _clickPower = 1;
    private bool _isAutoClickActive = false;

    public Text demonsText;

    private void Start()
    {
        LoadData();
        UpdateUI();

        if (_isAutoClickActive)
        {
            StartCoroutine(IdleFarmRoutine());
        }
    }

    private void LoadData()
    {
        demons = PlayerPrefs.GetInt("demons", 0);
        totalDemons = PlayerPrefs.GetInt("total_demons", 0);
        _clickPower = PlayerPrefs.GetInt("clickPower", 1);
        _isAutoClickActive = PlayerPrefs.GetInt("isAutoClick", 0) == 1;
    }

    public void ButtonClick()
    {
        demons += _clickPower;
        totalDemons += _clickPower;

        SaveData();
        UpdateUI();

        AchMenu.UpdateAchievements();
        LoadData();
    }

    private IEnumerator IdleFarmRoutine()
    {
        while (_isAutoClickActive)
        {
            yield return new WaitForSeconds(1f);
            demons += _clickPower;
            totalDemons += _clickPower;

            SaveData();
            UpdateUI();

            AchMenu.UpdateAchievements();
            LoadData();
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("demons", demons);
        PlayerPrefs.SetInt("total_demons", totalDemons);
        PlayerPrefs.Save();

    }

    private void UpdateUI()
    {
        if (demonsText != null)
        {
            demonsText.text = demons.ToString();
        }
    }

    public void toAchievements()
    {
        SceneManager.LoadScene(1);
    }
}