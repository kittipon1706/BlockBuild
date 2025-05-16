using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scene_manager : MonoBehaviour
{
    public static scene_manager instance;

    [SerializeField]
    private Button edit_btn;
    [SerializeField]
    private Button create_btn;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        edit_btn.onClick.AddListener(LoadEditScene);
        create_btn.onClick.AddListener(LoadCreateScene);
    }

    private void LoadEditScene()
    {
        SceneManager.LoadScene(1);
        edit_btn.onClick.RemoveAllListeners();
        create_btn.onClick.RemoveAllListeners();
    }

    private void LoadCreateScene()
    {
        SceneManager.LoadScene(2);
        edit_btn.onClick.RemoveAllListeners();
        create_btn.onClick.RemoveAllListeners();
    }
}
