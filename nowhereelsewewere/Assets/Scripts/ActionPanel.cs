using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ActionPanel : UIPanel {

    [SerializeField]
    private GameManager gm;

    public Button button;
    public Vector3 actionPanelOffsetFromPlayer;

    // Use this for initialization
    void Start ()
    {
        // Set the button panel for player options during turn to not active.
        this.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        gm.uiManager.SnapUIToCharacterWithOffset(this.gameObject, actionPanelOffsetFromPlayer);
    }

    public void Populate()
    {
        RemoveActionMenuButtons();
        // Get all possible actions from Action manager and populate the action menu.
        List<Action> possibleActions = gm.actionManager.GetAllPossibleActions();
        for (int i = 0; i < possibleActions.Count; i++)
        {
            Action action = possibleActions[i];
            Button actButton = (Button)Instantiate(button, this.transform);
            action.actionButton = actButton;
            RectTransform rectTrans = actButton.GetComponent<RectTransform>();
            gm.uiManager.raycastStoppingUI.Add(rectTrans);
            actButton.onClick.AddListener(() => action.DoAction());
            Text text = actButton.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = action.actionName;
            }
        }

        // Normalize all children UI for scaling issues.
        gm.uiManager.NormalizeChildrenScale(this.transform.gameObject);
        gm.uiManager.SnapUIToCharacterWithOffset(this.gameObject, actionPanelOffsetFromPlayer);

        this.gameObject.SetActive(true);
    }

    public void RemoveActionMenuButtons()
    {
        int childCount = this.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            // Remove button from raycast block list before destroying.
            Button button = transform.GetChild(i).gameObject.GetComponent<Button>();
            if (button != null)
            {
                RectTransform rectTrans = button.GetComponent<RectTransform>();
                gm.uiManager.raycastStoppingUI.Add(rectTrans);
                gm.uiManager.raycastStoppingUI.Remove(rectTrans);
            }
            Destroy(button.gameObject);
        }
        this.gameObject.SetActive(false);
    }

}
