using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxPart : MonoBehaviour
{
    public enum HitBodyPart{Head,Middle,Leg,Shield}

    [Header("部位の種類を選択")]
    public HitBodyPart _Bodypart;

    [Range(0f, 5f)]
    public float _DamageMultiplier = 0.4f;

    private Life_Manager _Life;

    void Awake()
    {
        // どのRootを見ているか確認
        var root = transform.root;
        //Debug.Log($"[{gameObject.name}] root = {root.name}");

        _Life = GetComponentInParent<Life_Manager>();
        if (_Life == null)
        {
            //Debug.LogError($"Life_Manager not found on parent of {gameObject.name}");
        }
        else
        {
            //Debug.Log($"Life_Manager FOUND for {gameObject.name}. Root = {_Life.gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[HitBox] triggered by {other.name} layer:{LayerMask.LayerToName(other.gameObject.layer)}");

        Debug.Log($"[HitBox] {gameObject.name} triggered by: {other.name}");

        //敵Weponかを確認
        E_WeponAttackHitBox Wepon = other.GetComponent<E_WeponAttackHitBox>();
        if (Wepon == null)
        {
            Debug.Log($"[HitBox] no weapon component on: {other.name}");
            return;
        }
        int finaldamage = Mathf.RoundToInt(Wepon.damage * _DamageMultiplier);

        Debug.Log($"[HitBox] Damage: {finaldamage} ({_Bodypart}) From {other.name}");

        _Life.TakeDamage(finaldamage);
    }
}