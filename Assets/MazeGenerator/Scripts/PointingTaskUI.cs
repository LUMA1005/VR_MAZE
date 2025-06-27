using TMPro;
using UnityEngine;

public class PointingTaskUI : MonoBehaviour
{
    public TextMeshProUGUI taskHintText; // 拖入你Canvas中的文本UI

    public void ShowTaskHint()
    {
        if (taskHintText != null)
        {
            taskHintText.text = $"请你指向起点位置";
            taskHintText.gameObject.SetActive(true);
        }
    }

    public void HideTaskHint()
    {
        if (taskHintText != null)
        {
            taskHintText.gameObject.SetActive(false);
        }
    }
}
