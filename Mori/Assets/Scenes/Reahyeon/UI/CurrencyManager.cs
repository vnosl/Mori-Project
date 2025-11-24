using UnityEngine;
using TMPro;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Start Values")]
    public int startMoney = 100000;
    public int startSouls = 100000;

    [Header("UI (optional)")]
    public TMP_Text moneyText;   // 상단 $ 텍스트
    public TMP_Text soulsText;   // 상단 해골 텍스트

    public int Money { get; private set; }
    public int Souls { get; private set; }

    public event Action OnCurrencyChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Money = startMoney;
        Souls = startSouls;
        RefreshUI();
    }

    public bool TryBuy(int price, int soulGain, out string message)
    {
        if (price <= 0) price = 0;
        if (Money < price)
        {
            message = "돈이 부족합니다";
            return false;
        }
        Money -= price;
        Souls += soulGain;
        RefreshUI();
        message = "구매 완료";
        return true;
    }

    public void RefreshUI()
    {
        if (moneyText) moneyText.text = $"{Money:n0}";
        if (soulsText) soulsText.text = $"{Souls:n0}";
        OnCurrencyChanged?.Invoke();
    }
}
