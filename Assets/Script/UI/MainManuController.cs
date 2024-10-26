using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainManuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text goldText;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonCLick);
        UpdateGoldText();
    }
    private void OnStartButtonCLick()
    {
        SceneManager.Instant.LoadScene(SceneType.GamePlay);
    }

    public void UpdateGoldText()
    {
        goldText.text = $"Gold : {PlayerData.Instant.Gold}";
    }
}
