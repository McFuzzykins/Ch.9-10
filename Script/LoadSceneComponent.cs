using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    float timer = 0;
    public string loadThisScene;

    void Start()
    {
        GameManager.Instance.GetComponentInChildren<ScoreManager>().
            ResetScore();
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3)
        {
            SceneManager.LoadScene(loadThisScene);
        }
    }
}
