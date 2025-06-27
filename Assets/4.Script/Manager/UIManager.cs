using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CustomInspector;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : BehaviourSingleton<UIManager>
{
    protected override bool IsDontdestroy() => false;

    // panels
    [Title("panels")]

    public GameObject mainPanel;
    public GameObject chapterSelectPanel;
    public GameObject settingsPanel;
    public GameObject chapter1Map;
    public GameObject resetPanel;
    [Space(20)]
    public GameObject quitPanel;

    public GameObject clearPanel;

    //public GameObject resetGamePanel;

    // Effect
    [Title("Effect")]
    public GameObject fadeEffect;

    [Title("Setting")]
    public Text soundText;
    public Text musicText;

    [Title("Ingame UI"), HideField] public bool _b0;

    public List<GameObject> stamps = new List<GameObject>();
    [SerializeField] List<GameObject> noAniStamps = new List<GameObject>();

    [Title("Chapter 1 Levels"), HideField] public bool _b1;
    [SerializeField] List<GameObject> Levels = new List<GameObject>();



    IEnumerator Start()
    {
        // unrelated by GameManager
        StartCoroutine(FadeFeedback());
        SetUI();

        yield return new WaitUntil(() => GameManager.I.isGameStart);

        stamps.RemoveAll(stamp => stamp == null); // 비어있는 List 삭제
        noAniStamps.RemoveAll(noAniStamp => noAniStamp == null);// 비어있는 List 삭제


        for (int i = 0; i < noAniStamps.Count; i++) // 업적에 해당하는 도장 키기
        {
            if (AchievementManager.I.IsLevelAchievment(GameManager.I.currentLevel, i + 1))
                noAniStamps[i].SetActive(true);
            Debug.Log($"Level{GameManager.I.currentLevel} \n Achievments{i + 1} : {AchievementManager.I.IsLevelAchievment(GameManager.I.currentLevel, i + 1)}");
        }


    }

    void SetUI()
    {

        SFXOn(true);
        BGMOn(true);
    }



    #region button Click Event

    public void OnClickedGo()
    {
        if (PlayerPrefs.GetInt($"Level_1_Clear", 0) == 0)   // New User New Game >> Level 1
            OnSceneLoad(1);

        if (chapterSelectPanel != null)
            StartCoroutine(ShowOnly(chapterSelectPanel));
    }
    public void OnClickedBack2Main()
    {
        if (mainPanel != null)
            StartCoroutine(ShowOnly(mainPanel));
    }
    public void OnClickedExit()
    {
        if (quitPanel != null)
            StartCoroutine(ShowOnly(quitPanel));
    }
    public void OnClickedSettings()
    {
        if (settingsPanel != null)
            StartCoroutine(ShowOnly(settingsPanel));
    }
     public void OnClickedReset()
    {
        if (resetPanel != null)
            StartCoroutine(ShowOnly(resetPanel));
    }

    // Chapter 1
    public void OnClickedEnterChpater1(Button btn)
    {
        if (chapter1Map != null)
        {
            btn.transform.localScale = Vector3.one;
            StartCoroutine(ShowOnly(chapter1Map));
        }


        for (int i = 0; i < Levels.Count - 1; i++)
        {
            if (PlayerPrefs.GetInt($"Level_{i + 1}_Clear", 0) == 1)
            {
                Levels[i].SetActive(true);
            }
            else
            {
                Levels[i].SetActive(false);
            }

        }

    }

    //
    public void OnClickedPopup(Button btn)
    {
        if (quitPanel != null)
            if (!quitPanel.activeSelf)
                quitPanel.SetActive(true);
            else
            {
                btn.transform.localScale = btn.transform.localScale / 1.2f;
                quitPanel.SetActive(false);
            }
    }
    public void OnClearPopup()
    {
        if (clearPanel != null)
            if (!clearPanel.activeSelf)
                clearPanel.SetActive(true);
            else
            {
                clearPanel.SetActive(false);
            }
    }

    public void OnClickedQuitGame()
    {
        Application.Quit();

    }
    #endregion



    public void OnButtonHoverEnter(Button btn)  // 마우스 호버시 버튼 크기 증가
    {
        btn.transform.localScale = btn.transform.localScale * 1.2f;
    }
    public void OnButtonHoverExit(Button btn)
    {
        btn.transform.localScale = Vector3.one;
    }


    IEnumerator ShowOnly(GameObject target) // fade Effect Coroutine
    {

        StartCoroutine(FadeFeedback());

        yield return new WaitForSeconds(0.5f);
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (chapterSelectPanel != null) chapterSelectPanel.SetActive(false);
        if (resetPanel != null) resetPanel.SetActive(false);
        if (quitPanel != null) quitPanel.SetActive(false);
        if (chapter1Map != null) chapter1Map.SetActive(false);

        if (target != null)
            target?.SetActive(true);



    }
    IEnumerator FadeFeedback()
    {
        fadeEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        fadeEffect.SetActive(false);
    }



    bool _s = true;

    public void SFXOn(bool on)
    {
        if (soundText == null) return;
        on = !_s;
        _s = on;
        if (on)
        {
            soundText.text = "On";
            // connect Audio Manager

        }
        else
        {
            soundText.text = "Off";
            // connect Audio Manager
        }
    }

    bool _b = true;
    public void BGMOn(bool on)
    {
        if (musicText == null) return;
        on = !_b;
        _b = on;
        if (on)
        {
            musicText.text = "On";
            // connect Audio Manager

        }
        else
        {
            musicText.text = "Off";
            // connect Audio Manager

        }
    }

    public void OnSceneLoad(int i)
    {
        SceneManager.LoadScene(i);
        GameManager.I.SetInitialIngame();
    }
    public void OnSceneReload()
    {
        GameManager.I.SetInitialIngame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        OnClickedBack2Main();
    }
}
