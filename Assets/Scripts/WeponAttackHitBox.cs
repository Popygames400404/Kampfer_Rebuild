
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponAttackHitBox : MonoBehaviour
{
    public int damage = 20;              // 与えるダメージ
    public string targetTag = "Enemy";   // 攻撃対象のTag (Playerなら"Player")

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[Weapon] Hit: {other.name} (layer:{LayerMask.LayerToName(other.gameObject.layer)})");
        //Debug.Log($"[Weapon] Hit: {other.name}");

        E_HitBoxPart hitbox = other.GetComponent<E_HitBoxPart>();
        if (hitbox != null)
        {
            //Debug.Log($"[Weapon] HitBox detected on part: {hitbox._Bodypart}");
        }
    }
}