using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instant;
    [SerializeField] private int gold;
    public int Gold => gold;
    private void Awake()
    {
        if (Instant == null)
        {
            Instant = this;
            gold = 0;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGold(int _data)
    {
        gold += _data;
    }
}
