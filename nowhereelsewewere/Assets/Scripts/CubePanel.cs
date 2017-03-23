using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CubePanel : UIPanel {

    private GameManager gm;

	[SerializeField]
	private Text textPrefab;

    [SerializeField]
    private VerticalLayoutGroup cubeStatsVertLayout;

    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        this.gameObject.SetActive(false);
    }

	public void Populate(Cube c)
	{
		Depopulate();
		if(c != null)
		{
            VerticalLayoutGroup g = cubeStatsVertLayout;
            if (g != null)
			{
				Text stat = (Text)Instantiate(textPrefab, g.transform);
				stat.text = c.GetCubeType() + " cube";
				stat = (Text)Instantiate(textPrefab, g.transform);
				stat.text = "Movement: " + (c.IsMovable() ? (1 - (int)c.difficulty).ToString() : "---");
				stat = (Text)Instantiate(textPrefab, g.transform);
				stat.text = "Dodge: " + (c.IsMovable() ? "+" + c.hitDifficulty.ToString() : "---");

                // Normalize the scale of all UI components.
                if (gm != null)
                {
                    gm.uiManager.NormalizeChildrenScale(g.gameObject);
                }

                // Show the cube panel
                this.transform.gameObject.SetActive(true);
            }
        }
	}

	public void Depopulate()
	{
        VerticalLayoutGroup g = cubeStatsVertLayout;
        if (g != null)
		{
			int numChilds = g.transform.childCount;
			for(int i = numChilds - 1; i >= 0; i--)
			{
				GameObject.Destroy(g.transform.GetChild(i).gameObject);
            }

            // Hide the cube panel 
            this.transform.gameObject.SetActive(false);
        }
	}
}
