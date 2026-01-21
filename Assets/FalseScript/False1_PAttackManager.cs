using System.Collections;
using UnityEngine;

public class False1_PAttackManager : MonoBehaviour
{
    [Header("コンボ設定")]
    public int maxCombo = 3;
    public float comboResetTime = 0.8f;

    private int currentCombo = 0;
    private bool isAttacking = false;
    private float lastAttackTime;
    private float comboTimer = 0f;

    private Animator _Anim;

    void Awake()
    {
        _Anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleAttackingInput();
    }

    private void HandleAttackingInput() //入力
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        if (Time.time - lastAttackTime > comboResetTime && !isAttacking)
        {
            currentCombo = 0;
        }
    }

    private void TryAttack()
    {
        if (isAttacking) return;

        currentCombo++;
        if (currentCombo > maxCombo)
        {
            currentCombo = 1;
        }

        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        // --- 攻撃開始 ---
        isAttacking = true;
        _Anim.SetBool("isAttacking", true);
        lastAttackTime = Time.time;

        _Anim.SetTrigger("Attack");
        _Anim.SetInteger("ComboStep", currentCombo);

        // --- 次入力受付（ここは維持） ---
        yield return new WaitForSeconds(0.3f); // Slash前半

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

        if (nextInput && currentCombo < maxCombo)
        {
            currentCombo++;
            _Anim.SetInteger("ComboStep", currentCombo);
        }

        // --- ⭐ここが最重要：攻撃終了を時間で保証 ---
        yield return new WaitForSeconds(0.4f); // ← Slash1本分の長さ

        // --- 確実に攻撃終了 ---
        currentCombo = 0;
        _Anim.SetInteger("ComboStep", 0);

        isAttacking = false;
        _Anim.SetBool("isAttacking", false);

        Debug.Log("【Attack End】isAttacking OFF");
    }
}
