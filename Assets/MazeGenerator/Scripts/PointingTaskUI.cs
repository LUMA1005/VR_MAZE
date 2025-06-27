using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class PointingTaskUI : MonoBehaviour
{
    public TMP_Text hintText;


    public void ShowTaskHint(string message)
    {
        Debug.Log("ShowTaskHint called with: " + message);
        if (hintText != null)
        {
            hintText.text = "TEST";
            hintText.enabled = true;
        }
    }


    public void HideTaskHint()
    {
        if (hintText != null)
        {
            hintText.enabled = false;
        }
    }
}
