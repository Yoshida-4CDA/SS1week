using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    [Header("生成するBallPrefab")]
    [SerializeField] GameObject ballPrefab = default;

    [Header("設定する画像")]
    [SerializeField] Sprite[] ballSprites = default;

    [Header("bombの画像")]
    [SerializeField] Sprite bombSprite = default;

    /// <summary>
    /// ballを生成する
    /// </summary>
    /// <param name="count">生成する個数</param>
    /// <returns></returns>
    public IEnumerator Spawns(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-0.2f, 0.2f), 8f);
            GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity);

            int ballID = Random.Range(0, ballSprites.Length);   // ballID => 0~4 : bomb => -1

            // もしbombなら ballID = -1
            if (Random.Range(0, 100) < ParamsSO.Entity.bombRate)    // bombRateの割合でtrue => bombを生成
            {
                ballID = -1;
                ball.GetComponent<SpriteRenderer>().sprite = bombSprite;
                ball.transform.localScale = Vector3.one * ParamsSO.Entity.bombScale;
            }
            // それ以外なら ballの画像をランダムに設定する
            else
            {
                ball.GetComponent<SpriteRenderer>().sprite = ballSprites[ballID];
            }

            // ballの差別化をする
            ball.GetComponent<Ball>().id = ballID;
            
            yield return new WaitForSeconds(0.04f);
        }
    }
}
