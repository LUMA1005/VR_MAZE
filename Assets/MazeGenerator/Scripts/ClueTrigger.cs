using UnityEngine;

public class ClueTrigger : MonoBehaviour
{
    public PointingTaskUI taskUI; // 引用 UI 控制脚本
    public GameObject pointerIndicator; // 拖拽指向箭头对象


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log("碰到了金币：" + other.name);
            Destroy(other.gameObject);

            if (taskUI != null)
            {
                ShowPointer(true);
                taskUI.ShowTaskHint();
            }
            else
            {
                Debug.LogWarning("taskUI 未绑定！");
            }
        }

    }
    
    public void ShowPointer(bool show)
    {
        if(pointerIndicator != null)
            pointerIndicator.SetActive(show);
    }

}
