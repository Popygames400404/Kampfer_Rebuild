using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_HitBoxPart : MonoBehaviour
{
    public enum HitBodyPart { Head, Middle, Leg, }

    [Header("•”ˆÊ‚ÌŽí—Þ‚ð‘I‘ð")]
    public HitBodyPart _Bodypart;

    [Range(0f, 5f)]
    public float _DamageMultiplier = 0.4f;

    private EnemyStatus _Life;

    void Awake()
    {
        var root = transform.root;
        _Life = GetComponentInParent<EnemyStatus>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //PlayerWepon‚©Šm”F
        WeponAttackHitBox Wepon = other.GetComponent<WeponAttackHitBox>();
        if (Wepon == null)
        {
            Debug.Log($"[HitBox] no weapon component on: {other.name}");
            return;
        }
        int finaldamage = Mathf.RoundToInt(Wepon.damage * _DamageMultiplier);
        Debug.Log($"[E_HitBox] Damage: {finaldamage} ({_Bodypart}) From {other.name}");

        _Life.TakeDamage(finaldamage);
    }
}
