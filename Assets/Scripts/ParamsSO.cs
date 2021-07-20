using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParamsSO : ScriptableObject
{
    [Header("初期のballの個数")]
    public int initBallCount;

    [Header("ゲーム開始前のカウントダウン秒数")]
    public int countdownTime;

    [Header("制限時間")]
    public float gameTime;

    [Header("フィーバーゲージの最大値")]
    public float feverMaxValue;

    // [Header("フィーバーゲージの減少値")]
    // public float updateFeverValue;

    [Header("スコアの基礎点")]
    public int scorePoint;

    [Header("bombによるスコアの基礎点")]
    public int bombScorePoint;

    [Header("comboScoreのスコア倍率")]
    public int comboScorePoint;

    [Header("フィーバータイム中のスコア倍率")]
    public int feverScorePoint;

    [Header("フィーバータイム突入時のタイムボーナス")]
    public int feverTimeBonus;

    [Header("ball間の判定距離")]
    public float ballDistance;

    [Header("ballの拡大率")]
    public float ballScale;

    [Header("ballを削除できる最低個数")]
    public int removeBallCount;

    [Header("comboScoreの発生に必要な最低個数")]
    public int comboCount;

    [Header("timeBonusの発生に必要な最低個数")]
    public int timeBonusCount;

    [Header("timeBonusの基礎値")]
    public int timeBonus;

    [Header("爆破エフェクトが消えるまでの時間")]
    public float destroyEffectTime;

    [Header("bombが生成される確率")]
    public int bombRate;

    [Header("フィーバータイム中のbomb生成率の倍率")]
    public int feverBombRate;

    [Header("bombの拡大率")]
    public float bombScale;

    [Header("bombの範囲")]
    [Range(1,10)]
    public float bombRange;

    // ParamsSOが保存してある場所のパス
    public const string PATH = "ParamsSO";

    // ParamsSOの実体
    private static ParamsSO _entity;
    public static ParamsSO Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<ParamsSO>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }
            return _entity;
        }
    }
}
