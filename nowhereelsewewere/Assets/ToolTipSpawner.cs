using UnityEngine;
using System.Collections;

public class ToolTipSpawner : MonoBehaviour {

    public string tip;

    public static float timeUntilShow = 1.0f;

    private RectTransform rectTransform;

    private ToolTip toolTipPrefab;
    private ToolTip toolTipInstance;

    public Vector2 offset = new Vector2(100.0f, 60.0f);

    private float spawnTimer = 0.0f;

    // Use this for initialization
    void Start () {
        rectTransform = GetComponent<RectTransform>();
        toolTipPrefab = Resources.Load<ToolTip>("ToolTip");
        spawnTimer = timeUntilShow;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        bool contains = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, point);

        // If cursor is in UI element.
        if (contains && toolTipInstance == null)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0)
            {
                SpawnToolTip();
            }
        }

        // If cursor exits UI element.
        if(!contains && toolTipInstance != null)
        {
            spawnTimer = timeUntilShow;
            DestroyToolTip();
        }
	}

    private void SpawnToolTip()
    {
        toolTipInstance = Instantiate(toolTipPrefab, this.transform, false) as ToolTip;
        toolTipInstance.transform.localPosition = new Vector3(offset.x, offset.y, 0);
        toolTipInstance.setToolTipText(tip);
    }

    private void DestroyToolTip()
    {
        Destroy(toolTipInstance.gameObject);
    }
}
