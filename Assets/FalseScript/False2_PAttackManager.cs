using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class False2_PAttackManager : MonoBehaviour
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
        _Player = GetComponent<Player_Manager>();
        Debug.Log("【Awake】 isAttacking:" + isAttacking + ", ComboStep:" + currentCombo);
    }

    void Update()
    {
        HandleAttackingInput(); // 毎フレーム、攻撃入力とコンボリセット判定をチェック
    }

    private void HandleAttackingInput()
    {
        if(Input.GetMouseButtonDown(0)) // マウス左クリックで攻撃入力
        {
            TryAttack();
        }

        if(Time.time - lastAttackTime > comboResetTime && isAttacking == false) // コンボが途切れて一定時間経過したらリセット
        {
            currentCombo = 0;
        }
    }

    private void TryAttack()
    {
        Debug.Log("【TryAttack】 isAttacking:" + isAttacking + ", ComboStep:" + currentCombo);

        if(!isAttacking) // 攻撃中でなければ攻撃を開始
        {
            currentCombo++;                     
            if(currentCombo > maxCombo)         
            {
                currentCombo = 1;
            }
            StartCoroutine(DoAttack());
        }
    }

    private IEnumerator DoAttack()
    {
        //----攻撃開始----
        isAttacking = true;             // 攻撃中フラグON
        _Anim.SetBool("isAttacking", true); // Animator パラメータ同期
        lastAttackTime = Time.time;     

        if(currentCombo == 1)
        {
            _Anim.SetTrigger("Attack");
        }

        _Anim.SetInteger("ComboStep", currentCombo);

        bool continueCombo = true;

        while (continueCombo && currentCombo <= maxCombo)
        {
            // 次コンボ入力受付のタイミングまで待機
            yield return new WaitUntil(() => // アニメーターが指定スラッシュアニメかつ後半まで再生されたら次入力受付 //今の攻撃アニメが指定状態になり、かつ一定以上再生されるまで、次のコンボ処理には進まない
                _Anim.GetCurrentAnimatorStateInfo(0).IsName($"Slash{currentCombo}") &&
                _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
            );

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
                continueCombo = false; // コンボ終了
                // AttackEnd はもう不要
            }
        }

        // 攻撃アニメーションが終わるまで待機
        yield return new WaitUntil(() =>
            _Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        // コンボ状態リセット
        _Anim.SetInteger("ComboStep", 0);
        currentCombo = 0;
        isAttacking = false;
        _Anim.SetBool("isAttacking", false); // Animator に同期
        Debug.Log("【DoAttack END】 isAttacking:" + isAttacking + ", ComboStep:" + currentCombo);
    }
}
