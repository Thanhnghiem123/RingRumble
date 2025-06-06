using UnityEngine;
using UnityEngine.UI; // Để sử dụng Text, Button, v.v.
using TMPro; // Để hỗ trợ TextMeshPro nếu bạn dùng nó

public class PopupEnd : MonoBehaviour
{
    // Singleton instance
    public static PopupEnd Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject popupPanel; // Panel của popup
    [SerializeField] private TextMeshProUGUI popupText; // Text UI
    // Nếu bạn dùng TextMeshPro thay vì Text, bỏ comment dòng dưới và comment dòng trên
    // [SerializeField] private TextMeshProUGUI popupText;

    private void Awake()
    {
        // Singleton: Nếu đã có instance, hủy đối tượng mới
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("PopupEnd: Another instance of PopupEnd already exists! Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Kiểm tra các thành phần UI
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

        // Ẩn popup khi khởi tạo
        HidePopup();
    }

    // Hiển thị popup
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

    // Ẩn popup
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

    // Đổi text thành "Victory"
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

    // Đổi text thành "Defeat"
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

    // Hiển thị popup với text "Victory"
    public void ShowVictoryPopup()
    {
        SetVictoryText();
        ShowPopup();
    }

    // Hiển thị popup với text "Defeat"
    public void ShowDefeatPopup()
    {
        SetDefeatText();
        ShowPopup();
    }

    // Tùy chọn: Đổi text tùy chỉnh
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

    // Tùy chọn: Hiển thị popup với text tùy chỉnh
    public void ShowCustomPopup(string customText)
    {
        SetCustomText(customText);
        ShowPopup();
    }
}