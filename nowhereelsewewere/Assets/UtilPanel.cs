using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UtilPanel : UIPanel {

    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private Button nextTurnButton;
    [SerializeField]
    private AudioClip nextTurnButtonSound;

    [SerializeField]
    private Button cyclePlayerButton;
    [SerializeField]
    private AudioClip cyclePlayerButtonSound;

    // Use this for initialization
    void Start()
    {
        // Bind buttons to certain actions
        nextTurnButton.onClick.AddListener(() => nextTurnButtonClicked());
        cyclePlayerButton.onClick.AddListener(() => cyclePlayerButtonClicked());

        // Disable both buttons to start
        nextTurnButton.enabled = false;
        cyclePlayerButton.enabled = false;

        RectTransform rectTrans = this.GetComponent<RectTransform>();
        if(gm != null)
        {
            gm.uiManager.raycastStoppingUI.Add(rectTrans);
        }
    }

    public void Update()
    {
        // Put turn phase info into variables temporarily for checks.
        TurnManager.PlayerTurnPhase currPlayerTurnPhase = gm.turnManager.getPlayerTurnPhase();
        TurnManager.TurnPhase currTurnPhase = gm.turnManager.getCurrentTurnPhase();
        // CHECK TO SEE WHAT BUTTONS SHOULD BE ON SCREEN.
        if (currTurnPhase == TurnManager.TurnPhase.Player && 
            gm.battleManager.InCombat == false &&
            currPlayerTurnPhase == TurnManager.PlayerTurnPhase.SelectPlayer)
        {
            flyButtonInScreen(nextTurnButton);
        }
        else
        {
            flyButtonOutScreen(nextTurnButton);
        }

        if (currTurnPhase == TurnManager.TurnPhase.Player &&
            gm.battleManager.InCombat == false &&
           (currPlayerTurnPhase == TurnManager.PlayerTurnPhase.SelectPlayer || currPlayerTurnPhase == TurnManager.PlayerTurnPhase.Move ))
        { 
            flyButtonInScreen(cyclePlayerButton);
        }
        else
        {
            flyButtonOutScreen(cyclePlayerButton);
        }
    }

    public void nextTurnButtonClicked()
    {
        gm.turnManager.SkipPlayerTurn();
        if (nextTurnButtonSound != null)
        {
            gm.audioManager.Play2DSound(nextTurnButtonSound, 1.0f, false);
        }
    }

    public void cyclePlayerButtonClicked()
    {
        gm.turnManager.CycleToNextPlayableCharacter();
        if (cyclePlayerButtonSound != null)
        {
            gm.audioManager.Play2DSound(cyclePlayerButtonSound, 1.0f, false);
        }
    }

    public void flyButtonInScreen(Button utilButton)
    {
        string parameter = "isActivated";
        if (GetAnimatorBool(utilButton, parameter) == false)
        {
            // Reenable the button.
            utilButton.enabled = true;

            // Make it fly into the screen.
            SetAnimatorBool(utilButton, parameter, true);
        }
    }

    public void flyButtonOutScreen(Button utilButton)
    {
        string parameter = "isActivated";
        if(GetAnimatorBool(utilButton, parameter) == true)
        {
            // Disable the button.
            utilButton.enabled = false;

            // Make it fly out of screen.
            SetAnimatorBool(utilButton, parameter, false);
        }
    }

    private void SetAnimatorBool(Button utilButton, string boolName, bool value)
    {
        if (utilButton != null)
        {
            Animator animator = utilButton.GetComponentInParent<Animator>();
            if (animator != null)
            {
                animator.SetBool(boolName, value);
            }
            else
            {
                Debug.Log("Animator for button is null.");
            }
        }
    }

    private bool GetAnimatorBool(Button utilButton, string boolName)
    {
        if (utilButton != null)
        {
            Animator animator = utilButton.GetComponentInParent<Animator>();
            if (animator != null)
            {
                return animator.GetBool(boolName);
            }
            else
            {
                Debug.Log("Animator for button is null.");
            }
        }
        return false;
    }
}