using UnityEngine;
using System.IO;
using System.Text;  // 放在文件顶部

public class ClueTrigger : MonoBehaviour
{
    public PointingTaskUI taskUI;           // 控制提示UI
    public GameObject pointerIndicator;     // 场景里的箭头
    private Vector3? lastCoinPosition = null;  // 上一个金币的位置
    private bool awaitingUserResponse = false; // 是否正在等待用户指认
    private Vector3 targetDirection;           // 真实方向
    private float? lastErrorAngle = null;      // 保存误差

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("吃到金币：" + other.name);

            if (lastCoinPosition.HasValue)
            {
                // 要求用户指认上一个金币
                targetDirection = lastCoinPosition.Value - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                if (pointerIndicator != null)
                    pointerIndicator.SetActive(true);

                taskUI?.ShowTaskHint("Please point using mouse and press space.");

                awaitingUserResponse = true;
            }

            // 更新上一个金币位置
            lastCoinPosition = other.transform.position;

            // 销毁金币
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        if (awaitingUserResponse)
        {
            // 用户按下空格表示确认
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Vector3 userDir = Camera.main.transform.forward;
                userDir.y = 0;
                userDir.Normalize();

                float angleError = Vector3.Angle(targetDirection, userDir);
                lastErrorAngle = angleError;

                Debug.Log($"用户指认误差: {angleError}°");

                SaveError(angleError);
                awaitingUserResponse = false;

                if (pointerIndicator != null)
                    pointerIndicator.SetActive(false);

                taskUI?.HideTaskHint();
            }
        }
    }

    void SaveError(float angle)
    {
        string filePath = Path.Combine(Application.dataPath, "../result.csv"); // 保存到项目根目录

        bool fileExists = File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            if (!fileExists)
            {
                // 写入表头
                writer.WriteLine("Timestamp,ErrorAngle");
            }

            string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"{timestamp},{angle:F2}");
        }

        Debug.Log($"保存误差 {angle:F2}° 到 {filePath}");
    }
}
