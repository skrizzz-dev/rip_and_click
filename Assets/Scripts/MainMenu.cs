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

    [Header("Потверждение сброса")]
    public GameObject resetPanel;

    [Header("Анимации")]
    public GameObject[] clickEffectPrefabs;
    public RectTransform buttonRect;

    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip achievementSound;
    public AudioClip[] shootSounds;
    public AudioClip[] demonSoundsDoom1;
    public AudioClip[] demonSounds2016;
    public AudioClip[] demonSoundsEternal;

    public AudioSource musicSource;
    public AudioClip[] doomMusic;


    public Image backgroundImage;
    public Sprite[] backgroundSprites;
    public Image doomguyImage;
    public Sprite[] doomguySprites;
    private bool isMuted = false;

    [Header("Звуки демонов рандом")]
    private int _clicksSinceLastDemon = 0;
    private int _demonPlaysWindow = 0;
    private const int WINDOW_SIZE = 5;
    private const int MIN_DEMON_PLAYS = 1;
    private const int MAX_DEMON_PLAYS = 4;


    private int _lastAchievemntIndex = -1;

    private void Start()
    {
        LoadData();
        UpdateUI();
        UpdateDoomguySprite();
        UpdateBackGround();
        UpdateDoomMusic();
        ApplyMuteState();

        _lastAchievemntIndex = GetCurrentAchievementIndex();

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
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
    }

    public void ButtonClick()
    {
        demons += _clickPower;
        totalDemons += _clickPower;

        SaveData();
        UpdateUI();

        PlayClickSound();
        PlayClickEffect();

        AchMenu.UpdateAchievements();
        LoadData();

        CheckAchievements();
    }

    void PlayClickEffect()
    {
        if (clickEffectPrefabs == null || clickEffectPrefabs.Length == 0) return;
        if (buttonRect == null) return;

        int index = Mathf.Clamp(_clickPower - 1, 0, clickEffectPrefabs.Length - 1);
        GameObject prefab = clickEffectPrefabs[index];
        if (prefab == null) return;

        Canvas canvas = buttonRect.GetComponent<Canvas>();
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            buttonRect,
            buttonRect.position,
            uiCamera,
            out worldPos
            );

        worldPos.z = 0;
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMuteState();

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ApplyMuteState()
    {
        if (isMuted)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = 1f;
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.SetInt("demons", 0);
        PlayerPrefs.SetInt("total_demons", 0);
        PlayerPrefs.SetInt("clickPower", 1);
        PlayerPrefs.SetInt("isAutoClick", 0);
        PlayerPrefs.SetInt("selectedCharacter", 0);

        PlayerPrefs.Save();
        LoadData();
        UpdateUI();
        UpdateDoomguySprite();
        UpdateBackGround();
        UpdateDoomMusic();
    }

    public void showResetConfirm()
    {
        resetPanel.SetActive(true);
    }

    public void HideResetConfirm()
    {
        resetPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        ResetProgress();
        HideResetConfirm();
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

    private void CkeckAutoClickUnlock()
    {
        bool wasActive = _isAutoClickActive;
        _isAutoClickActive = PlayerPrefs.GetInt("isAutoClick", 0) == 1;

        if (!wasActive && _isAutoClickActive)
        {
            StartCoroutine(IdleFarmRoutine());
            Debug.Log("Автоклик включен");
        }
    }

    private void CheckAchievements()
    {
        int currentAchievemntIndex = GetCurrentAchievementIndex();

        if (currentAchievemntIndex > _lastAchievemntIndex)
        {
            _lastAchievemntIndex = currentAchievemntIndex;

            PlayAchievementSound();
            UpdateDoomguySprite();
            UpdateBackGround();
            UpdateDoomMusic();

            CkeckAutoClickUnlock();
        }
    }

    private int GetCurrentAchievementIndex()
    {
        if (totalDemons >= 10000) return 6;
        if (totalDemons >= 5000) return 5;
        if (totalDemons >= 2000) return 4;
        if (totalDemons >= 1000) return 3;
        if (totalDemons >= 500) return 2;
        if (totalDemons >= 100) return 1;
        return 0;
    }

    private int GetDoomguySpriteIndex()
    {
        if (totalDemons >= 10000) return 3;
        if (totalDemons >= 2000) return 2;
        if (totalDemons >= 500) return 1;
        return 0;
    }

    private void PlayAchievementSound()
    {
        if (audioSource != null && achievementSound != null)
        {
            audioSource.PlayOneShot(achievementSound);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource == null) return;

        int doomguyIndex = GetDoomguySpriteIndex();

        if (shootSounds != null && doomguyIndex < shootSounds.Length && shootSounds[doomguyIndex] != null)
        {
            audioSource.PlayOneShot(shootSounds[doomguyIndex], 0.5f);
        }

        _clicksSinceLastDemon++;

        int clicksLeftInWindow = WINDOW_SIZE - _clicksSinceLastDemon;

        int playLeft = MAX_DEMON_PLAYS - _demonPlaysWindow;

        int playsNeeded = MIN_DEMON_PLAYS - _demonPlaysWindow;

        bool shouldPlayDemon = false;

        if (clicksLeftInWindow <= playsNeeded)
        {
            shouldPlayDemon = true;
        }
        else if (playLeft <= 0)
        {
            shouldPlayDemon = false;
        }
        else
        {
            shouldPlayDemon = Random.Range(0, 2) == 0;
        }

        if (shouldPlayDemon)
        {
            _demonPlaysWindow++;

            AudioClip[] demonPool = null;

            if (doomguyIndex == 0 || doomguyIndex == 1)
            {
                demonPool = demonSoundsDoom1;
            }
            else if (doomguyIndex == 2)
            {
                demonPool = demonSounds2016;
            }
            else if (doomguyIndex == 3)
            {
                demonPool = demonSoundsEternal;
            }

            if (demonPool != null && demonPool.Length > 0)
            {
                int randomIndex = Random.Range(0, demonPool.Length);

                if (demonPool[randomIndex] != null)
                {
                    audioSource.PlayOneShot(demonPool[randomIndex], 0.5f);
                }
            }
        }

        if (_clicksSinceLastDemon >= WINDOW_SIZE)
        {
            _clicksSinceLastDemon = 0;
            _demonPlaysWindow = 0;
        }
    }

    private void UpdateDoomguySprite()
    {
        if (doomguyImage == null || doomguySprites == null || doomguySprites.Length == 0)
        {
            return;
        }

        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", -1);

        int index;

        if (selectedCharacter >= 0 && selectedCharacter < doomguySprites.Length)
        {
            index = selectedCharacter;
        }
        else
        {
            index = GetDoomguySpriteIndex();
        }

        if (index < doomguySprites.Length)
        {
            doomguyImage.sprite = doomguySprites[index];
        }
    }

    private void UpdateBackGround()
    {
        if (backgroundImage == null || backgroundSprites == null || backgroundSprites.Length == 0) return;

        int doomguyIndex = GetDoomguySpriteIndex();

        if (doomguyIndex < backgroundSprites.Length)
        {
            backgroundImage.sprite = backgroundSprites[doomguyIndex];
        }
    }

    private void UpdateDoomMusic()
    {
        if (musicSource == null || doomMusic == null || doomMusic.Length == 0)
            return;

        int doomguyIndex = GetDoomguySpriteIndex();

        if (doomguyIndex >= doomMusic.Length) return;

        AudioClip newMusic = doomMusic[doomguyIndex];

        if (musicSource.clip == newMusic && musicSource.isPlaying)
        {
            return;
        }

        if (newMusic != null)
        {
            musicSource.clip = newMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
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

    public void toStore()
    {
        SceneManager.LoadScene(2);
    }
}