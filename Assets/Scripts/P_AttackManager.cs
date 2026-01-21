using System.Collections;
using UnityEngine;

/*
P_AttackManager 改訂版
========================
全体的な流れ：
1. Update() で攻撃入力を監視
2. 攻撃開始 TryAttack() → DoAttack() Coroutine
3. DoAttack():
   - ComboStep に応じて Slash1～Slash3 を再生
   - 再生終了（normalizedTime >=1f）で ForceReturnToBase() 呼び出し
   - 次コンボ入力受付も Slash Transition に依存せず、強制的に進める
4. ForceReturnToBase():
   - isAttacking=false にして、Idle に強制クロスフェード
   - Run は Speed に応じて BaseLayer で自動切替
*/

public class P_AttackManager : MonoBehaviour
{
    [Header("コンボ設定")]
    public int maxCombo = 3;
    public float comboResetTime = 0.5f;

    private int currentCombo = 0;
    private bool isAttacking = false;
    private float comboTimer = 0f;

    [SerializeField] private Animator _Anim;
    [SerializeField] private string idleStateName = "Idle1";
    [SerializeField] private float returnBlendTime = 0.05f;

    void Awake()
    {
        _Anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleAttackingInput();

        // 攻撃中は Slash 再生終了を監視
        if (isAttacking)
        {
            AnimatorStateInfo state = _Anim.GetCurrentAnimatorStateInfo(0);

            // Attack タグのアニメーションが終了したら Base Layer に戻す
            if (state.IsTag("Attack") && state.normalizedTime >= 1f)
            {
                ForceReturnToBase();
            }
        }
    }

    private void HandleAttackingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (!isAttacking)
        {
            currentCombo++;
            if (currentCombo > maxCombo) currentCombo = 1;
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;
        _Anim.SetBool("isAttacking", true);

        _Anim.SetInteger("ComboStep", currentCombo);

        if (currentCombo == 1)
            _Anim.SetTrigger("Attack");

        bool continueCombo = true;

        while (continueCombo && currentCombo <= maxCombo)
        {
            // Slash 再生が70%まで進むのを待つ
            yield return new WaitUntil(() =>
            {
                var state = _Anim.GetCurrentAnimatorStateInfo(0);
                // ここで Slash が再生されていない場合も無理に待たない
                return state.IsName($"Slash{currentCombo}") || state.normalizedTime >= 1f;
            });

            comboTimer = 0f;
            bool nextInput = false;

            // コンボ入力受付（Slash Transition に依存せず）
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
                if (currentCombo > maxCombo) continueCombo = false;
                else _Anim.SetInteger("ComboStep", currentCombo);
            }
            else
            {
                continueCombo = false;
            }
        }

        // 攻撃終了後、Base Layer に戻す
        ForceReturnToBase();
    }

    private void ForceReturnToBase()
    {
        currentCombo = 0;
        isAttacking = false;
        _Anim.SetBool("isAttacking", false);
        _Anim.SetInteger("ComboStep", 0);

        // 強制的に Idle にクロスフェード
        _Anim.CrossFade(idleStateName, returnBlendTime, 0);

        Debug.Log("⚡ Force Return To BaseLayer");
    }
}
