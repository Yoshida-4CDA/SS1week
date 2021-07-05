using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("ballを差別化するためのID")]
    public int id;

    [Header("爆破エフェクトのprefab")]
    [SerializeField] GameObject explosionPrefab = default;

    /// <summary>
    /// 爆破エフェクトの生成と破壊
    /// </summary>
    public void Explosion()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(explosion, ParamsSO.Entity.destroyEffectTime);
    }

    /// <summary>
    /// bombがどうかを判別する
    /// </summary>
    /// <returns>idが-1ならtrue,それ以外ならfalse</returns>
    public bool IsBomb()
    {
        return id == -1;
    }
}
