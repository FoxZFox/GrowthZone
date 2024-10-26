using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    public Action<int> OnSelectCard;
    public Action<ActionType> OnAction;

    public Action OnContinue;
    [Header("DrawCard")]
    [SerializeField] private GameObject drawCardPanel;
    [SerializeField] private Button card1Button;
    [SerializeField] private Button card2Button;
    [SerializeField] private TMP_Text card1Text;
    [SerializeField] private TMP_Text card2Text;
    [Space(10), Header("Reward")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text goldRewardText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;

    [Space(10), Header("GamePlayInfo")]
    [SerializeField] private Image[] stageBoxs;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private TMP_Text action1Text;
    [SerializeField] private TMP_Text action2Text;
    [SerializeField] private Button action1Button;
    [SerializeField] private Button action2Button;
    [SerializeField] private Image playerHealth;
    [SerializeField] private Image monsterHealth;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text monsterHealthText;
    [SerializeField] private TMP_Text goldText;


    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        card1Button.onClick.AddListener(() => OnCardClick(0));
        card2Button.onClick.AddListener(() => OnCardClick(1));
        action1Button.onClick.AddListener(() => OnActionClick(0));
        action2Button.onClick.AddListener(() => OnActionClick(1));
        continueButton.onClick.AddListener(() => OnContinueClick());
        backButton.onClick.AddListener(() => OnBackClick());
        GameManager.Instant.OnFinishAnimation += OnFinishAnimation;
        GameManager.Instant.OnStageFinish += OnStageFinish;
        UpdateGoldText();
        SetStageBoxs(GameManager.Instant.CurrentStage - 1);
    }

    private void OnCardClick(int _index)
    {
        OnSelectCard?.Invoke(_index);
    }

    private void OnActionClick(int _index)
    {
        Debug.Log("OnActionClick");
        actionPanel.SetActive(false);
        ActionType action = GameManager.Instant.PlayerAction switch
        {
            PlayerActionStage.Attacker => _index == 0 ? ActionType.Attack : ActionType.Strike,
            PlayerActionStage.Defender => _index == 0 ? ActionType.Defend : ActionType.Counter,
            _ => throw new ArgumentOutOfRangeException()
        };
        OnAction?.Invoke(action);
    }

    private void OnContinueClick()
    {
        PlayerData.Instant.SetGold(GameManager.Instant.CalculateReward());
        OnContinue?.Invoke();
        ResetUI();
        SetStageBoxs(GameManager.Instant.CurrentStage - 1);
    }

    private void SetStageBoxs(int _index)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == _index % 5)
            {
                stageBoxs[i].color = Color.red;
            }
            else
            {
                stageBoxs[i].color = Color.white;
                if (i == 4)
                {
                    stageBoxs[i].color = Color.yellow;
                }
            }
        }
    }

    private void OnBackClick()
    {
        PlayerData.Instant.SetGold(GameManager.Instant.CalculateReward());
        SceneManager.Instant.LoadScene(SceneType.Menu);
    }

    private void OnStageFinish()
    {
        int reward = GameManager.Instant.CalculateReward();
        if (GameManager.Instant.PlayerHealth > 0)
        {
            goldRewardText.text = $"+ {reward} G";
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            goldRewardText.text = $"0 G";
            continueButton.gameObject.SetActive(false);
        }
        rewardPanel.SetActive(true);
    }

    private void OnFinishAnimation()
    {
        Debug.Log("OnFinishAnimation");
        UpdateHealthUi();
        UpdateActionText();
        actionPanel.SetActive(true);
    }

    public void UpdateHealthUi()
    {
        playerHealthText.text = $"{GameManager.Instant.PlayerHealth} / 100";
        monsterHealthText.text = $"{GameManager.Instant.MonsterHealth} / 100";
        playerHealth.fillAmount = GameManager.Instant.PlayerHealth / 100f;
        monsterHealth.fillAmount = GameManager.Instant.MonsterHealth / 100f;
    }

    private void ResetUI()
    {
        UpdateGoldText();
        UpdateHealthUi();
        card1Text.text = "";
        card2Text.text = "";
        card1Button.interactable = true;
        card2Button.interactable = true;
        actionPanel.SetActive(false);
        rewardPanel.SetActive(false);
        drawCardPanel.SetActive(true);
    }

    private void UpdateGoldText()
    {
        goldText.text = $"Gold : {PlayerData.Instant.Gold}";
    }

    public void UpdateActionText()
    {
        switch (GameManager.Instant.PlayerAction)
        {
            case PlayerActionStage.Attacker:
                action1Text.text = "Attack";
                action2Text.text = "Strike";
                break;
            case PlayerActionStage.Defender:
                action1Text.text = "Defend";
                action2Text.text = "Counter";
                break;
        }
    }
    public void ShowCardText(int _startFirst)
    {
        StartCoroutine(ShowCardTextCoroutine(_startFirst));
    }

    private IEnumerator ShowCardTextCoroutine(int _startFirst)
    {
        card1Button.interactable = false;
        card2Button.interactable = false;

        if (_startFirst == 0)
        {
            card1Text.text = "First";
            card2Text.text = "Last";
        }
        else
        {
            card1Text.text = "Last";
            card2Text.text = "First";
        }

        yield return new WaitForSeconds(1.5f);

        drawCardPanel.SetActive(false);
        UpdateActionText();
        actionPanel.SetActive(true);
    }
}
