using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLanguage : MonoBehaviour
{
    [SerializeField] string key;
    TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = LanguageManager.Instance.GetLocalizedString(key);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
