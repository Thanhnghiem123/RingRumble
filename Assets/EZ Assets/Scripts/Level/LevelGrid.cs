using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    public GameObject levelButtonPrefab; // Gán Prefab của Button vào đây
    public int numberOfLevels = 10; // Số lượng vòng chơi

    void Start()
    {
        // Lấy component Grid Layout Group
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();

        // Tạo 10 nút cho 10 vòng chơi
        for (int i = 1; i <= numberOfLevels; i++)
        {
            // Tạo instance của Button từ Prefab
            GameObject newButton = Instantiate(levelButtonPrefab, transform);

            // Lấy Text component (hoặc TextMeshProUGUI nếu bạn dùng TMP)
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "" + i; // Đặt tên cho nút: Level 1, Level 2,...
            }

            // (Tùy chọn) Thêm sự kiện khi click nút
            int levelIndex = i; // Lưu giá trị i để dùng trong lambda
            newButton.GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
    }

    // Hàm xử lý khi nút được click
    void OnLevelButtonClicked(int level)
    {
        Debug.Log("Bạn đã chọn vòng chơi: " + level);
        // Thêm logic để tải vòng chơi, ví dụ: SceneManager.LoadScene("Level" + level);
    }
}