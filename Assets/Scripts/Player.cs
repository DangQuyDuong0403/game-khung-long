using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using TMPro;


public class Player : MonoBehaviour
{
    // Khai báo các thành phần cần thiết
    Rigidbody2D rb; // **Component vật lý (Rigidbody) của nhân vật**
    Animator anim; // **Component điều khiển hoạt ảnh của nhân vật**
    public float Jump; // **Lực nhảy của nhân vật**
    public bool isJumping = false; // **Kiểm tra trạng thái nhảy của nhân vật**
    public GameObject RestartBO; // **Nút Restart khi game over**

    public int score = 0; // **Điểm số hiện tại của người chơi**
    public int highScore = 0; // **Điểm số cao nhất đã đạt được**

    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    // **Script Serialization**: Lớp để lưu trữ dữ liệu game (Serialization)
    [System.Serializable]
    private class GameData
    {
        public int highScore;  // **Điểm cao**
        public float playTime; // **Thời gian chơi**
    }

    // **Event Function**: Hàm này được gọi khi game bắt đầu
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // **Lấy component Rigidbody2D**
        anim = GetComponent<Animator>();   // **Lấy component Animator**
        LoadGameData();  // **Tải dữ liệu game đã lưu trước đó**
        UpdateUI();
        StartCoroutine(IncreaseScoreOverTime(1f));  // **Bắt đầu Coroutine để tăng điểm**
        Debug.Log("[Start] Game bắt đầu!");
    }

    // **Method in Unity**: Hàm này gọi mỗi frame, sử dụng để kiểm tra các sự kiện nhảy
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
            DeleteGameData();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            WaitAndStop(2f);
        }
    }

    // **Method in Unity**: Hàm này gọi mỗi frame vật lý, dùng để cập nhật các yếu tố vật lý
    void FixedUpdate()
    {
        // Cập nhật các lực vật lý, chẳng hạn như trọng lực
        //if (rb.linearVelocity.y < 0)
        //{
        //    anim.SetBool("IsFalling", true);  // **Bật hoạt ảnh rơi nếu đang di chuyển xuống dưới**
        //}
    }

    // **Event Function**: Hàm này gọi khi có va chạm với các đối tượng khác
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
            StartCoroutine(FlashEffectCoroutine());  // **Bắt đầu hiệu ứng nhấp nháy khi chết**
            Debug.Log("[OnCollisionEnter2D] Game Over.");
            if (score > highScore)  // **Nếu điểm số hiện tại lớn hơn điểm cao nhất**
            {
                highScore = score;  // **Cập nhật điểm cao nhất**
                SaveGameData();  // **Lưu dữ liệu game**
            }
        }
    }

    // update điểm trên giao diện
    private void UpdateUI()
    {
        scoreText.text = "Score: " + score.ToString();
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    // **Method in Unity**: Ví dụ về hàm gọi áp dụng lực
    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force);  // **Áp dụng lực cho Rigidbody2D**
        Debug.Log("[ApplyForce] Đã áp dụng lực lên nhân vật!");
    }

    

    // **Method in Unity**: Hàm tăng điểm của người chơi
    private void IncreaseScore()
    {
        score += 1;  // **Tăng điểm**
        UpdateUI();
        Debug.Log("[IncreaseScore] Điểm hiện tại: " + score);
    }


    // **Coroutine**: Coroutine này tăng điểm số mỗi khoảng thời gian
    private IEnumerator IncreaseScoreOverTime(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);  // **Chờ một khoảng thời gian delay**
            IncreaseScore();  // **Tăng điểm**
            Debug.Log("[Coroutine] Điểm tăng mỗi " + delay + " giây.");
        }
    }

    // **Coroutine**: Coroutine này tạo hiệu ứng nhấp nháy cho nhân vật khi chết
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

    // **Coroutine**: Hàm này dùng để dừng nhân vật trong một khoảng thời gian
    private IEnumerator WaitAndStop(float time)
    {
        yield return new WaitForSeconds(time);  // **Chờ thời gian trước khi dừng**
        rb.linearVelocity = Vector2.zero;  // **Dừng chuyển động**
        Debug.Log("[WaitAndStop] Dừng chuyển động sau " + time + " giây.");
    }


    // **Generic Function**: Hàm tải dữ liệu game từ file JSON
    private T LoadData<T>(string path) where T : new()
    {
        if (File.Exists(path))  // **Kiểm tra xem file có tồn tại không**
        {
            string json = File.ReadAllText(path);  // **Đọc dữ liệu từ file**
            Debug.Log("[LoadData] Tải dữ liệu từ " + path);
            return JsonUtility.FromJson<T>(json);  // **Chuyển dữ liệu JSON thành đối tượng**
        }
        else
        {
            Debug.Log("[LoadData] Không tìm thấy dữ liệu, tạo mới.");
            return new T();  // **Nếu không có dữ liệu, tạo mới đối tượng**
        }
    }

    // **Generic Function**: Hàm lưu dữ liệu game vào file JSON
    private void SaveData<T>(string path, T data)
    {
        string json = JsonUtility.ToJson(data);  // **Chuyển đối tượng thành JSON**
        File.WriteAllText(path, json);  // **Lưu dữ liệu vào file**
        Debug.Log("[SaveData] Dữ liệu đã được lưu vào " + path);
    }

    // **Script Serialization**: Hàm này lưu trữ và lưu lại dữ liệu game
    private void SaveGameData()
    {
        GameData data = new GameData();  // **Tạo đối tượng GameData**
        data.highScore = highScore;  // **Lưu điểm cao**
        data.playTime = Time.timeSinceLevelLoad;  // **Lưu thời gian chơi**
        string path = Application.persistentDataPath + "/gamedata.json";  // **Đường dẫn file**
        SaveData(path, data);  // **Lưu dữ liệu**
    }

    // **Script Serialization**: Hàm này tải dữ liệu game từ file
    private void LoadGameData()
    {
        string path = Application.persistentDataPath + "/gamedata.json";  // **Đường dẫn file**
        GameData data = LoadData<GameData>(path);  // **Tải dữ liệu từ file**
        highScore = data.highScore;  // **Cập nhật điểm cao**
        Debug.Log("[LoadGameData] Điểm cao hiện tại: " + highScore);
    }

    // **Method in Unity**: Hàm xóa dữ liệu game (reset)
    public void DeleteGameData()
    {
        string path = Application.persistentDataPath + "/gamedata.json";  // **Đường dẫn file**
        if (File.Exists(path))  // **Kiểm tra xem file có tồn tại không**
        {
            File.Delete(path);  // **Xóa file dữ liệu game**
            Debug.Log("[DeleteGameData] Dữ liệu đã bị xóa.");
        }
        else
        {
            Debug.Log("[DeleteGameData] Không tìm thấy dữ liệu để xóa.");
        }
    }

    // **Event Function**: Hàm này được gọi khi game bị dừng (exit game)
    void OnDisable()
    {
        if (score > highScore)  // **Nếu điểm số hiện tại lớn hơn điểm cao nhất**
        {
            highScore = score;  // **Cập nhật điểm cao nhất**
            UpdateUI();
            Debug.Log("[OnDisable] điểm cao.");
            SaveGameData();  // **Lưu dữ liệu**
        }
        
    }

    // **Method in Unity**: Hàm này được gọi khi nhân vật va chạm với một vật phẩm
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PowerUp")  // **Nếu va chạm với vật phẩm**
        {
            Debug.Log("[OnTriggerEnter2D] Nhặt vật phẩm!");
        }
    }

    // **Event Function**: Hàm khởi động lại game
    public void Restart()
    {
        SceneManager.LoadScene("Level1");  // **Load lại cảnh Level1**
        Time.timeScale = 1;  // **Bật lại thời gian**
        Debug.Log("[Restart] Game Restarted!");
    }

    

    

}
