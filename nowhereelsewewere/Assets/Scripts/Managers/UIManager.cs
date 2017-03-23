using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

    public GameManager gm;

    [SerializeField]
    private InventoryPanel inventoryPanel;
    [SerializeField]
    private AttackPanel attackPanel;
    [SerializeField]
    private CharacterPanel characterPanel;
    [SerializeField]
    private ActionPanel actionPanel;
    [SerializeField]
    private PauseMenu pauseMenu;
    [SerializeField]
    private LumiusPanel lumiusPanel;
	[SerializeField]
	private CubePanel cubePanel;
    [SerializeField]
    private UtilPanel utilPanel;
    [SerializeField]
    private TutorialPanel tutorialPanel;
    public PhaseIndicator phaseIndicator;

    // List of all UI that we don't want raycast to be able to go through.
    [HideInInspector]
    public List<RectTransform> raycastStoppingUI;

    public void SnapUIToCharacterWithOffset(GameObject obj, Vector3 offset)
    {
        WorldCharacter currentSelectedCharacter = gm.selectionManager.GetCurrentSelectedCharacter();
        if (currentSelectedCharacter != null)
        {
            obj.transform.position = Camera.main.WorldToScreenPoint(currentSelectedCharacter.GetFutureCube().transform.position + currentSelectedCharacter.GetFutureCube().GetCubeOffset()) + offset;
        }
    }

    public InventoryPanel getInventoryPanel()
    {
        return inventoryPanel;
    }

    public AttackPanel getAttackPanel()
    {
        return attackPanel;
    }

    public CharacterPanel getCharacterPanel()
    {
        return characterPanel;
    }

    public ActionPanel getActionPanel()
    {
        return actionPanel;
    }

	public CubePanel getCubePanel()
	{
		return cubePanel;
	}

    public void ShowPauseMenu()
    {
        pauseMenu.gameObject.SetActive(true);
        lumiusPanel.gameObject.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        characterPanel.gameObject.SetActive(false);
        utilPanel.gameObject.SetActive(false);
        cubePanel.gameObject.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        HideAllHealthBars();
    }

    public void HidePauseMenu()
    {
        pauseMenu.gameObject.SetActive(false);

        if(cubePanel.isInLevel)
        {
            cubePanel.gameObject.SetActive(true);
        }
        if (lumiusPanel.isInLevel)
        {
            lumiusPanel.gameObject.SetActive(true);
        }
        if (utilPanel.isInLevel)
        {
            utilPanel.gameObject.SetActive(true);
        }

        tutorialPanel.gameObject.SetActive(false);

        if (gm.selectionManager.GetCurrentSelectedCharacter() != null)
        {
            if(inventoryPanel.isInLevel)
            {
                inventoryPanel.gameObject.SetActive(true);
            }
            if (characterPanel.isInLevel)
            {
                characterPanel.gameObject.SetActive(true);
            }
        }
        ShowAllHealthBars();
    }

    public void ShowAllHealthBars()
    {
        List<WorldCharacter> playableCharacters = gm.playableCharacters;
        List<WorldCharacter> enemyCharacters = gm.enemyCharacters;
        foreach (WorldCharacter c in playableCharacters)
        {
            c.getStatusBar().gameObject.SetActive(true);
        }
        foreach (WorldCharacter c in enemyCharacters)
        {
            c.getStatusBar().gameObject.SetActive(true);
        }
    }

    public void HideAllHealthBars()
    {
        List<WorldCharacter> playableCharacters = gm.playableCharacters;
        List<WorldCharacter> enemyCharacters = gm.enemyCharacters;
        foreach(WorldCharacter c in playableCharacters)
        {
            c.getStatusBar().gameObject.SetActive(false);
        }
        foreach (WorldCharacter c in enemyCharacters)
        {
            c.getStatusBar().gameObject.SetActive(false);
        }
    }

    // Checks all UI elements that we don't want raycasts to travel through. If this point we give this function has a UI element at that location then reutrn true.
    // This will tell the whoever called this function if the point was on a UI element specified in raycastStoppingUI list.
    public bool CheckUIContainsPoint(Vector2 point)
    {
        foreach(RectTransform rectTrans in raycastStoppingUI)
        {
            if(rectTrans != null)
            {
                bool contains = RectTransformUtility.RectangleContainsScreenPoint(rectTrans, point);
                if (contains && rectTrans.gameObject.activeSelf)
                {
                    return true;
                }
            }
            else
            {
                //Debug.Log("Rect transform null on UI that shouldnt' be allowed to raycast through.");
            }
        }
        return false;
    }

    public void NormalizeChildrenScale(GameObject go)
    {
        // Make sure the scale of all the children is correct cause Unity is messed up.
        int children = go.transform.childCount;
        for (int k = children - 1; k >= 0; k--)
        {
            GameObject newlyMadeStat = go.transform.GetChild(k).gameObject;
            if (newlyMadeStat != null)
            {
                newlyMadeStat.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
