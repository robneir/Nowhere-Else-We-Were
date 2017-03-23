using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryPanel : UIPanel {

    [SerializeField]
    GameManager gm;

    [SerializeField]
    [Tooltip("Gridlayout used to store all the different inventory items in organized manner.")]
    private GridLayoutGroup inventoryGrid;

    [SerializeField]
    [Tooltip("Prefab used to add the 2D sprite that corresponds to an item to the inventory grid.")]
    private InventoryItem itemUIPrefab;

    [SerializeField]
    [Tooltip("Main panel that holds weapon information. This contains the Weapon Info Grid Layout.")]
    private GameObject weaponInfoPanel;

    [SerializeField]
    [Tooltip("VerticalLayout that holds all information about weapon currently highlighted in inventory.")]
    private VerticalLayoutGroup weaponInfoVerticalLayout;

    [SerializeField]
    private Text statText;

    [SerializeField]
    private Text weaponStatText;

    private RectTransform previousHighlightedWeaponRect;

    // Use this for initialization
    void Start ()
    {
        // Make sure panel is not visible in the start of game.
        this.gameObject.SetActive(false);

        // Add to UI that can't be raycasted through
        RectTransform rectTrans = this.GetComponent<RectTransform>();
        gm.uiManager.raycastStoppingUI.Add(rectTrans);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Shows the weapon info panel if the player hovers over an item in weapon inventory.
        ShowWeaponInfoPanelOnHover();
    }

    private void ShowWeaponInfoPanelOnHover()
    {
        // Check every frame to see if mouse is highlighting over inventory slot.
        int childs = inventoryGrid.transform.childCount;
        RectTransform selectedRect = null;
        bool somethingContainedPoint = false;
        for (int i = 0; i < childs; i++)
        {
            selectedRect = inventoryGrid.transform.GetChild(i).GetComponent<RectTransform>();
            bool contains = RectTransformUtility.RectangleContainsScreenPoint(selectedRect, Input.mousePosition);
            if (somethingContainedPoint == false)
            {
                somethingContainedPoint = contains;
            }
            // Get the weapon that is associated with this element
            if (contains && selectedRect != previousHighlightedWeaponRect)
            {
                // Populate weapon panel if corresponds to a weapon
                WorldCharacter currCharacter = gm.selectionManager.GetCurrentSelectedCharacter();
                if (currCharacter != null)
                {
                    // Check to make sure that the selection is within range of the weapons we have.
                    List<Weapon> currWeapons = currCharacter.getInventory().Weapons;
                    if (currWeapons.Count > i)
                    {
                        // Destroy all children before adding the wewapon stat panel.
                        int children = weaponInfoVerticalLayout.transform.childCount;
                        for (int k = children - 1; k >= 0; k--)
                        {
                            GameObject.Destroy(weaponInfoVerticalLayout.transform.GetChild(k).gameObject);
                        }
                        Weapon weapon = currWeapons[i];
                        Text stat = (Text)Instantiate(weaponStatText, weaponInfoVerticalLayout.transform);
                        stat.text = weapon.Name;
                        stat = (Text)Instantiate(weaponStatText, weaponInfoVerticalLayout.transform);
                        stat.text = Constants.instance.DAMAGE + ": " + weapon.Damage;
                        stat = (Text)Instantiate(weaponStatText, weaponInfoVerticalLayout.transform);
                        stat.text = Constants.instance.CRIT + ": " + weapon.Crit;
                        stat = (Text)Instantiate(weaponStatText, weaponInfoVerticalLayout.transform);
                        stat.text = Constants.instance.HIT + ": " + weapon.Hit;
                        stat = (Text)Instantiate(weaponStatText, weaponInfoVerticalLayout.transform);
                        stat.text = Constants.instance.RANGE + ": ";
                        for (int j = 0; j < weapon.GetRange().Length; j++)
                        {
                            stat.text += weapon.GetRange()[j] + "/";
                        }
                        previousHighlightedWeaponRect = selectedRect;

                        // Normalize the children's scale.
                        gm.uiManager.NormalizeChildrenScale(weaponInfoVerticalLayout.gameObject);

                        // Show the weapon panel
                        weaponInfoPanel.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        if (somethingContainedPoint == false)
        {
            previousHighlightedWeaponRect = null;
            weaponInfoPanel.gameObject.SetActive(false);
        }
    }

    public void Populate(Inventory inventory)
    {
        if (itemUIPrefab == null)
        {
            Debug.Log("Inventory item prefab null");
            return;
        }

        // Clear all children of inventory panel.
        int childs = inventoryGrid.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.Destroy(inventoryGrid.transform.GetChild(i).gameObject);
        }

        // Add all weapons to the inventory panel.
        for (uint i = 0; i < inventory.weaponLimit; i++)
        {
            InventoryItem item = null;
            // Check to make sure we did not go out of bounds of weapon list.
            if (i < inventory.Weapons.Count)
            {
                Weapon weapon = inventory.Weapons[(int)i];
                if (weapon != null)
                {
                    // Add weapon to the inventory
                    Sprite weaponPortrait = weapon.portrait;
                    if (weaponPortrait != null)
                    {
                        item = (InventoryItem)Instantiate(itemUIPrefab, inventoryGrid.transform);
                        if (item != null)
                        {
                            // Set the item's button's inventory index so it knows what item the button corresponds to.
                            item.Init(i, inventory);

                            Image portrait = item.portrait.GetComponent<Image>();
                            if (portrait != null)
                            {
                                portrait.gameObject.SetActive(true);
                                portrait.sprite = weaponPortrait;
                            }

                            // If this is the equipped weapon show specific background color.
                            Image background = item.background.GetComponent<Image>();
                            if (background != null)
                            {
                                if (weapon == inventory.getEquippedWeapon())
                                {
                                    background.color = Constants.instance.equippedBackgroundColor;
                                }
                                else
                                {
                                    background.color = Constants.instance.notEquippedBackgroundColor;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Add empty item to inventory
                item = (InventoryItem)Instantiate(itemUIPrefab, inventoryGrid.transform);
                if (item != null)
                {
                    Image portrait = item.portrait.GetComponent<Image>();
                    if (portrait != null)
                    {
                        portrait.gameObject.SetActive(false);
                    }
                }
            }
        }

        // This is needed or else the scale of the element will look small for some weird reason...
        gm.uiManager.NormalizeChildrenScale(inventoryGrid.gameObject);

        // Make sure the panel is visible after we populate it.
        this.gameObject.SetActive(true);
    }
}
