using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public static int playerLives = 3;
    public static int currentScene = 0;
    public static int gameLevelScene = 3;
    static GameManager instance;

    bool died = false;
    public bool Died
    {
        get { return died; }
        set { died = value; }
    }
   
    public static GameManager Instance
    {
        get { return instance; }
        
    }
    void Awake()
    {
        CheckGameManagerIsInTheScene();
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        LightandCameraSetup(currentScene);
    }
    void Start()
    {
        SetLivesDisplay(playerLives);
    }
    public void SetLivesDisplay(int players)
    {
        if (GameObject.Find("lives"))
        {
            GameObject lives = GameObject.Find("lives");
            
            if (lives.transform.childCount < 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject life = GameObject.Instantiate(Resources.Load
                        ("Prefab/life")) as GameObject;
                    life.transform.SetParent(lives.transform);
                }
            }
            //set player lives
            for (int i = 0; i < lives.transform.childCount; i++)
            {
                lives.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
            }
            //remove visual lives
            for (int i = 0; i <(lives.transform.childCount - players); i++)
            {
                lives.transform.GetChild(lives.transform.childCount - i -1).localScale =
                    Vector3.zero;
            }
        }
    }
    void CheckGameManagerIsInTheScene()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }
    void LightandCameraSetup(int sceneNumber)
    {
        switch (sceneNumber)
        {
            //level1. level2, level3
            case 3: case 4: case 5: 
            {
                LightSetup();
                CameraSetup();
                break;     
            }
        }
    }
    void CameraSetup()
    { 
        GameObject gameCamera = GameObject.FindGameObjectWithTag("MainCamera");
            //Camera Transform
            gameCamera.transform.position = new Vector3(0, 0, -300);
            gameCamera.transform.eulerAngles = new Vector3(0, 0, 0);

            //Camera Properties
            gameCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            gameCamera.GetComponent<Camera>().backgroundColor = new Color32(0, 0, 0, 255);
    }
    void LightSetup()
    {
        GameObject dirLight = GameObject.Find("Directional Light");
        dirLight.transform.eulerAngles = new Vector3(50, -30, 0);
        dirLight.GetComponent<Light>().color = new Color32(152, 204, 255, 255);
    }
    public void LifeLost()
    {
        //lose life
        if (playerLives >= 1)
        {
            playerLives--;
            Debug.Log("Lives Left:" + playerLives);
            GetComponent<ScenesManager>().ResetScene();
            
        }
        else
        {
            GetComponent<ScenesManager>().GameOver();
            //reset lives back to 3
            playerLives = 3;
        }
    }
}