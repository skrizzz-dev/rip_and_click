using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreMenu : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public string title;
        public int requiredScore;
        public Sprite characterSprite;
        public Sprite lockedSprite;
    }

    public int totalDemons;

    public List<Character> charactersConfig = new List<Character>()
    {
        new Character { title = "Marine", requiredScore = 0},
        new Character { title = "Doomguy", requiredScore = 500},
        new Character { title = "Slayer", requiredScore = 2000},
        new Character { title = "Doom Slayer", requiredScore = 10000},
        new Character { title = "???", requiredScore = 666 },
        new Character { title = "???", requiredScore = 100000 }
    };

    public GameObject buttonPrefab;
    public Transform contentTransform;

    private readonly List<GameObject> _spawnedButtons = new List<GameObject>();

    private const string SELECTED_KEY = "selectedCharacter";

    private void Start()
    {
        totalDemons = PlayerPrefs.GetInt("total_demons", 0);
        RenderCharactersList();
    }

    private void RenderCharactersList()
    {
        foreach (var elem in _spawnedButtons)
        {
            if (elem != null) Destroy(elem);
        }
        _spawnedButtons.Clear();

        if (buttonPrefab == null || contentTransform == null) return;

        int selectedIndex = PlayerPrefs.GetInt(SELECTED_KEY, 0);

        for (int i = 0; i < charactersConfig.Count; i++)
        {
            int index = i;
            Character ch = charactersConfig[index];

            GameObject item = Instantiate(buttonPrefab, contentTransform);
            _spawnedButtons.Add(item);

            bool isUnlocked = totalDemons >= ch.requiredScore;
            bool isSelected = (selectedIndex == index);

            string displayTitle = ch.title;

            if (ch.title == "???" && isUnlocked)
            {
                if (ch.requiredScore == 666)
                {
                    displayTitle = "undead slayer";
                }
                else if (ch.requiredScore == 100000)
                {
                    displayTitle = "IDDQD";
                }
            }

            Text itemText = item.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                string status;
                if (!isUnlocked) status = "<color=red>Закрыто</color>";
                else if (isSelected) status = "<color=yellow>Выбран</color>";
                else status = "<color=green>Доступен</color>";

                itemText.text = $"{displayTitle}\n{ch.requiredScore} демонов - {status}";

                itemText.color = Color.white;
            }

            Image[] images = item.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject == item) continue;

                if (!isUnlocked && ch.lockedSprite != null)
                    img.sprite = ch.lockedSprite;
                else if (ch.characterSprite != null)
                    img.sprite = ch.characterSprite;
            }
            
            Button btn = item.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.interactable = isUnlocked;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnCharacterClicked(index));
            }
        }
    }

    private void OnCharacterClicked(int index)
    {
        PlayerPrefs.SetInt(SELECTED_KEY, index);
        PlayerPrefs.Save();

        RenderCharactersList();
    }

    public void toMenu()
    {
        SceneManager.LoadScene(0);
    }

}
