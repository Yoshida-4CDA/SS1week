using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointEffect : MonoBehaviour
{
    [Header("スコアテキスト")]
    [SerializeField] Text scoreText = default;

    [HideInInspector]
    public static int indexSE;   // ならすSEの番号

    /// <summary>
    /// スコアに応じて表示を変更する
    /// </summary>
    /// <param name="score"></param>
    public void Show(int score)
    {
        scoreText.text = "+" + score.ToString();
        SoundManager.instance.PlaySE(indexSE);
        StartCoroutine(MoveUp());
    }

    /// <summary>
    /// 上にあげる
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveUp()
    {
        for (int i = 0; i < 10; i++)
        {
            // yield return new WaitForSeconds(0.01f);
            yield return null;
            transform.Translate(0, 0.15f, 0);
        }
        Destroy(gameObject, 0.2f);
    }
}
