using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public partial class Player : MonoBehaviour
{
    // Khai báo các thành phần cần thiết của PlayerController
    Rigidbody2D rb; // **Component vật lý (Rigidbody) của nhân vật**
    Animator anim; // **Component điều khiển hoạt ảnh của nhân vật**
    public float Jump; // **Lực nhảy của nhân vật**
    public bool isJumping = false; // **Kiểm tra trạng thái nhảy của nhân vật**
    public GameObject RestartBO; // **Nút Restart khi game over**

    // Hàm này được gọi khi game bắt đầu
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // **Lấy component Rigidbody2D**
        anim = GetComponent<Animator>();   // **Lấy component Animator**
        LoadGameData();  // **Tải dữ liệu game đã lưu trước đó** (SystemManager)
        StartCoroutine(IncreaseScoreOverTime(1f));  // **Bắt đầu Coroutine để tăng điểm** (ScoreManager)
        UpdateUI();      // **Cập nhật giao diện** (UIController)
        Debug.Log("[Start] Game bắt đầu!");
    }

    // Hàm này gọi mỗi frame, sử dụng để kiểm tra các sự kiện nhảy
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isJumping)  // **Nếu nhấn chuột trái và nhân vật đang ở trên mặt đất**
        {
            rb.AddForce(Vector2.up * Jump);  // **Áp dụng lực nhảy lên nhân vật**
            isJumping = false;  // **Đánh dấu nhân vật không còn ở trạng thái nhảy**
            anim.SetBool("IsJumping", true);  // **Bật hoạt ảnh nhảy**
            Debug.Log("[Update] Người chơi nhảy!");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteGameData(); // (SystemManager)
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            WaitAndStop(2f); // (SystemManager) – Lưu ý: giữ nguyên như cũ
        }
    }

    // Hàm này gọi mỗi frame vật lý, dùng để cập nhật các yếu tố vật lý
    void FixedUpdate()
    {
        // Cập nhật các lực vật lý, chẳng hạn như trọng lực
        //if (rb.linearVelocity.y < 0)
        //{
        //    anim.SetBool("IsFalling", true);  // **Bật hoạt ảnh rơi nếu đang di chuyển xuống dưới**
        //}
    }

    // Hàm này được gọi khi có va chạm với các đối tượng khác
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")  // **Nếu va chạm với mặt đất**
        {
            isJumping = true;  // **Đánh dấu nhân vật có thể nhảy lại**
            anim.SetBool("IsJumping", false);  // **Tắt hoạt ảnh nhảy**
            Debug.Log("[OnCollisionEnter2D] Chạm đất!");
        }
        if (collision.gameObject.tag == "Enemy")  // **Nếu va chạm với kẻ địch**
        {
            anim.SetBool("Dead", true);  // **Bật hoạt ảnh chết**
            Time.timeScale = 0;  // **Dừng thời gian khi game over**
            RestartBO.SetActive(true);  // **Hiển thị nút Restart**
            StopAllCoroutines();  // **Dừng tất cả Coroutine khi game over**
            Debug.Log("[OnCollisionEnter2D] Game Over.");
            if (score > highScore)  // **Nếu điểm số hiện tại lớn hơn điểm cao nhất**
            {
                highScore = score;  // **Cập nhật điểm cao nhất**
                SaveGameData();  // **Lưu dữ liệu game** (SystemManager)
            }
        }
    }

    // Hàm này được gọi khi nhân vật va chạm với một vật phẩm
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PowerUp")  // **Nếu va chạm với vật phẩm**
        {
            Debug.Log("[OnTriggerEnter2D] Nhặt vật phẩm!");
        }
    }

    // Ví dụ về hàm gọi áp dụng lực
    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force);  // **Áp dụng lực cho Rigidbody2D**
        Debug.Log("[ApplyForce] Đã áp dụng lực lên nhân vật!");
    }

    // Hàm khởi động lại game
    public void Restart()
    {
        SceneManager.LoadScene("Level1");  // **Load lại cảnh Level1**
        Time.timeScale = 1;  // **Bật lại thời gian**
        Debug.Log("[Restart] Game Restarted!");
    }

    // Hàm này được gọi khi game bị dừng (exit game)
    void OnDisable()
    {
        if (score > highScore)  // **Nếu điểm số hiện tại lớn hơn điểm cao nhất**
        {
            highScore = score;  // **Cập nhật điểm cao nhất**
            UpdateUI();  // (UIController)
            Debug.Log("[OnDisable] điểm cao.");
            SaveGameData();  // (SystemManager)
        }
    }
}
