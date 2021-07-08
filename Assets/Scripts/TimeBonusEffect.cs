using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBonusEffect : MonoBehaviour
{
    [Header("タイムボーナス表示のテキスト")]
    [SerializeField] Text timeBonusText = default;

    /// <summary>
    /// タイムボーナスに応じて表示を変更する
    /// </summary>
    /// <param name="timeBonus"></param>
    public void ShowTimeBonus(int timeBonus)
    {
        timeBonusText.text = "+" + timeBonus.ToString();
        Destroy(gameObject, 0.5f);
    }
}
