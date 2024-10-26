using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instant;
    public Action OnFinishAnimation;
    public Action OnStageFinish;
    [Header("GameStageData")]
    [SerializeField] private PlayerActionStage playerAction;
    [SerializeField] private float playerHealth;
    [SerializeField] private float monsterHealth;
    [SerializeField] private List<GameObject> monsterList;
    [SerializeField] private Vector3 monsterSpawnPoint;
    [SerializeField] private int currentStage;
    [Header("GameObject")]
    [SerializeField] private GameObject monsterObject;
    [Space(10)]
    [Header("Other Component")]
    [SerializeField] private GamePlayUIController uiController;
    [SerializeField] private AnimationController playerAnimation;
    [SerializeField] private AnimationController monsterAnimation;
    public PlayerActionStage PlayerAction => playerAction;
    public float PlayerHealth => playerHealth;
    public float MonsterHealth => monsterHealth;
    public int CurrentStage => currentStage;
    private void Awake()
    {
        if (Instant == null)
        {
            Instant = this;
            SetUp();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void SetUp()
    {
        currentStage = 1;
        ResetHealth();
        uiController.OnSelectCard += OnSelectCard;
        uiController.OnAction += OnAction;
        uiController.OnContinue += OnContinue;
    }

    private void ResetHealth()
    {
        playerHealth = 100f;
        monsterHealth = 100f;
    }

    private void OnContinue()
    {
        ResetHealth();
        playerAnimation.ResetAnimation();
        if (monsterObject != null)
        {
            Destroy(monsterObject);
        }
        currentStage++;
    }

    public int CalculateReward()
    {
        int mainreward = 0;
        if (playerHealth > 0)
        {
            mainreward = 10;
        }
        return Mathf.FloorToInt(mainreward * Mathf.Pow(1.5f, currentStage - 1));
    }

    private void OnSelectCard(int _index)
    {
        int startFirst = UnityEngine.Random.Range(0, 2);
        if (_index == startFirst)
        {
            playerAction = PlayerActionStage.Attacker;
        }
        else
        {
            playerAction = PlayerActionStage.Defender;
        }
        uiController.ShowCardText(startFirst);
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        int index = UnityEngine.Random.Range(0, monsterList.Count);
        monsterObject = Instantiate(monsterList[index], monsterSpawnPoint, Quaternion.identity);
        monsterObject.transform.DORotate(new Vector3(0, -80f, 0), 0);
        monsterAnimation = monsterObject.GetComponent<AnimationController>();
    }

    private void OnAction(ActionType action)
    {
        ActionType monsterAction = GetMonsterAction(PlayerAction);
        float damage = CalculateDamage(action, monsterAction);
        bool damageToPlayer = DamageToPlayer(action, monsterAction);
        Debug.Log($"P : {action} M : {monsterAction} damage : {damage}");
        PlayAnimation(damageToPlayer, damage);
    }

    private float CalculateDamage(ActionType pAction, ActionType mAction)
    {
        float value;
        if ((pAction == ActionType.Attack && mAction == ActionType.Defend) || (mAction == ActionType.Attack && pAction == ActionType.Defend))
        {
            value = 10f;
        }
        else if (pAction == ActionType.Attack && mAction == ActionType.Counter || (mAction == ActionType.Attack && pAction == ActionType.Counter))
        {
            value = 25f;
        }
        else
        {
            value = 50f;
        }
        return value;
    }

    private void DealthDamage(float damage, bool self = false)
    {
        if (self)
        {
            playerHealth = Mathf.Clamp(playerHealth - damage, 0f, 999f);
        }
        else
        {
            monsterHealth = Mathf.Clamp(monsterHealth - damage, 0f, 999f);
        }
    }

    private bool DamageToPlayer(ActionType pAction, ActionType mAction)
    {
        if (playerAction == PlayerActionStage.Attacker)
        {
            if (pAction == ActionType.Strike && mAction == ActionType.Counter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (mAction == ActionType.Strike && pAction == ActionType.Counter)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void PlayAnimation(bool playerAttack, float damage)
    {
        Action onAttackComplete = !playerAttack ? (() =>
        {
            DealthDamage(damage, playerAttack);
            if (monsterHealth > 0)
            {
                monsterAnimation.PlayHurtAnimation(() => OnFinishAnimation?.Invoke());

            }
            else
            {
                monsterAnimation.PlayDieAnimation();
                OnStageFinish?.Invoke();
                uiController.UpdateHealthUi();
            }
        })
        : () =>
        {
            DealthDamage(damage, playerAttack);
            if (playerHealth > 0)
            {
                playerAnimation.PlayHurtAnimation(() => OnFinishAnimation?.Invoke());
            }
            else
            {
                playerAnimation.PlayDieAnimation();
                OnStageFinish?.Invoke();
                uiController.UpdateHealthUi();
            }

        };

        if (!playerAttack)
            playerAnimation.PlayAttackAnimation(onAttackComplete);
        else
            monsterAnimation.PlayAttackAnimation(onAttackComplete);
        SwapAction();
    }

    private ActionType GetMonsterAction(PlayerActionStage stage)
    {
        return stage switch
        {
            PlayerActionStage.Attacker => GetRandomActionInRange(ActionType.Defend, ActionType.Counter),
            PlayerActionStage.Defender => GetRandomActionInRange(ActionType.Attack, ActionType.Strike),
            _ => throw new System.ArgumentOutOfRangeException(nameof(stage), $"Unsupported action stage: {stage}")
        };
    }
    private ActionType GetRandomActionInRange(ActionType min, ActionType max)
    {
        int randomValue = UnityEngine.Random.Range((int)min, (int)max + 1);
        return (ActionType)randomValue;
    }

    private void SwapAction()
    {
        if (playerAction == PlayerActionStage.Attacker)
        {
            playerAction = PlayerActionStage.Defender;
        }
        else
        {
            playerAction = PlayerActionStage.Attacker;
        }
    }

    private void OnDisable()
    {
        Instant = null;
    }
}

public enum PlayerActionStage
{
    Attacker,
    Defender
}

public enum ActionType
{
    Attack,
    Strike,
    Defend,
    Counter
}