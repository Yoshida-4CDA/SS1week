using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    [Header("BallGenerator")]
    [SerializeField] BallGenerator ballGenerator = default;

    [Header("削除するballを格納するList")]
    [SerializeField] List<Ball> removeBalls = new List<Ball>();

    [Header("スコアを表示するテキスト")]
    [SerializeField] Text scoreText = default;

    [Header("ポイントのエフェクト")]
    [SerializeField] GameObject pointEffectPrefab = default;

    [Header("カウントダウンの背景")]
    [SerializeField] Image countdownImage = default;

    [Header("カウントダウンのテキスト")]
    [SerializeField] Text countdownText = default;

    [Header("制限時間のテキスト")]
    [SerializeField] Text timerText = default;

    [Header("ゲームオーバーの背景")]
    [SerializeField] Image gameOverImage = default;

    [Header("リスタートボタン")]
    [SerializeField] Button restartButton = default;

    bool isDragging;            // ドラッグ中かどうかを判別する変数
    Ball currentDraggingBall;   // 現在ドラッグしているオブジェクトを判別する変数
    int score;                  // スコア
    bool isCountdown;           // カウントダウン中かどうかを判別する変数
    float gameTime;             // 制限時間
    bool isGameOver;            // ゲームが終了したかどうかを判別する変数

    void Start()
    {
        // スコアの初期化
        score = 0;
        AddScore(score);

        // 制限時間の初期化
        gameTime = ParamsSO.Entity.gameTime;
        timerText.text = gameTime.ToString("f0");

        // ballを生成
        StartCoroutine(ballGenerator.Spawns(ParamsSO.Entity.initBallCount));

        // カウントダウン開始
        StartCoroutine(CountDown());

        // ゲームオーバー画面の初期化
        gameOverImage.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// スコアを加算と表示
    /// </summary>
    /// <param name="point"></param>
    void AddScore(int point)
    {
        score += point;
        scoreText.text = score.ToString();
    }

    void Update()
    {
        StopCoroutine(CountDown());

        if (!isCountdown && !isGameOver)
        {
            gameTime -= Time.deltaTime;
            timerText.text = gameTime.ToString("f0");
            if (gameTime <= 0)
            {
                gameTime = 0;
                timerText.text = gameTime.ToString("f0");
                isGameOver = true;
                if (isGameOver)
                {
                    StartCoroutine(GameOver());
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                // 左クリックを押し込んだ時
                OnDragBegin();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // 左クリックを離したとき
                OnDragEnd();
            }
            else if (isDragging)
            {
                OnDragging();
            }
        }
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    void OnDragBegin()
    {
        // メインカメラ上のマウスカーソルのある位置からスクリーンに向かってRayを飛ばす
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // Rayがhitしたオブジェクトがballかどうかを判別する
        if (hit && hit.collider.GetComponent<Ball>())
        {
            Ball ball = hit.collider.GetComponent<Ball>();

            // bombなら周囲を含めて爆破、それ以外なら通常の処理をする
            if (ball.IsBomb())
            {
                // 爆破
                BombExplosion(ball);
            }
            else
            {
                AddRemoveBall(ball);
                isDragging = true;
            }
        }
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    void OnDragging()
    {
        // メインカメラ上のマウスカーソルのある位置からスクリーンに向かってRayを飛ばす
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        // Rayがヒットしたオブジェクトがballかどうかを判別する
        if (hit && hit.collider.GetComponent<Ball>())
        {
            Ball ball = hit.collider.GetComponent<Ball>();

            // IDが同じ => 同じ種類のballかつball間の距離が近かったらListに追加する
            if (ball.id == currentDraggingBall.id)
            {
                float distance = Vector2.Distance(ball.transform.position, currentDraggingBall.transform.position);
                if (distance < ParamsSO.Entity.ballDistance)
                {
                    AddRemoveBall(ball);
                }
            }
        }
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    void OnDragEnd()
    {
        int removeCount = removeBalls.Count;
        int comboScore = 0;

        // removeCountが3個以上だったらballを削除する
        if (removeCount >= ParamsSO.Entity.removeBallCount)
        {
            for (int i = 0; i < removeCount; i++)
            {
                removeBalls[i].Explosion();
            }
            // 消した数だけballを追加する
            StartCoroutine(ballGenerator.Spawns(removeCount));

            // removeCountが4個以上だったらcomboScoreを加算する
            if (removeCount >= ParamsSO.Entity.comboCount)
            {
                // comboScore => 4個消したら4*50=200、5個消したら4*50+5*50=450、6個消したら4*50+5*50+6*50=750、・・・
                for (int i = ParamsSO.Entity.comboCount; i <= removeCount; i++)
                {
                    comboScore += i * ParamsSO.Entity.comboScorePoint;
                }
            }
            // スコア：4個 => 300+200=500、5個 => 300+450=750、6個 => 300+750=1050、・・・
            int scorePoint = ParamsSO.Entity.scorePoint + comboScore;
            AddScore(scorePoint);
            SpawnPointEffect(removeBalls[removeBalls.Count - 1].transform.position, scorePoint);
        }
        // 全てのremoveBallのサイズと色を元に戻す
        for (int i = 0; i < removeCount; i++)
        {
            removeBalls[i].transform.localScale = Vector3.one;
            removeBalls[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        removeBalls.Clear();
        isDragging = false;
    }

    /// <summary>
    /// 削除するballをListに追加する
    /// </summary>
    /// <param name="ball"></param>
    void AddRemoveBall(Ball ball)
    {
        currentDraggingBall = ball;
        if (removeBalls.Contains(ball) == false)
        {
            // ballのサイズを拡大、色を変更する
            ball.transform.localScale = Vector3.one * ParamsSO.Entity.ballScale;
            ball.GetComponent<SpriteRenderer>().color = Color.yellow;
            removeBalls.Add(ball);
        }
    }

    /// <summary>
    /// bombによる爆破
    /// </summary>
    void BombExplosion(Ball bomb)
    {
        // 爆破するballを格納するリスト
        List<Ball> explosionList = new List<Ball>();

        // bombを中心に爆破するBallを集める => 「bombから半径がbombRangeの円」にぶつかったColliderを集めてくる
        Collider2D[] hitObj = Physics2D.OverlapCircleAll(bomb.transform.position, ParamsSO.Entity.bombRange);
        for (int i = 0; i < hitObj.Length; i++)
        {
            // ballだったら爆破リストに追加する
            Ball ball = hitObj[i].GetComponent<Ball>();
            if (ball)
            {
                explosionList.Add(ball);
            }
        }
        // 爆破する
        int removeCount = explosionList.Count;
        int comboScore = 0;

        for (int i = 0; i < removeCount; i++)
        {
            explosionList[i].Explosion();
        }
        // 消した数だけballを追加する
        StartCoroutine(ballGenerator.Spawns(removeCount));

        // removeCountが4個以上だったらcomboScoreを加算する
        if (removeCount >= ParamsSO.Entity.comboCount)
        {
            // comboScore => 4個消したら4*50=200、5個消したら4*50+5*50=450、6個消したら4*50+5*50+6*50=750、・・・
            for (int i = ParamsSO.Entity.comboCount; i <= removeCount; i++)
            {
                comboScore += i * ParamsSO.Entity.comboScorePoint;
            }
        }
        // スコア：4個 => 500+200=700、5個 => 500+450=950、6個 => 500+750=1250、・・・
        int scorePoint = ParamsSO.Entity.bombScorePoint + comboScore;
        AddScore(scorePoint);
        SpawnPointEffect(bomb.transform.position, scorePoint);
    }

    /// <summary>
    /// ポイントエフェクトを発生させる
    /// </summary>
    /// <param name="position">発生させる場所</param>
    /// <param name="score">表示するスコア</param>
    void SpawnPointEffect(Vector2 position, int score)
    {
        GameObject effectObj = Instantiate(pointEffectPrefab, position, Quaternion.identity);
        PointEffect pointEffect = effectObj.GetComponent<PointEffect>();
        pointEffect.Show(score);
    }

    /// <summary>
    /// ゲーム開始前のカウントダウン
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {
        isCountdown = true;
        countdownImage.gameObject.SetActive(true);
        countdownText.text = "";
        yield return new WaitForSeconds(0.5f);

        for (int i = ParamsSO.Entity.countdownTime; i > 0; i--)
        {
            countdownText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownImage.gameObject.SetActive(false);
        isCountdown = false;
    }

    IEnumerator GameOver()
    {
        Debug.Log("ゲーム終了");
        yield return new WaitForSeconds(1f);

        gameOverImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        Debug.Log("ランキング画面表示");
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
        yield return new WaitForSeconds(0.5f);

        Debug.Log("リスタートボタン表示");
        restartButton.gameObject.SetActive(true);
    }
}
