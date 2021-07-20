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

    [Header("タイムボーナスのエフェクト")]
    [SerializeField] GameObject timeBonusEffectPrefab = default;

    [Header("フィーバータイムボーナスのエフェクト")]
    [SerializeField] GameObject feverTimeBonusEffectPrefab = default;

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

    [Header("フィーバーゲージ")]
    [SerializeField] Slider feverGauge = default;

    [Header("フィーバーゲージのテキスト")]
    [SerializeField] Text feverText = default;

    [Header("サウンドボタン")]
    [SerializeField] GameObject soundButton = default;

    [Header("音量調節のメニュー画面")]
    [SerializeField] GameObject soundSettingImage = default;

    [Header("音量調節バー")]
    [SerializeField] Slider bgmSlider, seSlider = default;

    bool isDragging;            // ドラッグ中かどうかを判別する変数
    Ball currentDraggingBall;   // 現在ドラッグしているオブジェクトを判別する変数
    int score;                  // スコア
    bool isCountdown;           // カウントダウン中かどうかを判別する変数
    float gameTime;             // 制限時間
    bool isGameOver;            // ゲームが終了したかどうかを判別する変数
    float minValue;             // フィーバーゲージの最小値
    float maxValue;             // フィーバーゲージの最大値
    float feverValue;           // フィーバーゲージの現在の値
    bool isFever;               // フィーバータイム中かどうかを判別する変数
    int feverPoint = 1;         // フィーバータイム中のスコア倍率
    bool isSetting;             // 音量調節画面を開いているかどうかを判別する変数

    // コルーチンを変数に代入 => コルーチンの処理を途中で停止させるため
    IEnumerator countDown;          
    IEnumerator updateFeverValue;

    [HideInInspector]
    public int feverBombRate = 1;   // フィーバータイム中にbombが生成される確率

    void Start()
    {
        // BGMとSEの初期化
        PointEffect.indexSE = (int)SoundManager.IndexSE.Score;

        // スコアの初期化
        score = 0;
        AddScore(score);

        // 制限時間の初期化
        gameTime = ParamsSO.Entity.gameTime;
        timerText.text = gameTime.ToString("f0");

        // ballを生成
        StartCoroutine(ballGenerator.Spawns(ParamsSO.Entity.initBallCount));

        // カウントダウン開始
        countDown = CountDown();
        StartCoroutine(countDown);

        // ゲームオーバー画面の初期化
        isGameOver = false;
        gameOverImage.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        // フィーバーゲージの初期化
        minValue = feverGauge.minValue;
        maxValue = ParamsSO.Entity.feverMaxValue;
        feverValue = minValue;
        feverGauge.value = feverValue;
        feverGauge.maxValue = maxValue;
        feverText.color = Color.white;

        // 音量調節画面の初期化
        soundSettingImage.SetActive(false);
        
        // 設定したBGM、SEの音量を各Sliderに反映
        bgmSlider.onValueChanged.AddListener(value => SoundManager.instance.audioSourceBGM.volume = value);
        seSlider.onValueChanged.AddListener(value => SoundManager.instance.audioSourceSE.volume = value);
        bgmSlider.value = SoundManager.instance.audioSourceBGM.volume;
        seSlider.value = SoundManager.instance.audioSourceSE.volume;
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

    /// <summary>
    /// ボーナスタイムの加算と表示
    /// </summary>
    /// <param name="timeBonus"></param>
    void AddTimeBonus(int timeBonus)
    {
        gameTime += timeBonus;
        timerText.text = gameTime.ToString("f0");
    }

    /// <summary>
    /// フィーバーゲージの加算と表示
    /// </summary>
    /// <param name="value"></param>
    void AddFeverValue(float value)
    {
        if (isFever)
        {
            return;
        }

        feverValue += value;
        feverGauge.value = feverValue;

        // ゲージが満タンになったらフィーバータイムへ移行
        if (feverValue >= maxValue)
        {
            FeverMode();
            updateFeverValue = UpdateFeverValue();
            StartCoroutine(updateFeverValue);
        }
    }

    /// <summary>
    /// フィーバーゲージを自動的に減らす
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateFeverValue()
    {
        yield return new WaitForSeconds(1f);
        while (feverValue > 0)
        {
            // feverValue -= ParamsSO.Entity.updateFeverValue;
            feverValue--;
            feverGauge.value = feverValue;
            yield return new WaitForSeconds(0.15f);
        }
        // ゲージが空になったら各設定をデフォルトに戻す
        ReturnDefaultGameMode();
    }

    /// <summary>
    /// フィーバータイム中の設定
    /// </summary>
    void FeverMode()
    {
        if (isGameOver)
        {
            return;
        }

        SoundManager.instance.PlayBGM(2);
        PointEffect.indexSE = (int)SoundManager.IndexSE.Feverscore;
        AddTimeBonus(ParamsSO.Entity.feverTimeBonus);
        SpawnFeverTimeBonusEffect(ParamsSO.Entity.feverTimeBonus);
        feverValue = maxValue;
        feverGauge.value = feverValue;
        feverText.color = Color.magenta;
        feverPoint = ParamsSO.Entity.feverScorePoint;
        feverBombRate = ParamsSO.Entity.feverBombRate;
        isFever = true;
    }

    /// <summary>
    /// フィーバータイム外の設定
    /// </summary>
    void ReturnDefaultGameMode()
    {
        if (isGameOver)
        {
            return;
        }

        SoundManager.instance.PlayBGM(1);
        PointEffect.indexSE = (int)SoundManager.IndexSE.Score;
        feverValue = minValue;
        feverGauge.value = feverValue;
        feverText.color = Color.white;
        feverPoint = 1;
        feverBombRate = 1;
        isFever = false;
    }

    void Update()
    {
        if (isCountdown || isGameOver || isSetting)
        {
            return;
        }

        gameTime -= Time.deltaTime;
        timerText.text = gameTime.ToString("f0");
        
        if (gameTime <= 0)
        {
            gameTime = 0;
            timerText.text = gameTime.ToString("f0");
            StartCoroutine(GameOver());
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
            int scorePoint = (ParamsSO.Entity.scorePoint + comboScore) * feverPoint;
            AddScore(scorePoint);
            AddFeverValue(removeCount);
            SpawnPointEffect(removeBalls[removeBalls.Count - 1].transform.position, scorePoint);

            // removeCountが7個以上だったらtimeBonusを加算
            if (removeCount >= ParamsSO.Entity.timeBonusCount)
            {
                int timeBonus = removeCount - ParamsSO.Entity.timeBonusCount + ParamsSO.Entity.timeBonus;
                SpawnTimeBonusEffect(timeBonus);
                AddTimeBonus(timeBonus);
            }
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
            SoundManager.instance.PlaySE((int)SoundManager.IndexSE.Dragbegin);
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
        // Bomb用のSEを設定する
        int currentIndexSE = PointEffect.indexSE;
        PointEffect.indexSE = (int)SoundManager.IndexSE.Bomb;

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
        int scorePoint = ParamsSO.Entity.scorePoint + comboScore * feverPoint;
        AddScore(scorePoint);
        AddFeverValue(removeCount);
        SpawnPointEffect(bomb.transform.position, scorePoint);

        // タイムボーナス追加
        int timeBonus = ParamsSO.Entity.timeBonus;
        SpawnTimeBonusEffect(timeBonus);
        AddTimeBonus(timeBonus);

        // SEを元に戻す
        PointEffect.indexSE = currentIndexSE;
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
    /// タイムボーナスエフェクトを発生させる
    /// </summary>
    /// <param name="time">表示するタイムボーナス</param>
    void SpawnTimeBonusEffect(int time)
    {
        GameObject effectObj = Instantiate(timeBonusEffectPrefab);
        TimeBonusEffect timeBonusEffect = effectObj.GetComponent<TimeBonusEffect>();
        timeBonusEffect.ShowTimeBonus(time);

    }

    /// <summary>
    /// フィーバータイムボーナスエフェクトを発生させる
    /// </summary>
    /// <param name="feverTime"></param>
    void SpawnFeverTimeBonusEffect(int feverTime)
    {
        GameObject effectObj = Instantiate(feverTimeBonusEffectPrefab);
        FeverTimeBonusEffect feverTimeBonusEffect = effectObj.GetComponent<FeverTimeBonusEffect>();
        feverTimeBonusEffect.ShowTimeBonus(feverTime);
    }

    /// <summary>
    /// ゲーム開始前のカウントダウン
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {
        SoundManager.instance.StopBGM();

        // カウントダウン開始
        isCountdown = true;
        countdownImage.gameObject.SetActive(true);
        countdownText.text = "";
        yield return new WaitForSeconds(0.5f);

        for (int i = ParamsSO.Entity.countdownTime; i > 0; i--)
        {
            countdownText.color = Color.red;
            countdownText.text = $"{i}";
            SoundManager.instance.PlaySE((int)SoundManager.IndexSE.Countdown);
            yield return new WaitForSeconds(1f);
        }
        countdownText.color = Color.green;
        countdownText.text = "GO!";
        SoundManager.instance.PlaySE((int)SoundManager.IndexSE.Gamestart);
        yield return new WaitForSeconds(1f);

        // カウントダウン終了
        countdownImage.gameObject.SetActive(false);
        isCountdown = false;
        SoundManager.instance.PlayBGM(1);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOver()
    {
        isGameOver = true;
        soundButton.SetActive(false);
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE((int)SoundManager.IndexSE.Gameover);
        
        yield return new WaitForSeconds(1.5f);

        // ゲームオーバー画面の表示
        gameOverImage.gameObject.SetActive(true);
        SoundManager.instance.PlayBGM(3);
        yield return new WaitForSeconds(2f);

        // ランキングボードの表示
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score);
        yield return new WaitForSeconds(0.5f);

        // リトライボタンの表示
        restartButton.gameObject.SetActive(true);
    }

    public void OnClickSoundButton()
    {
        if (isGameOver)
        {
            return;
        }

        isSetting = true;
        soundSettingImage.SetActive(true);

        if (isCountdown)
        {
            StopCoroutine(countDown);
        }
        else if (isFever)
        {
            StopCoroutine(updateFeverValue);
        }
    }

    public void OnClickExitButton()
    {
        if (isGameOver)
        {
            return;
        }

        isSetting = false;
        soundSettingImage.SetActive(false);

        if (isCountdown)
        {
            StartCoroutine(countDown);
        }
        else if (isFever)
        {
            StartCoroutine(updateFeverValue);
        }
    }
}
