using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_AttackManager : MonoBehaviour
{
    [Header("コンボ設定")]
    public int maxCombo = 3;           // 最大コンボ数
    public float comboResetTime = 0.5f; // コンボ入力をリセットする時間

    private int currentCombo = 0;      // 現在のコンボ段階
    private bool isAttacking = false;  // 攻撃中かどうか
    private float lastAttackTime;      // 最後に攻撃した時間
    private float comboTimer = 0f;     // コンボ入力受付用タイマー

    private Animator _Anim;            // Animatorコンポーネントの参照
    private Player_Manager _Player;    // プレイヤー管理スクリプトの参照

    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Player = GetComponent<Player_Manager>();// ゲーム開始時にAnimatorとPlayer_Managerを取得
    }

    void Update()
    {
        HandleAttackingInput();// 毎フレーム、攻撃入力とコンボリセット判定をチェック
    }

    private void HandleAttackingInput()
    {
        if(Input.GetMouseButtonDown(0))// マウス左クリックで攻撃入力
        {
            TryAttack();
        }

        if(Time.time - lastAttackTime > comboResetTime && isAttacking == false)// コンボが途切れて一定時間経過したらリセット
        {
            currentCombo = 0;
        }
    }

    private void TryAttack()
    {
        if(!isAttacking) // 攻撃中でなければ攻撃を開始
        {
            currentCombo++;                     // コンボ段階を進める
            if(currentCombo > maxCombo)         // 最大コンボ(3)を超えたら1に戻す
            {
                currentCombo = 1;
            }
            StartCoroutine(DoAttack());         // 実際の攻撃処理を開始（アニメーション再生）
        }
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;             // 攻撃中フラグON
        lastAttackTime = Time.time;     // 最後の攻撃時間を更新

        if(currentCombo == 1)// 1段目の攻撃ならTriggerでアニメーション再生
        {
            _Anim.SetTrigger("Attack");
        }

        // ComboStepパラメータに現在の段階をセット
        _Anim.SetInteger("ComboStep", currentCombo);

        bool continueCombo = true;      // コンボ継続フラグ

        while (continueCombo && currentCombo <= maxCombo)//コンボを続けるかどうかの管理。
        {
            // アニメーターが指定スラッシュアニメかつ後半まで再生されたら次入力受付
            yield return new WaitUntil(() =>    //コルーチン枠
                _Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}") &&      //今の攻撃アニメが指定状態になり、かつ一定以上再生されるまで、
                _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f);               //次のコンボ処理には進まない    ←   コルーチン部分

            // コンボ入力タイマーをリセット
            comboTimer = 0f;
            bool nextInput = false;

            // コンボ入力受付時間中、次の攻撃ボタンが押されたかチェック
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
                // 次のコンボ段階へ
                currentCombo++;
                _Anim.SetInteger("ComboStep", currentCombo);
            }
            else
            {
                // コンボ終了
                _Anim.SetBool("AttackEnd", true); // Animator側でExit処理用
                continueCombo = false;
            }
        }

        // 攻撃アニメーションが終わるまで待機
        yield return new WaitUntil(() =>
            !_Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}")
        );

        // コンボ状態リセット
        _Anim.SetInteger("ComboStep", 0);
        _Anim.SetBool("AttackEnd", false);
        currentCombo = 0;
        isAttacking = false;           // 攻撃中フラグOFF
    }

    private IEnumerator ResetComboAfterDelay()
    {
        // もし必要ならコンボリセット用コルーチン
        yield return new WaitForSeconds(comboResetTime);
        currentCombo = 0;
        _Anim.SetInteger("ComboStep", 0);
    }
}
