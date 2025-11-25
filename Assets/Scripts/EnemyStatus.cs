using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    public string enemyName="Night";
    public int maxHP = 100;
    public int currentHP = 100;

    private Enemy_Manager _Enemy;
    private IDamageable _Dame;

    void Awake()
    {
        _Enemy = GetComponent<Enemy_Manager>();
        _Dame = GetComponent<IDamageable>();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"ダメージ:{damage} 残りHP:{currentHP}");

        // UI更新（UI側で null チェック）
        EnemyUI_Manager.Instance?.UpdateEnemyInfo(this);

        if (currentHP <= 0)
        {
            // 死亡処理
            EnemyUI_Manager.Instance?.HideEnemyInfo(this);
            // Destroy(gameObject); // 必要なら
            _Dame.Die();
        }
    }
}
