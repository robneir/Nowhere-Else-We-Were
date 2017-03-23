using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Action {
    public GameManager gm;
    public string actionName;
    public Button actionButton;
    public AudioClip actionSound;

    public Action(string actionName, AudioClip actionSound)
    {
        this.actionName = actionName;
        this.actionSound = actionSound;
    }

    public virtual void DoAction()
    {
        if(actionSound != null)
        {
            gm.audioManager.Play2DSound(actionSound, 1.0f, false);
        }
        else
        {
            Debug.Log("Button action sound is null");
        }
    }

    public void RemoveActionFromPossibleActions()
    {
        gm.actionManager.currentPossibleActions.Remove(this);
        // Remove button from UI list of objects that we cant raycast through before destroying
        RectTransform rectTrans = actionButton.GetComponent<RectTransform>();
        gm.uiManager.raycastStoppingUI.Add(rectTrans);
		UnityEngine.Object.Destroy(actionButton.gameObject);
        gm.uiManager.getActionPanel().Populate();
    }
}
