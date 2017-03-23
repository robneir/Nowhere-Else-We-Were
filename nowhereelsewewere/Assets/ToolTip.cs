using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolTip : MonoBehaviour {

    public void setToolTipText(string tipText)
    {
        Text textComp = GetComponentInChildren<Text>();
        if (textComp != null)
        {
            textComp.text = tipText;
        }
        else
        {
            Debug.Log("No text component on tool tip.");
        }
    }
}
