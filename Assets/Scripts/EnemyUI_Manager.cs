using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI_Manager : MonoBehaviour
{
    public static EnemyUI_Manager Instance;

    [Header("UI参照")]
    public GameObject enemyInfoUI;   // EnemyInfoUI（親オブジェクト）
    public TMP_Text enemyNameText;   // TextMeshPro テキスト
    public Slider enemyHPBar;        // Slider (0..1)

    private EnemyStatus currentTarget;//現在ロックオン中の敵

    private void Awake()
    {
        if (Instance == null) Instance = this;//Instanceに登録
        else Destroy(gameObject);//すでにある場合は削除

        if (enemyInfoUI != null) enemyInfoUI.SetActive(false);//まだ誰もロックオンしていないのでFalse
    }

    public void ShowEnemyInfo(EnemyStatus enemy)
    {
        currentTarget = enemy;//ターゲットとして登録
        if (enemyInfoUI != null) enemyInfoUI.SetActive(true);//UI全体をONに
        if (enemyNameText != null) enemyNameText.text = enemy.enemyName;//敵の名前をTextに
        UpdateEnemyInfo(enemy);//HPも更新するため、UpdateEnemyInfoを呼び出す
    }

    public void UpdateEnemyInfo(EnemyStatus enemy)
    {
        if (enemy != currentTarget) return;//現在表示中の敵と違うなら更新しない
        if (enemyHPBar != null) enemyHPBar.value = (float)enemy.currentHP / enemy.maxHP;//HPバーを現在HP÷最大HPで更新
    }

    public void HideEnemyInfo(EnemyStatus enemy = null)
    {
        if (enemy != null && enemy != currentTarget) return;//enemyが指定された場合は「今表示している敵と一致したときだけ」消す
        if (enemyInfoUI != null) enemyInfoUI.SetActive(false);　//UI全体を OFF
         currentTarget = null;//ターゲットをクリア
    }
}
