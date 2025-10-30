using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public float _Speed = 5f;         // 移動スピード
    public float _Gravity = -9.8f;    // 重力の強さ

    private CharacterController _Controller;
    private Vector3 _Velocity;        // 現在の速度
    private bool _IsGrounded;         // 接地判定
    private Transform _Cam;

    [Header("回転")]
    private float _TurnSmoothVelocity;
    public float _TurnSmoothTime = 0.1f; // 回転の滑らかさ

    // Start is called before the first frame update
    void Start()
    {
        _Controller = GetComponent<CharacterController>();
        _Controller.Move(Vector3.zero);// 初期化で軽く接地させる
        _Cam = Camera.main.transform;
    }

    void Update()
    {
        _IsGrounded = _Controller.isGrounded;

        //WASD入力を取得　※floatを付けると上で宣言した_Horizontalとは別物となってしまう為、
        //下のFixed Updateでの_Horizontalには整数が入らず0のままとなってしまう。
        float _Horizontal = Input.GetAxis("Horizontal");
        float _Vertical = Input.GetAxis("Vertical");

        //カメラ
        //カメラの向きを基準に移動方向を作る
        Vector3 camForward = _Cam.forward;
        Vector3 camRight = _Cam.right;

        // 上下方向への移動をカット（地面に平行な移動）
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();//ベクトルの長さを1に揃えます（正規化:計算結果を安定させるための処理です。）
        camRight.Normalize();

        // カメラ基準での移動方向を決定
        Vector3 move = (camRight * _Horizontal + camForward * _Vertical);

        ////move(移動方向)*_Speed(移動速度)*Time.fixedDeltaTime(フレームごとの補正《１秒当たりに直す》)
        //_Controller.Move(move * _Speed * Time.deltaTime);

        Vector3 totalMove = move * _Speed + _Velocity;
        _Controller.Move(totalMove * Time.deltaTime);

        // 簡易的な重力処理
        if (!_IsGrounded)
        {
            _Velocity.y += _Gravity * Time.deltaTime;
        }
        else
        {
            _Velocity.y = -2f;
        }

        //// 🔹 縦方向の移動
        //_Controller.Move(_Velocity * Time.deltaTime);

        // Playerの向きを移動方向に合わせる
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _TurnSmoothTime);
        }
    }
}