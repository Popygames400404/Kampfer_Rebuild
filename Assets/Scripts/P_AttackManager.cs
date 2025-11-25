using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_AttackManager : MonoBehaviour
{
    [Header("コンボ設定")]
    public int maxCombo = 3;// 最大コンボ数
    public float comboResetTime = 0.5f;// コンボ入力受付時間
    private int currentCombo = 0;// 現在のコンボ段階
    private bool isAttacking = false;// 攻撃中フラグ
    private float lastAttackTime;// 最後に攻撃した時間
    private float comboTimer = 0f;           // コンボ入力タイマー

    private Animator _Anim;
    private Player_Manager _Player;

    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Player = GetComponent<Player_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleAttackingInput();
    }

    private void HandleAttackingInput()
    {
        //左クリックで攻撃
        if(Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
        //一定時間経過でリセット
        if(Time.time-lastAttackTime>comboResetTime&&isAttacking==false)
        {
            currentCombo = 0;
        }
    }

    private void TryAttack()
    {
        if(!isAttacking)
        {
            currentCombo++;
            if(currentCombo>maxCombo)
            {
                currentCombo = 1;
            }
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if(currentCombo==1)
        {
            _Anim.SetTrigger("Attack");
        }

        _Anim.SetInteger("ComboStep",currentCombo);

        bool continueCombo = true;

        while (continueCombo && currentCombo <= maxCombo)
        {
            // 現在の攻撃アニメ終了まで待つ
            yield return new WaitUntil(() =>
                _Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}") &&
                _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
            );

            // 入力受付（コンボ途中キャンセル対応）
            comboTimer = 0f;
            bool nextInput = false;
            while (comboTimer < comboResetTime)
            {
                comboTimer += Time.deltaTime;
                if (Input.GetMouseButtonDown(0))
                {
                    nextInput = true;
                    break;
                }
                yield return null;
            }

            if (nextInput)
            {
                currentCombo++;
                _Anim.SetInteger("ComboStep", currentCombo);
            }
            else
            {
                // コンボ途中終了
                _Anim.SetBool("AttackEnd", true); // Animator の Exit 条件用
                continueCombo = false;
            }
        }

        // コンボ終了時の後処理
        yield return new WaitUntil(() =>
            !_Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}") // SubStateMachine を抜けるまで待つ
        );

        _Anim.SetInteger("ComboStep", 0);
        _Anim.SetBool("AttackEnd", false);
        currentCombo = 0;
        isAttacking = false;
    }

    private IEnumerator ResetComboAfterDelay()
    {
        Debug.Log("");
        yield return new WaitForSeconds(comboResetTime);
        currentCombo = 0;
        _Anim.SetInteger("ComboStep", 0);
    }
}
