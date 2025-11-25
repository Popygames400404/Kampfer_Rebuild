using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP_Manager : MonoBehaviour
{
    public static PlayerHP_Manager Instance;

    [Header("Player UI")]
    public Slider playerHPBar;

    void Awake()
    {
        Instance = this;
    }

    // 🟢 Player の HP 反映
    public void UpdatePlayerHP(int current, int max)
    {
        float rate = (float)current / max;

        playerHPBar.value = rate;
    }
}
