using System.Collections;          // コルーチンなどで必要
using System.Collections.Generic;  // List や Dictionary で必要
using UnityEngine;

public class Generic_Test : MonoBehaviour
{
    public float _Speed = 5f;         // 移動スピード
    public float _Gravity = -9.8f;    // 重力の強さ

    private CharacterController _Controller;
    private Vector3 _Velocity;        // 現在の速度
    private bool _IsGrounded;         // 接地判定
    private Transform _Cam;
    private Animator _Anim;

    void Awake()
    {
        _Anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _Anim.Play("Run2");
        if(!_Anim)
        {
            Debug.Log("NoneAnimetor");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
