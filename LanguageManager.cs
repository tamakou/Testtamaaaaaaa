using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance; // Để truy cập từ mọi nơi

    private Dictionary<string, string> languageDictionary;
    [SerializeField]
    private string currentLanguage = "en"; // Ngôn ngữ mặc định (tiếng Anh)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo rằng nó không bị hủy khi chuyển cảnh
            LoadLanguageData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLanguageData()
    {
        TextAsset languageData = Resources.Load<TextAsset>("language_" + currentLanguage);
        languageDictionary = SimpleJson.SimpleJson.DeserializeObject <Dictionary<string, string>>(languageData.text);
    }

    public string GetLocalizedString(string key)
    {
        if (languageDictionary.ContainsKey(key))
        {
            return languageDictionary[key];
        }
        else
        {
            // Trả về chuỗi gốc nếu không tìm thấy phiên bản dịch
            return key;
        }
    }

}