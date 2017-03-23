using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LumiusPanel : UIPanel {

    [SerializeField]
    private Button damageButton;
    [SerializeField]
    private Button hitButton;
    [SerializeField]
    private Button dodgeButton;
    [SerializeField]
    private Button damageRedButton;
    [SerializeField]
    private Button moveButton;

    private LumiusManager lumiusManager;

    private GameManager gm;
    
	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
        lumiusManager = GameObject.FindObjectOfType<LumiusManager>();
        if (lumiusManager == null)
        {
            Debug.Log("Couldn't find lumius manager so it is null in lumius panel UI class.");
        }
        damageButton.onClick.AddListener(()=>damageButtonClicked());
        hitButton.onClick.AddListener(() => hitButtonClicked());
        dodgeButton.onClick.AddListener(() => dodgeButtonClicked());
        damageRedButton.onClick.AddListener(() => damageRedButtonClicked());
        moveButton.onClick.AddListener(() => moveButtonClicked());

        // Add to UI that can't be raycasted through
        RectTransform rectTrans = this.GetComponent<RectTransform>();
        if(gm != null)
        {
            gm.uiManager.raycastStoppingUI.Add(rectTrans);
        }
    }
	
    private void damageButtonClicked()
    {
        if(lumiusManager)
        {
            lumiusManager.ApplyBuff(LumiusManager.LumiusStats.Damage);
        }
    }

    private void hitButtonClicked()
    {
        if (lumiusManager)
        {
            lumiusManager.ApplyBuff(LumiusManager.LumiusStats.Hit);
        }
    }

    private void dodgeButtonClicked()
    {
        if (lumiusManager)
        {
            lumiusManager.ApplyBuff(LumiusManager.LumiusStats.Dodge);
        }
    }

    private void damageRedButtonClicked()
    {
        if (lumiusManager)
        {
            lumiusManager.ApplyBuff(LumiusManager.LumiusStats.DamageReduction);
        }
    }

    private void moveButtonClicked()
    {
        if (lumiusManager)
        {
            lumiusManager.ApplyBuff(LumiusManager.LumiusStats.Movement);
        }
    }

}
