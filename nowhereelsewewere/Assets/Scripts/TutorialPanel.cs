using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialPanel : UIPanel {

    [Tooltip("This tip that will be displayed in current state of tutorial.")]
    public Text tip;

    public Image mouseSprite;
    public Image leftMouseSprite;
    public Image middleMouseSprite;
    public Image rightMouseSprite;

    public GameObject arrowsParent;
    public Image rightArrowSprite;
    public Image leftArrowSprite;
    public Image upArrowSprite;
    public Image downArrowSprite;

	public Image lumiusExampleImage;
	public Image lumiusKeysImage;
    public Image multipleCharacterImage;
    public Image characterPortrait;
    public Sprite skyePortrait;
    public Sprite roderickPortrait;
    public Sprite rowanPortrait;

    public Button nextButton;
    public Button prevButton;
    public Button hideButton;

    public Button helpButton;

    private GameManager gm;

    void Awake()
    {
        HideTutorial();
    }

    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        RectTransform rectTrans = this.GetComponent<RectTransform>();
        if (gm != null)
        {
            gm.uiManager.raycastStoppingUI.Add(rectTrans);
        }
    }

    public void HideTutorial()
    {
        this.gameObject.SetActive(false);
        arrowsParent.SetActive(false);
        mouseSprite.gameObject.SetActive(false);
        helpButton.gameObject.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(false);
        characterPortrait.gameObject.SetActive(false);
    }

    public void ShowStartTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);

        arrowsParent.SetActive(false);

        tip.text = "Press \"Got it\" when you have read and completed each tutorial step.";
    }

    public void ShowCameraPanTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);

        arrowsParent.SetActive(true);

        tip.text = "Hold left click on empty space and drag the mouse to pan over the world. WASD is also an option for this effect.";
    }

    public void ShowCameraRotateTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(true);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);

        arrowsParent.SetActive(true);

        tip.text = "Hold middle click and drag the mouse to rotate the camera around the world. You can reset the camera to original orientation with 'R'";
    }

    public void ShowCameraZoomTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(true);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);

        arrowsParent.SetActive(false);

        tip.text = "Scroll the middle mouse wheel to zoom into and out of the world.";
    }

    public void ShowCharacterSelectTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);

        arrowsParent.SetActive(false);

        tip.text = "Ok! Now left click on a character to select them. Your characters have blue arrow above their heads while enemies have red.";
    }

    public void ShowMoveTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);

        arrowsParent.SetActive(false);

        tip.text = "With the character selected you are able to choose what actions that character will perform this turn. While your character is selected left click on a blue tile to bring up the actions menu.";
    }


    public void ShowUndoTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(true);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);


        tip.text = "To revert any non-final action, right click anywhere on the screen. Lets revert the selection we just made. Right click for me.";
    }


    public void ShowUndo2Tutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(true);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);


        tip.text = "You will now see that you are in the movement stage again. If you right click another time we will deselect the character. try that.";
    }

    public void ShowWaitTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(false);


        tip.text = "Now select a character and move them toward an enemy by pressing \"Wait\", not \"Attack\" since no enemy is within range.";
    }

    public void ShowAttackTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(false);

        tip.text = "Now that the enemy phase is over, select Skye and select a cube next to one of the raiders If you are within range. Then press \"Attack\". left click again on the enemy you want to attack!";
    }


    public void ShowAttack2Tutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(true);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(false);

        tip.text = "The panel that popped up will show you your character's and enemy's stats. From here you can decide to attack by pressing the button or reverting with right click as usual.";
    }

    public void ShowKillTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(true);

        tip.text = "Now kill the raiders.";
    }


    // This will restart the tutorial to the player can see tutorial again if they missed something.
    public void ShowHelpButton()
    {
        this.gameObject.SetActive(false);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);

        // Show the help button.
        helpButton.gameObject.SetActive(true);
    }

    public void HideHelpButton()
    {
        helpButton.gameObject.SetActive(false);
    }

    public void ShowMultipleCharacterTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(true);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(true);
        multipleCharacterImage.gameObject.SetActive(true);
        characterPortrait.gameObject.SetActive(false);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "Now you may notice you have more than one character you will be controlling! Say hello to Skye, Roderick, and Rowan.";
    }

    public void ShowCharacterTypesTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);
        characterPortrait.gameObject.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(true);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "Each character you have has a character type and weapon types he or she is limited to. Here are the basics you need to know about each character before you get started...";
    }

    public void ShowCharacterStatsTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);
        characterPortrait.gameObject.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(true);
        lumiusExampleImage.gameObject.SetActive(false);
        lumiusKeysImage.gameObject.SetActive(false);

        tip.text = "To examine character stats just select a character and examine the character stats panel on the bottom right of the screen.";
    }

    public void ShowRoderickTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(false);
        arrowsParent.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(false);
        characterPortrait.sprite = roderickPortrait;
        characterPortrait.gameObject.SetActive(true);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "Roderick is a knight and may only use piercers and thrown weapons.";
    }

    public void ShowSkyeTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(false);
        characterPortrait.sprite = skyePortrait;
        characterPortrait.gameObject.SetActive(true);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "Skye is a myrmidon and she may use slashers.";
    }

    public void ShowRowanTutorial()
    {
        this.gameObject.SetActive(true);
        mouseSprite.gameObject.SetActive(false);
        middleMouseSprite.gameObject.SetActive(false);
        rightMouseSprite.gameObject.SetActive(false);
        leftMouseSprite.gameObject.SetActive(true);
        arrowsParent.SetActive(false);
        multipleCharacterImage.gameObject.SetActive(false);
        characterPortrait.sprite = rowanPortrait;
        characterPortrait.gameObject.SetActive(true);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "Rowan is a mage and wields nature magic to crush his opponents.";
    }

    public void ShowLumiusTutorial()
	{
		this.gameObject.SetActive(true);
		mouseSprite.gameObject.SetActive(false);
		middleMouseSprite.gameObject.SetActive(false);
		rightMouseSprite.gameObject.SetActive(false);
		leftMouseSprite.gameObject.SetActive(false);
		arrowsParent.SetActive(false);
		multipleCharacterImage.gameObject.SetActive(false);
		characterPortrait.gameObject.SetActive(false);
		lumiusExampleImage.gameObject.SetActive(true);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "In addition, you can now use Lumius, magical fuel that can boost a character's abilities in combat. This bonus is in effect on the turn it is applied and the subsequent enemy's phase.";
	}

	public void ExplainLumiusControlsTutorial()
	{
		this.gameObject.SetActive(true);
		mouseSprite.gameObject.SetActive(false);
		middleMouseSprite.gameObject.SetActive(false);
		rightMouseSprite.gameObject.SetActive(false);
		leftMouseSprite.gameObject.SetActive(false);
		arrowsParent.SetActive(false);
		multipleCharacterImage.gameObject.SetActive(false);
		characterPortrait.gameObject.SetActive(false);
		lumiusExampleImage.gameObject.SetActive(false);
		lumiusKeysImage.gameObject.SetActive(true);

		tip.text = "You can apply Lumius to a selected character ONLY BEFORE they have moved by pressing the appropriate key, as shown below. These keys correspond to the Lumius panel on the bottom left corner of your screen.";
	}

	public void ExplainLumiusEffectsTutorial()
	{
		this.gameObject.SetActive(true);
		mouseSprite.gameObject.SetActive(false);
		middleMouseSprite.gameObject.SetActive(false);
		rightMouseSprite.gameObject.SetActive(false);
		leftMouseSprite.gameObject.SetActive(false);
		arrowsParent.SetActive(false);
		multipleCharacterImage.gameObject.SetActive(false);
		characterPortrait.gameObject.SetActive(false);
		lumiusExampleImage.gameObject.SetActive(true);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "There are 5 main types of Lumius: Increased Damage, Increased Hit Chance, Decreased Enemy Hit Chance, Decreased Enemy Damage, and Increased Movement Range.";
	}

	public void ExplainLumiusExtraTutorial()
	{
		this.gameObject.SetActive(true);
		mouseSprite.gameObject.SetActive(false);
		middleMouseSprite.gameObject.SetActive(false);
		rightMouseSprite.gameObject.SetActive(false);
		leftMouseSprite.gameObject.SetActive(false);
		arrowsParent.SetActive(false);
		multipleCharacterImage.gameObject.SetActive(false);
		characterPortrait.gameObject.SetActive(false);
		lumiusExampleImage.gameObject.SetActive(true);
		lumiusKeysImage.gameObject.SetActive(false);

		tip.text = "You only have a limited number of Lumius per level (as shown screen bottom left), so use them wisely! They can mean the difference between Victory and Defeat!";
	}
}
