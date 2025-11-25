using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSystem_Manager : MonoBehaviour
{
    public Button _startButton;

    private Player_Manager _P_Manager;
    private Camera_Manager _C_Manager;

    void Awake()
    {
        _P_Manager = FindObjectOfType<Player_Manager>();
        if (_P_Manager != null) 
        {
            Debug.Log("見つけた");
            _P_Manager.enabled = false;
        }
        _C_Manager = FindObjectOfType<Camera_Manager>();
        if (_C_Manager != null) 
        {
            Debug.Log("見つけた");
            _C_Manager.enabled = false;
        }
    }

    void Start()
    {
        _startButton.onClick.AddListener(OnStartBattle);
    }

    public void OnStartBattle()
    {
        Debug.Log("GAME_START");
        // ここにシーン遷移やバトル処理を追加
        _startButton.gameObject.SetActive(false);
        _P_Manager.enabled = true;
        _C_Manager.enabled = true;
    }
}
