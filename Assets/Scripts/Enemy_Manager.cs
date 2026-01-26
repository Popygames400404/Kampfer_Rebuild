using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour, IDamageable
{   
    public Transform _Player;                 // プレイヤー参照
    public float _MoveSpeed = 6f;             // 移動速度
    public float _AttackRange = 2f;           // 攻撃可能距離
    public float _DetectionRange = 15f;       // プレイヤー検知距離
    public float _Gravity = -9.8f;            // 重力
    public float _RotationSpeeds = 5f;        // 回転速度

    private CharacterController _Controller;  // 移動制御用
    private Animator _Anim;                   // アニメーター
    private bool isAttacking = false;         // 攻撃中フラグ
    private Vector3 _Velocity;                // 重力用速度ベクトル

    [Header("攻撃エフェクト")]
    public TrailRenderer swordTrail; // 剣のTrail 

    void Start()
    {
        _Controller = GetComponent<CharacterController>();
        _Anim = GetComponent<Animator>();

        if (swordTrail != null)
        swordTrail.enabled = false; // 最初はOFF

        // Playerタグからプレイヤー取得
        if (!_Player)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                _Player = p.transform;
        }
    }

    void Update()
    {
        if (_Controller == null || _Player == null)
            return;

        // 敵とプレイヤーの距離（水平距離のみ）
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = _Player.position;
        Vector3 horizontalDistance = new Vector3(enemyPos.x, 0, enemyPos.z) 
                                   - new Vector3(playerPos.x, 0, playerPos.z);
        float distance = horizontalDistance.magnitude;

        // 重力処理
        if (!_Controller.isGrounded)
        {
            _Velocity.y += _Gravity * Time.deltaTime;
        }
        else
        {
            _Velocity.y = -2f; // 接地時の吸着
        }

        // 行動分岐
        if (distance < _AttackRange)
        {
            AttackBehavior(); // 攻撃
        }
        else if (distance < _DetectionRange)
        {
            MoveTowardPlayer(); // 追跡
        }
        else
        {
            IdleBehavior(); // 待機
        }

        _Controller.Move(_Velocity * Time.deltaTime);
    }

    // プレイヤーへ移動
    void MoveTowardPlayer()
    {
        if (isAttacking) return; // 攻撃中は移動しない

        if (_Anim != null) _Anim.SetBool("isMoving", true);

        Vector3 dir = (_Player.position - transform.position);
        dir.y = 0f;
        dir = dir.normalized;

        // プレイヤー方向に回転
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _RotationSpeeds * Time.deltaTime);

        // 移動
        _Controller.Move(dir * _MoveSpeed * Time.deltaTime);
    }

    // 攻撃開始判定
    void AttackBehavior()
    {
        if (_Anim != null) _Anim.SetBool("isMoving", false);

        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    // 実際の攻撃処理
    IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (_Anim != null)
            _Anim.SetTrigger("Slash"); // 攻撃アニメーション再生
            // ★ 攻撃開始 → エフェクトON
            if (swordTrail != null)
            swordTrail.enabled = true;

        yield return new WaitForSeconds(0.5f); // 攻撃時間

        // ★ 攻撃終了 → エフェクトOFF
        if (swordTrail != null)
        swordTrail.enabled = false;

        yield return new WaitForSeconds(2.0f); // 攻撃クールタイム

        isAttacking = false;
    }

    // 待機
    void IdleBehavior()
    {
        if (_Anim != null) _Anim.SetBool("isMoving", false);
    }

    // 死亡処理
    public void Die()
    {
        this.enabled = false;
        _Anim.SetBool("Dead", true);

        Destroy(gameObject, 3.5f); // 死亡アニメ後に削除
    }

    public void TrailOn()
    {
        if (swordTrail != null)
            swordTrail.enabled = true;
    }

    public void TrailOff()
    {
        if (swordTrail != null)
            swordTrail.enabled = false;
    }

}
