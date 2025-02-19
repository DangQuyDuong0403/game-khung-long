using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public partial class Player : MonoBehaviour
{
    // Lớp để lưu trữ dữ liệu game (Serialization)
    [System.Serializable]
    private class GameData
    {
        public int highScore;  // **Điểm cao**
        public float playTime; // **Thời gian chơi**
    }

    // Hàm tải dữ liệu game từ file JSON
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

    // Hàm lưu dữ liệu game vào file JSON
    private void SaveData<T>(string path, T data)
    {
        string json = JsonUtility.ToJson(data);  // **Chuyển đối tượng thành JSON**
        File.WriteAllText(path, json);  // **Lưu dữ liệu vào file**
        Debug.Log("[SaveData] Dữ liệu đã được lưu vào " + path);
    }

    // Hàm này lưu trữ và lưu lại dữ liệu game
    private void SaveGameData()
    {
        GameData data = new GameData();  // **Tạo đối tượng GameData**
        data.highScore = highScore;  // **Lưu điểm cao**
        data.playTime = Time.timeSinceLevelLoad;  // **Lưu thời gian chơi**
        string path = Application.persistentDataPath + "/gamedata.json";  // **Đường dẫn file**
        SaveData(path, data);  // **Lưu dữ liệu**
    }

    // Hàm này tải dữ liệu game từ file
    private void LoadGameData()
    {
        string path = Application.persistentDataPath + "/gamedata.json";  // **Đường dẫn file**
        GameData data = LoadData<GameData>(path);  // **Tải dữ liệu từ file**
        highScore = data.highScore;  // **Cập nhật điểm cao**
        Debug.Log("[LoadGameData] Điểm cao hiện tại: " + highScore);
    }

    // Hàm xóa dữ liệu game (reset)
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

    // Hàm này dùng để dừng nhân vật trong một khoảng thời gian
    private IEnumerator WaitAndStop(float time)
    {
        yield return new WaitForSeconds(time);  // **Chờ thời gian trước khi dừng**
        rb.linearVelocity = Vector2.zero;  // **Dừng chuyển động**
        Debug.Log("[WaitAndStop] Dừng chuyển động sau " + time + " giây.");
    }
}
