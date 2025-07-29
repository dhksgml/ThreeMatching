using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;
    private int combo = 0;
    private int comboBonus = 50;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateCombo(combo);
    }

    public void AddScore(int matchCount)
    {
        int baseScore = matchCount * 100;
        int comboScore = combo > 0 ? combo * comboBonus : 0;

        int total = baseScore + comboScore;
        score += total;
        UIManager.Instance.UpdateScore(score);
        IncreaseCombo();
    }

    public void IncreaseCombo()
    {
        combo++;

        if (combo >= 2)
        {
            UIManager.Instance.UpdateCombo(combo);
            UIManager.Instance.ShowComboPopup(combo);
        }
        else
        {
            UIManager.Instance.UpdateCombo(0);
        }
    }

    public void ResetCombo()
    {
        combo = 0;
        UIManager.Instance.UpdateCombo(0);
    }
}
