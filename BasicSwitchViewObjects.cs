using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BasicSwitchViewObjects : MonoBehaviour
{

    public GameObject[] showObjects;
    public GameObject[] buttons;
    private void Start()
    {
        SelectObject(PlayerPrefs.GetInt("lastChoise"));
    }
    public void SelectObject(int index)
    {
        foreach (var item in buttons)
        {
            item.GetComponent<Image>().color = Color.white;
        }
        buttons[index].GetComponent<Image>().color = Color.green;
        foreach (var item in showObjects)
        {
            item.SetActive(false);
        }
        showObjects[index].SetActive(true);
        PlayerPrefs.SetInt("lastChoise", index);
    }
}
