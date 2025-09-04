using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ApiCMS.FileCAD;
using TMPro;
public class CADFileListFromCMSUI : MonoBehaviour
{
    public List<CADFileFromCmsCard> listCards;
    public TMP_InputField inputFieldSearch;

    [SerializeField] CADFileFromCmsCard cardPrefab;
    [SerializeField] Transform rootCard;
    [SerializeField] GameObject emptyFileObj;
    [SerializeField] GameObject iconSearchObj;


    public WorkSessionInfo infoTest;
    private void Start()
    {
        inputFieldSearch.onValueChanged.AddListener(OnInputFieldChange);
        CreatList();
    }
    public void CreatList()
    {
        if (FileCADManager.instance.listFile.result.data.Count == 0) return;
        foreach (var item in FileCADManager.instance.listFile.result.data)
        {
            var wsi = FileCADManager.instance.WSIfromID(item.id);
            AddCard(wsi);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CreatList();
        }
    }

    public void AddCard(WorkSessionInfo info)
    {
        CADFileFromCmsCard newCard = Instantiate(cardPrefab, rootCard);
        newCard.SetData(info);
        listCards.Add(newCard);
    }
    public void Back()
    {
        
    }
    public void Search()
    {
        if (inputFieldSearch.text == null) return;

        string keyWord = inputFieldSearch.text;

        List<CADFileFromCmsCard> result = listCards.FindAll(x => x.myInfo.nameMRLine.Contains(keyWord, StringComparison.OrdinalIgnoreCase));

        if (result.Count > 0)
        {
            foreach (var item in listCards)
            {
                if (!result.Exists(x => x == item))
                    item.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var item in listCards)
            {
                item.gameObject.SetActive(false);
            }
        }
        emptyFileObj.SetActive(result.Count == 0);
    }
    public void CancelSearch()
    {
        foreach (var item in listCards)
        {
            if (!item.gameObject.activeInHierarchy)
                item.gameObject.SetActive(true);
        }
        inputFieldSearch.text = "";
        iconSearchObj.gameObject.SetActive(true);
        emptyFileObj.SetActive(false);
    }
    public void OnInputFieldChange(string str)
    {
        if (str == "")
            CancelSearch();
        else
            iconSearchObj.gameObject.SetActive(false);
    }
}
