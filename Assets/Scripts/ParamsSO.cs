using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParamsSO : ScriptableObject
{
    [Header("初期のballの個数")]
    public int initBallCount;

    [Header("スコアの基礎点")]
    public int scorePoint;

    [Header("bombによるスコアの基礎点")]
    public int bombScorePoint;

    [Header("comboScoreのスコア倍率")]
    public int comboScorePoint;

    [Header("ball間の判定距離")]
    public float ballDistance;

    [Header("ballの拡大率")]
    public float ballScale;

    [Header("ballを削除できる最低個数")]
    public int removeBallCount;

    [Header("comboScoreの発生に必要な最低個数")]
    public int comboCount;

    [Header("爆破エフェクトが消えるまでの時間")]
    public float destroyEffectTime;

    [Header("bombが生成される確率")]
    public int bombRate;

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
