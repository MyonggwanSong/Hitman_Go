using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // panels
    public GameObject mainPanel;
    public GameObject chapterSelectPanel;
    public GameObject settingsPanel;
    //public GameObject resetGamePanel;
    //public GameObject levelSellectPanel;
    
    // Effect
    public GameObject fadeEffect;





    public void OnClickedGo()
    {
        StartCoroutine(ShowOnly(chapterSelectPanel));
    }
    public void OnClickedBack2Main()
    {
         StartCoroutine(ShowOnly(mainPanel));
    }
    public void OnClickedExit()
    {
        StartCoroutine(ShowOnly(settingsPanel));
    }
    public void OnClickedSettings()
    {
        StartCoroutine(ShowOnly(settingsPanel));
    }


    public void OnButtonHoverEnter(Button btn)
    {
        btn.transform.localScale = btn.transform.localScale * 1.2f;
    }
    public void OnButtonHoverExit(Button btn)
    {
        btn.transform.localScale = btn.transform.localScale / 1.2f;
    }


    IEnumerator ShowOnly(GameObject target)
    {
        fadeEffect.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        chapterSelectPanel.SetActive(false);
        //resetGamePanel.SetActive(false);
        //levelSellectPanel.SetActive(false);

        if (target != null)
            target.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        fadeEffect.SetActive(false);

    }



    // public void ChangeScene(string name)
    // {
    //     if (settingsPanel.ToString() == name)
    //         ShowOnly(settingsPanel);

    //     if (chapterSelectPanel.ToString() == name)
    //         ShowOnly(chapterSelectPanel);

    //     if (resetGamePanel.ToString() == name)
    //         ShowOnly(resetGamePanel);

    //     if (levelSellectPanel.ToString() == name)
    //         ShowOnly(levelSellectPanel);

    // }
}
