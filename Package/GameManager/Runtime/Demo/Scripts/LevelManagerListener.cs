using System;
using Joyixir.GameManager.Scripts.Level;
using TMPro;
using UnityEngine;

public class LevelManagerListener : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    protected void OnEnable()
    {
        LevelManager.OnLevelReady += ShowStart;
        LevelManager.OnLevelStart += ShowInGame;
        LevelManager.OnLevelFinish += ShowFinish;
    }

    private void ShowFinish(LevelData data)
    {
        text.SetText(data.ToString());
        Debug.Log(text.text);
    }

    private void ShowInGame()
    {
        text.SetText("InGame");
        Debug.Log(text.text);
    }

    private void ShowStart()
    {
        text.SetText("Ready");
        Debug.Log(text.text);
    }

    protected void OnDisable()
    {
        LevelManager.OnLevelReady -= ShowStart;
        LevelManager.OnLevelStart -= ShowInGame;
        LevelManager.OnLevelFinish -= ShowFinish;
    }
}
