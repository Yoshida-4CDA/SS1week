using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverTimeBonusEffect : MonoBehaviour
{
    [Header("フィーバーボーナス表示のテキスト")]
    [SerializeField] Text feverTimeBonusText = default;

    /// <summary>
    /// タイムボーナスに応じて表示を変更する
    /// </summary>
    /// <param name="timeBonus"></param>
    public void ShowTimeBonus(int timeBonus)
    {
        feverTimeBonusText.text = "+" + timeBonus.ToString();
        Destroy(gameObject, 0.5f);
    }
}
