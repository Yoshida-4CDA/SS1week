using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("タイトルテキスト")]
    [SerializeField] Text titleText = default;

    void Start()
    {
        // タイトルの文字の色を変更
        titleText.text =
            "<size=180>" +
            "<color=#ff0000>F</color>" +
            "<color=#ffff00>r</color>" +
            "<color=#f09199>u</color>" +
            "<color=#ffa500>i</color>" +
            "<color=#522f60>T</color>" +
            "<color=#e6b422>sum</color>" +
            "</size>";
    }
}
