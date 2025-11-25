using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life_Manager : MonoBehaviour
{
    [Header("ステータス")]
    public int _MaxHP = 100;
    //現在のHP
    private int _CurrentHP;

    private IDamageable _Damageable;

    [Header("スクリプト")]
    private Player_Manager _P_Manager;
    private Enemy_Manager _E_Manager;
    private IDamageable _Dame;

    void Awake()
    {
        _P_Manager = GetComponent<Player_Manager>();
        _E_Manager = GetComponent<Enemy_Manager>();
        _Dame = GetComponent<IDamageable>();

        //_Damageable = GetComponent<IDamageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //最初は最大HP
        _CurrentHP = _MaxHP;
    }

    //ダメージ減少
    public void TakeDamage(int damage,string attackerName ="Unknown")
    {
        Debug.Log($"[Life] {gameObject.name} took damage: {damage}. HP Before: {_CurrentHP}");

        _CurrentHP -= damage;
        Debug.Log($"[Life] Took {damage} damage from [{attackerName}]");
        Debug.Log($"ダメージ:{damage} 残りHP:{_CurrentHP}");

        // 🟢 Player の UI を更新（Player の時だけ）
        if (CompareTag("Player"))
        {
            PlayerHP_Manager.Instance.UpdatePlayerHP(_CurrentHP, _MaxHP);
        }

        if (_CurrentHP <= 0)
        {
            _CurrentHP = 0;
            _Dame.Die();
        }
    }

    //回復処理
    public void Heal(int amount)
    {
        _CurrentHP = Mathf.Min(_CurrentHP + amount, _MaxHP);
    }

    public int GetCurrent()
    {
        return _CurrentHP;
    }
}