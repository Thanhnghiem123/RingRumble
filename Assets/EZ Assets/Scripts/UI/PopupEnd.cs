using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupEnd : MonoBehaviour
{
    // Singleton instance
    public static PopupEnd Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject popupPanel; 
    [SerializeField] private TextMeshProUGUI popupText; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("PopupEnd: Another instance of PopupEnd already exists! Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (popupPanel == null)
        {
            Debug.LogWarning("PopupEnd: popupPanel is not assigned!");
            return;
        }
        if (popupText == null)
        {
            Debug.LogWarning("PopupEnd: popupText is not assigned!");
            return;
        }

        HidePopup();
    }

    public void ShowPopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PopupEnd: Cannot show popup, popupPanel is null!");
        }
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PopupEnd: Cannot hide popup, popupPanel is null!");
        }
    }

    public void SetVictoryText()
    {
        if (popupText != null)
        {
            popupText.text = "Victory";
        }
        else
        {
            Debug.LogWarning("PopupEnd: Cannot set Victory text, popupText is null!");
        }
    }

    public void SetDefeatText()
    {
        if (popupText != null)
        {
            popupText.text = "Defeat";
        }
        else
        {
            Debug.LogWarning("PopupEnd: Cannot set Defeat text, popupText is null!");
        }
    }

    public void ShowVictoryPopup()
    {
        SetVictoryText();
        ShowPopup();
    }

    public void ShowDefeatPopup()
    {
        SetDefeatText();
        ShowPopup();
    }

    public void SetCustomText(string customText)
    {
        if (popupText != null)
        {
            popupText.text = customText;
        }
        else
        {
            Debug.LogWarning("PopupEnd: Cannot set custom text, popupText is null!");
        }
    }

    public void ShowCustomPopup(string customText)
    {
        SetCustomText(customText);
        ShowPopup();
    }
}