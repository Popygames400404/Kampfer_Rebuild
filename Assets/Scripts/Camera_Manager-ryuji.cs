using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Manager : MonoBehaviour
{
    [Header("追従対象")]
    public Transform _Target; //追従対象
    public Vector3 _Offset = new Vector3(0f, 4.5f, 6f); //（左右,高さ,前後)カメラの位置補正 「Player から見たカメラ位置」
    public float _SmoothSpeed = 0.15f; //追従の滑らかさ
    public float _MouseSensitivity = 3f; //マウス感度

    [Header("視点制限")]
    public float _MinPitch=-45;
    public float _MaxPitch=75;
    private float _Yaw;
    private float _Pitch;

    [Header("障害物検知")]
    public LayerMask ObstacleLayers; // 障害物用レイヤー

    void Start()
    {
        // カーソルを非表示にする
        Cursor.visible = false;
        // カーソルを画面中央にロック（任意）
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        //_Targetを見つけれなかったら処理を最初から。
        if (!_Target) return;

        //マウス入力で回転
        _Yaw += Input.GetAxis("Mouse X") * _MouseSensitivity;
        _Pitch -= Input.GetAxis("Mouse Y") * _MouseSensitivity;
        _Pitch = Mathf.Clamp(_Pitch, _MinPitch, _MaxPitch); //カメラ上下制限

        // 📸 カメラ位置と回転を計算
        Quaternion rotation = Quaternion.Euler(_Pitch, _Yaw, 0);
        // カメラ位置をPivotと同じ高さに設定
        Vector3 desiredPosition = _Target.position - rotation * Vector3.forward * _Offset.z;
        desiredPosition.y += _Offset.y; // ← 高さは固定オフセットとして加算
        //Vector3 desiredPosition = _Target.position + new Vector3(_Offset.x, _Offset.y, -_Offset.z);


        // 🔹 障害物チェック
        RaycastHit hit;
        Vector3 direction = desiredPosition - _Target.position; // Player → カメラ方向
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(_Target.position + Vector3.up * 1f, direction, out hit, distance, ObstacleLayers))
        {
            // 障害物の手前に少し余裕をもたせる（0.3f）
            Vector3 adjustedPosition = hit.point - direction * 0.3f;

            // 地面より下には行かないように制限
            float minY = _Target.position.y + 2f; // 1fはカメラ高さの最低値
            if (adjustedPosition.y < minY)
                adjustedPosition.y = minY;

            desiredPosition = adjustedPosition;
        }

        // スムーズに追従
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _SmoothSpeed);

        float lookDownAngle = 20f;
        transform.rotation=Quaternion.Euler(lookDownAngle,_Yaw,0);
        // ターゲットを見る
        transform.LookAt(_Target.position + Vector3.up * 1f);
    }
}