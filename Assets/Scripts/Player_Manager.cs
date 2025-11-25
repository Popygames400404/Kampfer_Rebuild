using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour,IDamageable
{
    [Header("移動・重力")]
    public float _Speed = 6f;         // 移動スピード
    public float _Gravity = -9.8f;    // 重力の強さ


    [Header("ロックオン設定")]
    public float _LockOnRange = 15f;
    public LayerMask _EnemyLayer;
    public Transform _LockOnTarget;
    public Camera_Manager _Camera;
    public Transform _Cam;


    private Life_Manager _L_Manager;

    private CharacterController _Controller;
    private Vector3 _Velocity;        // 現在の速度
    private bool _IsGrounded;         // 接地判定
    private Animator _Anim;

    [Header("回転")]
    private float _TurnSmoothVelocity;
    public float _TurnSmoothTime = 0.1f; // 回転の滑らかさ

    [Header("ガード設定")]
    public float _GuardSlowTimeScale = 0.5f;  // ガード中スローモーション倍率
    public float _NormalTimeScale = 1.0f;

    //[Header("攻撃処理")]
    //private int _ComboStep = 0;
    //private float _LastAttackTime;
    //public float _ComboresetTime = 1f;

    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _L_Manager = GetComponent<Life_Manager>();
        if (_L_Manager == null)
        {
            Debug.Log("NotFound");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!_L_Manager)
        {
            Debug.Log("_Life_Managerが見つかりません");
            return;
        }

        _Controller = GetComponent<CharacterController>();
        _Controller.Move(Vector3.zero);// 初期化で軽く接地させる
        _Cam = Camera.main.transform;

        if (_Anim)
        {
            _Anim.Play("Idle1");
        }
    }

    void Update()
    {
        _IsGrounded = _Controller.isGrounded;

        //---ロックオン切り替え---
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (_LockOnTarget == null)
            {
                FindLockOnTarget();
            }
            else
            {
                ClearLockOn();
            }
        }

        //WASD入力を取得　※floatを付けると上で宣言した_Horizontalとは別物となってしまう為、
        //下のFixed Updateでの_Horizontalには整数が入らず0のままとなってしまう。
        float _Horizontal = Input.GetAxis("Horizontal");
        float _Vertical = Input.GetAxis("Vertical");

        //---カメラの向きを基準に移動方向を作る---
        Vector3 camForward = _Cam.forward;
        Vector3 camRight = _Cam.right;
        // 上下方向への移動をカット（地面に平行な移動）
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();//ベクトルの長さを1に揃えます（正規化:計算結果を安定させるための処理です。）
        camRight.Normalize();
        // カメラ基準での移動方向を決定
        Vector3 move = (camRight * _Horizontal + camForward * _Vertical);

        Vector3 totalMove = move * _Speed + _Velocity;
        _Controller.Move(totalMove * Time.deltaTime);

        //---簡易的な重力処理---
        if (!_IsGrounded)
        {
            _Velocity.y += _Gravity * Time.deltaTime;
        }
        else
        {
            _Velocity.y = -2f;
        }

        //---回転処理---
        if(_LockOnTarget!=null)
        {
            //ロックオン中は敵を向く
            Vector3 dir = _LockOnTarget.position - transform.position;
            dir.y = 0;
            Quaternion LookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation,LookRot,_TurnSmoothTime);
        }
        else if(move.sqrMagnitude>0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _TurnSmoothTime);
        }

       //---アニメーション速度設定
        float speedPercent = move.magnitude; // 0～1の範囲に正規化してもOK
        _Anim.SetFloat("Speed", speedPercent, 0.01f, Time.deltaTime);
    }

    void FindLockOnTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + Vector3.up * 1.0f, _LockOnRange, _EnemyLayer);
        Debug.Log($"[LockOn] 検出数: {enemies.Length}");

        if (enemies.Length == 0)
        {
            _LockOnTarget = null; // 敵がいないならロックオン解除
            return;
        }

        //
        Transform nearest = enemies[0].transform;
        float minAngle = Vector3.Angle(_Cam.forward, nearest.position - _Cam.position);//カメラの方向と敵の位置の角度を測定

        foreach(var e in enemies)
        {
            float angle = Vector3.Angle(_Cam.forward, e.transform.position - _Cam.position);
            if (angle < minAngle)//minAngleより小さい角度にいる敵をnearestに更新
            {
                minAngle = angle;
                nearest = e.transform;
            }
        }

        _LockOnTarget = nearest;//ロックオン確定
        _Camera.SetLockOnTarget(_LockOnTarget);//Camera_Managerにお知らせ
        Debug.Log($"[LockOn] {_LockOnTarget.name} にロックオンしました。");

        EnemyStatus enemyStatus = _LockOnTarget.GetComponent<EnemyStatus>();
        if (enemyStatus != null)
        {
            EnemyUI_Manager.Instance.ShowEnemyInfo(enemyStatus); // 🟢ここが追加
        }

    }

    void ClearLockOn()
    {
        if (_LockOnTarget != null)
        {
            EnemyUI_Manager.Instance.HideEnemyInfo();
            // 🟡 ロックオン解除で UI を非表示

            _LockOnTarget = null;
            _Camera.SetLockOnTarget(null);
            // 🟠 カメラ追従解除
        }

    }

    public void Die()
    {
        _Anim.SetBool("IsDead", true);
    }
}