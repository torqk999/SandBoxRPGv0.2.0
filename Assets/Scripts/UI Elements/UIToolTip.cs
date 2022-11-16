using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    public RectTransform MyRect;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Stats;
    public TextMeshProUGUI Flavour;

    public bool Active;
    public Vector2 offset;

    public bool UpdateText(StringBuilder title, StringBuilder stats = null, StringBuilder flavour = null)
    {
        if (title == null)
            return false;

        Active = true;
        Title.text = title.ToString();

        if (stats != null)
            Stats.text = stats.ToString();
        else
            Stats.text = string.Empty;

        if(flavour != null)
            Flavour.text = flavour.ToString();
        else
            Flavour.text = string.Empty;

        return true;
    }

    public void DisableTip()
    {
        Active = false;
    }

    void FollowMouse(Vector2 mousePos)
    {
        if (Active && !gameObject.activeSelf)
            gameObject.SetActive(true);

        if (!Active)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }

        MyRect.position = mousePos + offset;
    }

    void Offset(Vector2 mousePos)
    {
        offset = MyRect.sizeDelta / 2;

        if (mousePos.x + offset.x > Screen.width)
            offset.x = -offset.x;

        if (mousePos.y + offset.y > Screen.height)
            offset.y = -offset.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        //MyRect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = (Vector2)Input.mousePosition;
        FollowMouse(mousePos);
        Offset(mousePos);
    }
}
