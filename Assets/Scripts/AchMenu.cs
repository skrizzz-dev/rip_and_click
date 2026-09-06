using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;


public class AchMenu : MonoBehaviour
{
    [System.Serializable]
    public class Achievement
    {
        public string title;
        public int requiredScore;
        public int clickMultiplier;
        public bool unlockAutoCLick;
        public Sprite icon;
    }
    public int totalDemons;

    public Sprite unlockedIcon;
    public Sprite lockedIcon;

    public List<Achievement> achievementsConfig = new List<Achievement>()
    {
        new Achievement { title = "E1M1", requiredScore = 100, clickMultiplier = 1, unlockAutoCLick = false},
        new Achievement { title = "Glory Kill", requiredScore = 500, clickMultiplier = 2, unlockAutoCLick = false},
        new Achievement { title = "GodKiller", requiredScore = 1000, clickMultiplier = 3, unlockAutoCLick = false},
        new Achievement { title = "The Only Thing They Fear Is Your Click", requiredScore = 2000, clickMultiplier = 4, unlockAutoCLick = false},
        new Achievement { title = "Rampage", requiredScore = 5000, clickMultiplier = 5, unlockAutoCLick = false},
        new Achievement { title = "Rip and Tear", requiredScore = 10000, clickMultiplier = 5, unlockAutoCLick = true}
    };

    public GameObject buttonPrefab;
    public Transform contentTransform;

    private readonly List<GameObject> _spawnedButtons = new List<GameObject>();
    
    private void Start()
    {
        totalDemons = PlayerPrefs.GetInt("total_demons", 0);

        CalculateProgressAndSave();
        RenderAchievementsList();
    }

    private void CalculateProgressAndSave()
    {
        int currentClickPower = 1;
        bool isAutoClickerUnlocked = false;

        foreach (var ach in achievementsConfig)
        {
            if (totalDemons >= ach.requiredScore)
            {
                currentClickPower = ach.clickMultiplier;

                if (ach.unlockAutoCLick)
                {
                    isAutoClickerUnlocked = true;
                }
            }
        }

        PlayerPrefs.SetInt("clickPower", currentClickPower);
        PlayerPrefs.SetInt("isAutoClick", isAutoClickerUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void RenderAchievementsList()
    {
        foreach (var elem in _spawnedButtons)
        {
            if (elem != null) Destroy(elem);
        }
        _spawnedButtons.Clear();

        if (buttonPrefab == null) return; 

        foreach (var ach in achievementsConfig)
        {
            GameObject item = Instantiate(buttonPrefab, contentTransform);
            Text itemText = item.GetComponentInChildren<Text>();
            bool isUnlocked = totalDemons >= ach.requiredScore;

            if (itemText != null)
            {
                itemText.text = $"{ach.title}\n{ach.requiredScore} демонов - " +
                    (isUnlocked ? "<color=green>Получено</color>" : "<color=red>Закрыто</color>");
            }

            Image[] images = item.GetComponentsInChildren<Image>();

            foreach (Image img in images)
            {
                if (img.gameObject != item)
                {
                    img.sprite = isUnlocked ? unlockedIcon : lockedIcon;
                }
            }
            _spawnedButtons.Add(item);
        }
    }
    public void toMenu()
    {
        SceneManager.LoadScene(0);
    }
}
