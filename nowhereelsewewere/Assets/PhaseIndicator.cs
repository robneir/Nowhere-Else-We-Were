using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PhaseIndicator : MonoBehaviour {

    private GameManager gm;

    [SerializeField]
    private List<Image> mPhaseNodes;

    [SerializeField]
    private Image mCurrentPhaseNode;

    private Image bar;
    private RectTransform barRectTransform;

    private float mtargetFillAmount;

    private int mCurrentPhaseIndex;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
        bar = GetComponent<Image>();
        mCurrentPhaseIndex = 0;
        mtargetFillAmount = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        if(gm.turnManager.getCurrentTurnPhase() == TurnManager.TurnPhase.Enemy)
        {
            this.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.transform.parent.gameObject.SetActive(true);
            // Figure out what stage we are currently in to display.
            if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.SelectPlayer)
            {
                mtargetFillAmount = 0.0f;
                SetImageAlpha(1.0f);
                mCurrentPhaseIndex = 0;
            }
            else if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.Move ||
                gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.MoveBack)
            {
                mtargetFillAmount = 1.0f / 2.0f;
                SetImageAlpha(1.0f);
                mCurrentPhaseIndex = 1;
            }
            else if (gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.PreAttack ||
                gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.Attack ||
                gm.turnManager.getPlayerTurnPhase() == TurnManager.PlayerTurnPhase.ChoosePhase)
            {
                mtargetFillAmount = 1.0f;
                SetImageAlpha(1.0f);
                mCurrentPhaseIndex = 2;
            }

            // Lerp current phase node to correct phase.
            LerpCurrentPhaseNode();
            // Lerp between current and target fill amount for smooth effect.
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, mtargetFillAmount, 0.1f);
            SetImageAlpha(Mathf.Lerp(bar.color.a, 0.0f, 0.05f));
        }
    }

    void LerpCurrentPhaseNode()
    {
        if(mCurrentPhaseIndex >= mPhaseNodes.Count)
        {
            Debug.Log("Trying to access element of list that does not exist");
            return;
        }

        Image currentPhaseNode = mPhaseNodes[mCurrentPhaseIndex];
        if(currentPhaseNode != null)
        {
            mCurrentPhaseNode.transform.position = Vector3.Lerp(mCurrentPhaseNode.transform.position,
                currentPhaseNode.transform.position, 0.1f);
        }
    }

    void SetImageAlpha(float alpha)
    {
        for(int i=0; i<mPhaseNodes.Count; i++)
        {
            Image imageComp = mPhaseNodes[i].GetComponent<Image>();
            if(imageComp != null)
            {
                imageComp.color = new Color(imageComp.color.r, imageComp.color.g, imageComp.color.b, alpha);
            }
        }
        bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, alpha);
    }
}
