using UnityEngine;
using TMPro;

public partial class Player : MonoBehaviour
{
    // Thành phần giao diện của UIController
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    // Hàm cập nhật điểm trên giao diện
    private void UpdateUI()
    {
        scoreText.text = "Score: " + score.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();
    }
}
