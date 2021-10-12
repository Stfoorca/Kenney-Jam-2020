using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    private string mainSceneName = "Main";

    private void Awake()
    {
        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            Debug.Log("Master Bank Loaded");
            SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
        }
    }
}
