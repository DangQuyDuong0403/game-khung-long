using System.Collections;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    // Biến điểm số thuộc ScoreManager
    public int score = 0; // **Điểm số hiện tại của người chơi**
    public int highScore = 0; // **Điểm số cao nhất đã đạt được**

    // Hàm tăng điểm của người chơi
    private void IncreaseScore()
    {
        score += 1;  // **Tăng điểm**
        UpdateUI();  // (UIController)
        Debug.Log("[IncreaseScore] Điểm hiện tại: " + score);
    }

    // Coroutine này tăng điểm số mỗi khoảng thời gian
    private IEnumerator IncreaseScoreOverTime(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);  // **Chờ một khoảng thời gian delay**
            IncreaseScore();  // **Tăng điểm**
            Debug.Log("[Coroutine] Điểm tăng mỗi " + delay + " giây.");
        }
    }

    // Coroutine này tạo hiệu ứng nhấp nháy cho nhân vật khi chết
    private IEnumerator FlashEffectCoroutine()
    {
        for (int i = 0; i < 5; i++)  // **Lặp lại 5 lần**
        {
            GetComponent<SpriteRenderer>().enabled = false;  // **Tắt renderer của nhân vật**
            yield return new WaitForSeconds(0.3f);  // **Chờ 0.3 giây**
            GetComponent<SpriteRenderer>().enabled = true;  // **Bật lại renderer**
            yield return new WaitForSeconds(0.3f);  // **Chờ tiếp 0.3 giây**
        }
    }
}
