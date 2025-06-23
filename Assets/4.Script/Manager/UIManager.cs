using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CustomInspector;

public class UIManager : BehaviourSingleton<UIManager>
{
    protected override bool IsDontdestroy() => true;

    // panels
    [Title("panels")]

    public GameObject mainPanel;
    public GameObject chapterSelectPanel;
    public GameObject settingsPanel;
    public GameObject quitPanel;
    //public GameObject resetGamePanel;

    // Effect
    [Title("Effect")]
    public GameObject fadeEffect;

    [Title("Setting")]
    public Text soundText;
    public Text musicText;

    void Start()
    {
        SetUI();

    }

    void SetUI()
    {
         StartCoroutine(FadeFeedback());
        SFXOn(true);
        BGMOn(true);
    }


    #region button Click Event

    public void OnClickedGo()
    {
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


    public void OnClickedQuitGame()
    {
        Application.Quit();

    }
    #endregion



    public void OnButtonHoverEnter(Button btn)
    {
        btn.transform.localScale = btn.transform.localScale * 1.2f;
    }
    public void OnButtonHoverExit(Button btn)
    {
        btn.transform.localScale = btn.transform.localScale / 1.2f;
    }


    IEnumerator ShowOnly(GameObject target) // fade Effect Coroutine
    {

        StartCoroutine(FadeFeedback());

        yield return new WaitForSeconds(0.5f);
        mainPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        chapterSelectPanel?.SetActive(false);
        quitPanel?.SetActive(false);
        //resetGamePanel.SetActive(false);

        if (target != null)
            target?.SetActive(true);

      

    }
    IEnumerator FadeFeedback()
    {
         fadeEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
         fadeEffect.SetActive(false);
    }


   
    bool _s = true;

    public void SFXOn(bool on)
    {
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



}
