using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TutorialManager : MonoBehaviour {

    [SerializeField]
    private GameManager gm;

    [SerializeField]
    private TutorialPanel tutorialPanel;

    delegate void DelegateShowTutorial();

    List<DelegateShowTutorial> tutorialDelegates = new List<DelegateShowTutorial>();

    private enum TutorialType
    {
        BasicsTutorial,
        AdvancedTutorial
    }
    [SerializeField]
    private TutorialType tutorialType = TutorialType.BasicsTutorial;

    int tutorialIndex;

	// Use this for initialization
	void Start ()
    {
        if(tutorialType == TutorialType.BasicsTutorial)
        {
            tutorialDelegates.Add(tutorialPanel.ShowStartTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowCameraPanTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowCameraZoomTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowCameraRotateTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowCharacterSelectTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowMoveTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowUndoTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowUndo2Tutorial);
            tutorialDelegates.Add(tutorialPanel.ShowWaitTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowAttackTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowAttack2Tutorial);
            tutorialDelegates.Add(tutorialPanel.ShowKillTutorial);
        }
        else if(tutorialType == TutorialType.AdvancedTutorial)
        {
            tutorialDelegates.Add(tutorialPanel.ShowMultipleCharacterTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowCharacterStatsTutorial);
            tutorialDelegates.Add(tutorialPanel.ShowLumiusTutorial);
			tutorialDelegates.Add(tutorialPanel.ExplainLumiusControlsTutorial);
			tutorialDelegates.Add(tutorialPanel.ExplainLumiusEffectsTutorial);
			tutorialDelegates.Add(tutorialPanel.ExplainLumiusExtraTutorial);
		}

        // tie hide button to hiding tutorial panel
        tutorialPanel.hideButton.onClick.AddListener(() => HideTutorialPanel());

        tutorialPanel.nextButton.onClick.AddListener(() => GoToNextTutorialMenu());
        tutorialPanel.prevButton.onClick.AddListener(() => GoToPrevTutorialMenu());
        tutorialPanel.helpButton.onClick.AddListener(() => GoToNextTutorialMenu());
        tutorialPanel.helpButton.onClick.AddListener(() => tutorialPanel.HideHelpButton());
        tutorialIndex = -1;
        // Get tutorial kicked off. This shouldn't really be here but JANK. like this wont play the sound of success on button click so YES I FUCKIN JANKED IT SO WAT! ;)
        if (tutorialDelegates.Count > 0)
        {
            tutorialIndex++;
            tutorialDelegates[tutorialIndex].Invoke();
        }

        tutorialPanel.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void GoToPrevTutorialMenu()
    {
        if (tutorialIndex > 0)
        {
            tutorialIndex--;
            tutorialDelegates[tutorialIndex].Invoke();
            gm.audioManager.Play2DSound(gm.audioManager.uiDownwardSound, .5f, false);
        }
        else
        {
            gm.audioManager.Play2DSound(gm.audioManager.uiFailSound, .5f, false);   
        }
    }

    public void GoToNextTutorialMenu()
    {
        if(tutorialIndex < tutorialDelegates.Count-1)
        {
            tutorialIndex++;
            tutorialDelegates[tutorialIndex].Invoke();
            gm.audioManager.Play2DSound(gm.audioManager.uiDownwardSound, .5f, false);
        }
        else
        {
            HideTutorialPanel();
            //gm.audioManager.Play2DSound(gm.audioManager.uiFailSound, .5f, false);
        }
    }

    public void HideTutorialPanel()
    {
        tutorialPanel.ShowHelpButton();
        // Reset the tutorial index to start from beginning next time we open the tutorial.
        tutorialIndex = -1;
    }
}
