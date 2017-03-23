using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {

    [Tooltip("The portrait for this specific item in inventory")]
    public Image portrait;

    [Tooltip("The background for the item in the inventory")]
    public Image background;

    [SerializeField] // Button that needs to be mapped to inventory item.
    private Button button;
    [HideInInspector] // Inventory index to map the button above to specific inventory item when clicked.
    public uint inventoryIndex;

    private Inventory inventory;

    void Start()
    {
        button.onClick.AddListener(() => OnButtonClick());
    }

    public void Init(uint inventoryIndex, Inventory inventory)
    {
        this.inventory = inventory;
        this.inventoryIndex = inventoryIndex;
    }

    public void OnButtonClick()
    {
        Debug.Log("Clicked");
        if(inventory != null)
        {
            inventory.equipWeapon(inventoryIndex);
        }
    }
}
