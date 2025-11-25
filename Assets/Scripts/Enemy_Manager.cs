using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Manager : MonoBehaviour,IDamageable
{
    public Transform _Player;//攻撃対象
    public float _MoveSpeed = 6f;//移動速度
    public float _AttackRange = 2f;//攻撃できる距離
    public float _DetectionRange = 15f;//プレイヤーを見つける範囲
    //public float _RetreatDistance=1.5f;//
    public float _Gravity = -9.8f;//重力
    public float _RotationSpeeds = 5f;//向きを変える速さ

    private CharacterController _Controller;//物理移動用
    private Animator _Anim;
    private bool isAttacking = false;//攻撃中かどうか
    private Vector3 _Velocity;//重力の為のY方向ベクトル

    void Awake()
    {
        //_Controller = GetComponent<CharacterController>();
        //_Anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("[Enemy] Start()");
        _Controller = GetComponent<CharacterController>();
        _Anim = GetComponent<Animator>();

        //Debug.Log($"[Enemy] Controller {(_Controller == null ? "NULL" : "OK")}, Animator {(_Anim == null ? "NULL" : "OK")}");

        if (!_Player)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                _Player = p.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // デバッグ用の早期ログ（必ず出るはず）
        //Debug.Log("[Enemy] Update() enter");

        if (_Controller == null)
        {
            //Debug.LogError("[Enemy] CharacterController が null です。オブジェクトに付けてください。");
            return;
        }

        if (!_Player)//ターゲットがいなければ繰り返す（エラー対策）
        {
            //Debug.LogWarning("[Enemy] _Player が null。Start()で取得できていない可能性があります。");
            return;
        }

        //Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
        //Vector3 playerPos = new Vector3(_Player.position.x, 0, _Player.position.z);
        //float distance = Vector3.Distance(enemyPos, playerPos);

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = _Player.position;
        //distance距離を測って、どう行動するかを決める。
        //float distance = Vector3.Distance(new Vector3(enemyPos.x, 0, enemyPos.z), new Vector3(playerPos.x, 0, playerPos.z));
        Vector3 horizontalDistance = new Vector3(enemyPos.x, 0, enemyPos.z) - new Vector3(playerPos.x, 0, playerPos.z);
        float distance = horizontalDistance.magnitude;

        //重力処理
        if (!_Controller.isGrounded)
        {
            _Velocity.y += -1f;
        }
        else
        {
            _Velocity.y += _Gravity * Time.deltaTime;
        }
        //_Controller.Move(_Velocity * Time.deltaTime);

        //行動選択
        if (distance < _AttackRange)
        {
            //攻撃可能距離
            AttackBehavior();
            //Debug.Log("[Attack");
        }
        else if (distance < _DetectionRange)
        {
            //Debug.Log("MoveTowardPlayer (should move)");
            MoveTowardPlayer();
            //Debug.Log("Move");
        }
        else
        {
            //Debug.Log("Idle");
            IdleBehavior();
            //Debug.Log("None");
        }
        _Controller.Move(_Velocity * Time.deltaTime);
    }

    void MoveTowardPlayer()
    {
        //Debug.Log("[Enemy] MoveTowardPlayer() called");
        if (isAttacking)
        {
            //Debug.Log("[Enemy] Move canceled: isAttacking == true");
            return;
        }
        if (_Anim != null) _Anim.SetBool("isMoving", true);

        Vector3 dir = (_Player.position - transform.position);
        dir.y = 0f;//上下の傾きは無視
        dir = dir.normalized;

        ////Debug.Log($"[Enemy] dir = {dir}");

        //
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, _RotationSpeeds * Time.deltaTime);

        //水平移動と重力を1回のMoveにまとめる
        Vector3 move = dir * _MoveSpeed;
        move.y = _Velocity.y;

        //Debug.Log($"[Enemy] Move delta (before Move) = {move * Time.deltaTime}");

        _Controller.Move(dir * _MoveSpeed * Time.deltaTime);
        //Debug.Log("[Enemy] _Controller.Move() called");
    }

    void AttackBehavior()
    {
        //Debug.Log("[Enemy] AttackBehavior()");
        if (_Anim != null) _Anim.SetBool("isMoving", false);
        if (!isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        //Debug.Log("[Enemy] PerformAttack start");
        isAttacking = true;
        if (_Anim != null) _Anim.SetTrigger("Slash");
        yield return new WaitForSeconds(2.0f);
        isAttacking = false;
        //Debug.Log("[Enemy] PerformAttack end");

        ////攻撃判定
        //Ray ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);
        //// 可視化（Sceneビューに赤線を描画）
        //Debug.DrawRay(ray.origin, ray.direction * 2.0f, Color.red, 1.0f);

        //if (Physics.Raycast(ray, out RaycastHit hit, 2.0f))
        //{
        //    Life_Manager enemyLife = hit.collider.GetComponent<Life_Manager>();
        //    if (enemyLife)
        //    {
        //        enemyLife.TakeDamage(50);
        //    }
        //}
    }

    void IdleBehavior()
    {
        if (_Anim != null) _Anim.SetBool("isMoving", false);
    }

    public void Die()
    {
        this.enabled = false;
        _Anim.SetBool("Dead", true);


        //死亡アニメーションが終わったら削除
        // 秒数は死亡アニメーションの長さに合わせる
        Destroy(gameObject, 3.5f);
    }
}

