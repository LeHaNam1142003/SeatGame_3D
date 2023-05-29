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
    [ShowIf("isHaveTopContent")] [SerializeField] private GameObject topContentUI;
    private GameObject bottom;
    protected GameObject getObj;
    protected int index;
    public void ShowContent()
    {
        bottom = new GameObject();
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
                var emptyObj = Instantiate(bottom, content.transform);
                emptyObj.AddComponent<RectTransform>();
                content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
            }
        }
    }
    void InstainUIContent(GameObject getUIContent)
    {
        getObj = Instantiate(getUIContent, content.transform);
        content.rectTransform().sizeDelta += new Vector2(content.cellSize.x, content.cellSize.y + content.spacing.y);
        SetIndexText();
    }
    protected abstract void SetIndexText();
}
