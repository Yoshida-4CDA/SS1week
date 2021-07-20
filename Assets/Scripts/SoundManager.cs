using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum IndexSE
    {
        Countdown,
        Gamestart,
        Score,
        Feverscore,
        Dragbegin,
        Bomb,
        Gameover
    }

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            // シーンが変わっても破壊されない
        }
        else
        {
            Destroy(this.gameObject);
            // 同じものがあったら破壊する
        }
    }

    [Header("BGMのスピーカー")]
    public AudioSource audioSourceBGM = default;

    [Header("BGMの素材")]
    public AudioClip[] audioClipBGM = default;

    [Header("SEのスピーカー")]
    public AudioSource audioSourceSE = default;

    [Header("SEの素材")]
    public AudioClip[] audioClipSE = default;

    /// <summary>
    /// BGMを流す
    /// </summary>
    public void PlayBGM(int index)
    {
        StopBGM();
        audioSourceBGM.clip = audioClipBGM[index];
        audioSourceBGM.Play();
    }

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    /// <summary>
    /// SEを一度だけ鳴らす
    /// </summary>
    public void PlaySE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipSE[index]);
    }
}
