using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollBoard : MonoBehaviour
{
    public int elements;
    public GridLayoutGroup content;
    [SerializeField] private GameObject contentUI;
    [SerializeField] private bool isHaveTopContent;
    [SerializeField] private GameObject emptyObj;
    [ShowIf("isHaveTopContent")] [SerializeField] private GameObject topContentUI;
    [ReadOnly] public List<GameObject> contentUIs;
    protected GameObject getObj;
    protected int index;
    public void ShowContent()
    {
        for (int i = 1; i <= elements + 1; i++)
        {
            index = i;
            if (i <= elements)
            {
                if (i == elements && topContentUI != null)
                {
                    InstainUIContent(topContentUI);
                }
                else
                {
                    InstainUIContent(contentUI);
                }
            }
            else
            {
                var emptyObj = Instantiate(this.emptyObj, content.transform);
                emptyObj.AddComponent<RectTransform>();
                content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
            }
        }
    }
    public void ClearContent()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            DestroyImmediate(content.transform.GetChild(i).gameObject);
        }
    }

    void InstainUIContent(GameObject getUIContent)
    {
        getObj = Instantiate(getUIContent, content.transform);
        contentUIs.Add(getObj);
        content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
        SetIndexText();
    }
    protected abstract void SetIndexText();
}
