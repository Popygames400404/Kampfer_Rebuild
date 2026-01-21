using System.Collections;
using UnityEngine;

/*
==========================================================
P_AttackManager（攻撃管理スクリプト） 関数ごとの処理フロー
==========================================================

1. Awake()
   - Animator と Player_Manager コンポーネントを取得
   - 初期設定を行う

2. Update()
   - 毎フレーム呼ばれる
   - HandleAttackingInput() を呼んで攻撃入力を監視
   - 攻撃中(isAttacking==true)の場合：
       - 現在再生中の AnimatorState を取得
       - Attack タグのアニメーションが normalizedTime >= 1f になったら
         → ForceReturnToBase() を呼び、Idle に戻す

3. HandleAttackingInput()
   - 左クリック（Input.GetMouseButtonDown(0)）で TryAttack() を呼ぶ
   - 攻撃入力を受け付けるだけの関数

4. TryAttack()
   - 攻撃中でなければ攻撃開始
   - currentCombo を進める（1～maxCombo）
   - コルーチン DoAttack() を開始
   - 1回目の攻撃の場合は Trigger を発火してSlash1再生

5. DoAttack() [コルーチン]
   - 攻撃開始フラグを立て、Animator に isAttacking と ComboStep を反映
   - Slash1～Slash3 のアニメーションを順番に再生
   - 各 Slash の 70% 再生時に次コンボ入力受付ウィンドウを開始
       - 入力があれば currentCombo を進めて次 Slash 再生
       - 入力がなければコンボ終了
   - 攻撃アニメが normalizedTime >= 1f になるまで待機
   - 攻撃終了後は isAttacking を false に戻し、Animator のパラメータをリセット

6. ForceReturnToBase()
   - 現在のコンボと攻撃フラグをリセット
   - Animator を Idle にクロスフェード
   - Run への遷移は Speed パラメータに応じて自動判定される

==========================================================
※ 補足
- currentCombo：現在のコンボステップ（Slash1～3）
- isAttacking：攻撃中フラグ
- comboTimer：コンボ入力受付用タイマー
- returnBlendTime：Idle へのクロスフェード時間
==========================================================
*/


public class False3_P_AttackManager : MonoBehaviour
{
    [Header("コンボ設定")]
    public int maxCombo = 3;            // 最大コンボ数（Slash1～3）
    public float comboResetTime = 0.5f; // コンボ入力受付時間（次攻撃入力を受け付けるウィンドウ）

    private int currentCombo = 0;       // 現在のコンボステップ（1～maxCombo）
    private bool isAttacking = false;   // 攻撃中フラグ
    private float comboTimer = 0f;      // コンボ入力受付タイマー

    [SerializeField] private Animator _Anim;          // アニメーターコンポーネント
    [SerializeField] private string idleStateName = "Idle1"; // BaseLayerのIdleステート名
    [SerializeField] private float returnBlendTime = 0.05f; // Idleに戻るときのクロスフェード時間
    private Player_Manager _Player;                   // プレイヤー管理スクリプトへの参照

    void Awake()
    {
        _Anim = GetComponent<Animator>(); // Animator を取得
        _Player = GetComponent<Player_Manager>(); // Player_Manager を取得
    }

    void Update()
    {
        HandleAttackingInput(); // 毎フレーム、攻撃入力をチェック

        // 攻撃中はアニメーション終了を監視
        if (isAttacking)
        {
            AnimatorStateInfo state = _Anim.GetCurrentAnimatorStateInfo(0);
            //Debug.Log($"State: {state.fullPathHash}, Name: {state.shortNameHash}, NormalizedTime: {state.normalizedTime}");
            //Debug.Log(_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //Debug.Log(_Anim.GetFloat("Speed"));
            Debug.Log(isAttacking);

            // Attack タグのアニメが再生終了したら BaseLayer に戻す
            if (state.IsTag("Attack") && state.normalizedTime >= 1f)
            {
                ForceReturnToBase(); // 強制的にIdleに戻す
            }
        }
    }

    private void HandleAttackingInput()
    {
        // 左クリックで攻撃入力
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack(); // 攻撃開始を試みる
        }
    }

    private void TryAttack()
    {
        // 攻撃中でなければ攻撃を開始
        if (!isAttacking)
        {
            currentCombo++; // コンボステップを進める
            if (currentCombo > maxCombo) currentCombo = 1; // 最大コンボを超えたら1に戻す
            StartCoroutine(DoAttack()); // 攻撃処理コルーチンを開始
        }
    }

    private IEnumerator DoAttack() // 攻撃処理
    {
        
        Debug.Log($"ComboStep: {currentCombo}");
        isAttacking = true;              // 攻撃中フラグON
        _Anim.SetBool("isAttacking", true); // Animatorに反映
        _Anim.SetInteger("ComboStep", currentCombo); // ComboStepをAnimatorに反映

        if (currentCombo == 1)
            _Anim.SetTrigger("Attack"); // 最初の攻撃はTriggerで再生

        bool continueCombo = true;

        while (continueCombo && currentCombo <= maxCombo)
        {
            // 現在のSlashアニメが70%以上再生されるまで待機
            yield return new WaitUntil(() =>
                _Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}") &&
                _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
            );

            comboTimer = 0f;
            bool nextInput = false;

            // コンボ入力受付ウィンドウ
            while (comboTimer < comboResetTime)
            {
                comboTimer += Time.deltaTime;
                if (Input.GetMouseButtonDown(0)) // 次攻撃入力検知
                {
                    nextInput = true;
                    break;
                }
                yield return null;
            }

            if (nextInput)
            {
                currentCombo++; // コンボ進行
                _Anim.SetInteger("ComboStep", currentCombo); // Animatorに反映
            }
            else
            {
                continueCombo = false; // コンボ終了
            }
        }

        // 攻撃アニメが完全に終了するまで待機
        yield return new WaitUntil(() =>
            _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        // 攻撃終了処理
        currentCombo = 0;
        _Anim.SetInteger("ComboStep", 0); // Animatorに反映
        isAttacking = false;
        _Anim.SetBool("isAttacking", false); // Animatorに反映
    }

    private void ForceReturnToBase()
    {
        // 強制的に攻撃終了
        currentCombo = 0;
        isAttacking = false;
        _Anim.SetBool("isAttacking", false);

        // Idleへクロスフェード（RunはSpeedに応じてAnimatorが判断）
        _Anim.CrossFade(idleStateName, returnBlendTime, 0);

        Debug.Log("⚡ Force Return To BaseLayer");
    }
}
