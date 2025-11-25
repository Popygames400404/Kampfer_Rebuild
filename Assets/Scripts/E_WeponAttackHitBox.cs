using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_WeponAttackHitBox : MonoBehaviour
{
    public int damage = 20;              // —^‚¦‚éƒ_ƒ[ƒW
    public string targetTag = "Player";   // UŒ‚‘ÎÛ‚ÌTag (Player‚È‚ç"Player")

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log($"[Weapon] Hit: {other.name}");

    //    HitBoxPart hitbox = other.GetComponent<HitBoxPart>();
    //    if (hitbox != null)
    //    {
    //        Debug.Log($"[Weapon] HitBox detected on part: {hitbox._Bodypart}");
    //    }
    //}
}
