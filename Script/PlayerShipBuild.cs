using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;

public class PlayerShipBuild : MonoBehaviour
{
    [SerializeField]
    GameObject[] visualWeapons;
    GameObject textBoxPanel;
    GameObject bankObj;
    GameObject buyButton; 
    GameObject tmpSelection;
    int bank = 600;
    string placementID_rewardedvideo = "rewardedvideo";
    string gameID = "4075685";
    bool purchaseMade = false;

    [SerializeField]
    SOActorModel defaultPlayerShip;
    GameObject playerShip;
    void Start()
    {
        purchaseMade = false;
        bankObj = GameObject.Find("bank");
        bankObj.GetComponentInChildren<TextMesh>().text = bank.ToString();
        textBoxPanel = GameObject.Find("textBoxPanel");
        buyButton = GameObject.Find("BUY?").gameObject;
        buyButton.SetActive(false);
        CheckPlatform();
        TurnOffPlayerShipVisuals();
        PreparePlayerShipForUpgrade();
        TurnOffSelectionHighlights();
    }
    void PreparePlayerShipForUpgrade()
    {
        playerShip = GameObject.Instantiate(Resources.Load
            ("Prefab/Player/Player_Ship")) as GameObject;
        playerShip.GetComponent<Player>().enabled = false;
        playerShip.transform.position = new Vector3(0, 10000, 0);
        playerShip.GetComponent<Player>
            ().ActorStats(defaultPlayerShip);
    }
    void CheckPlatform()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameID = "4075684";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            gameID = "4075685";
        }
        Monetization.Initialize(gameID, false);
    }
    
    //GameObject ReturnClickedObject (out RaycastHit hit)
    //{
    //    GameObject target = null;
    //    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        
    //    if (Physics.Raycast(ray.origin, ray.direction * 100, out hit))
    //    {
    //        target = hit.collider.gameObject;
    //    }
    //    return target;
    //}
    void TurnOffSelectionHighlights()
    {
        GameObject[] selections = GameObject.FindGameObjectsWithTag("Selection");
        for (int i = 0; i < selections.Length; i++)
        {
            if (selections[i].GetComponentInParent<ShopPiece>())
            {
                if (selections[i].GetComponentInParent<ShopPiece>().ShopSelection.iconName == "sold Out")
                {
                    selections[i].SetActive(false);
                }
            }
        }
    }
    void UpdateDescriptionBox()
    {
        textBoxPanel.transform.Find("name").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName;
        textBoxPanel.transform.Find("desc").gameObject.GetComponent<TextMesh>().text = tmpSelection.GetComponent<ShopPiece>().ShopSelection.description;
    }
    //void Select()
    //{
    //    tmpSelection = target.transform.Find("SelectionQuad").gameObject;
    //    tmpSelection.SetActive(true);
    //}


    public void AttemptSelection(GameObject buttonName)
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hitInfo;
        //    target = ReturnClickedObject(out hitInfo);

        //    if (target != null)
        //    {
        //Select();
        //UpdateDescriptionBox();
        if (buttonName)
        {
            TurnOffSelectionHighlights();

            tmpSelection = buttonName;
            tmpSelection.transform.GetChild(1).
                gameObject.SetActive(true);

            UpdateDescriptionBox();

            //NOT ALREADY SOLD
            if (buttonName.GetComponentInChildren<Text>().text != "SOLD")
            {
                //can afford
                Affordable();

                //cannot afford
                LackOfCredits();
            }
            else if (buttonName.GetComponentInChildren<Text>().text == "SOLD")
            {
                SoldOut();
            }
        }
    }
    //            else if (target.name == "BUY ?")
    //            {
    //                BuyItem();
    //            }
    //            else if (target.name == "START")
    //            {
    //                StartGame();
    //            }
    //            else if (target.name == "WATCH AD")
    //            {
    //                WatchAdvert();
    //            }
    //        }
    //    }
    //}
    public void WatchAdvert()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            ShowRewardedAds();
        }
    }
    
    IEnumerator WaitForAd()
    {
        string placementID = placementID_rewardedvideo;
        while (!Monetization.IsReady(placementID))
        {
            yield return null;
        }
        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementID) as ShowAdPlacementContent;

        if (ad != null)
        {
            ad.Show(AdFinished);
        }
    }
    void AdFinished (ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            bank += 300;
            bankObj.GetComponentInChildren<TextMesh>().text = bank.ToString();
            //TurnOffSelectionHighlights();
        }
    }
    void ShowRewardedAds()
    {
        StartCoroutine(WaitForAd());
    }
    public void BuyItem()
    {
        Debug.Log("PURCHASED");
        purchaseMade = true;
        buyButton.SetActive(false);
        textBoxPanel.transform.Find("desc").gameObject.GetComponent<TextMesh>().text = "";
        textBoxPanel.transform.Find("name").gameObject.GetComponent<TextMesh>().text = "";

        for (int i = 0; i < visualWeapons.Length; i++)
        {
            if (visualWeapons[i].name == tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName)
            {
                visualWeapons[i].SetActive(true);
            }
        }

        UpgradeToShip(tmpSelection.GetComponent<ShopPiece>().ShopSelection.iconName);

        bank = bank - System.Int16.Parse(tmpSelection.GetComponent<ShopPiece>().ShopSelection.cost);
        
        bankObj.transform.Find("bankText").GetComponent<TextMesh>().text = bank.ToString();
        
        tmpSelection.transform.Find("itemText").GetComponentInChildren<Text>().text = "SOLD";
    }
    void UpgradeToShip(string upgrade)
    {
        GameObject shipItem = GameObject.Instantiate(Resources.Load
            ("Prefab/Player/"+upgrade)) as GameObject;
        shipItem.transform.SetParent(playerShip.transform);
        shipItem.transform.localPosition = Vector3.zero;
    }
    //void Update()
    //{
    //    AttemptSelection();
    //}
    void Affordable()
    {
        if (bank >= System.Int32.Parse(tmpSelection.GetComponent<ShopPiece>().ShopSelection.cost))
        {
            Debug.Log("CAN BUY");
            buyButton.SetActive(true);
        }
    }
    void LackOfCredits()
    {
        if (bank < System.Int32.Parse(tmpSelection.GetComponent<TextMesh>().text))
        {
            Debug.Log("CAN'T BUY");
        }
    }
    void SoldOut()
    {
        Debug.Log("SOLD OUT");
    }
    void TurnOffPlayerShipVisuals()
    {
        for (int i = 0; i < visualWeapons.Length; i++)
        {
            visualWeapons[i].gameObject.SetActive(false);
        }
    }  
    public void StartGame()
    {
        if (purchaseMade)
        {
            playerShip.name = "UpgradedShip";
           
            if (playerShip.transform.Find("energy +1(Clone)"))
            {
                playerShip.GetComponent<Player>().Health = 2;
            }
            DontDestroyOnLoad(playerShip);
        }
        GameManager.Instance.GetComponent<ScenesManager>().
            BeginGame(GameManager.gameLevelScene);
    }
}
