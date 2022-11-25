using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragTest : MonoBehaviour
{
    public RectTransform MyRect;
    public Image MyImage;
    public Vector2 Offset;
    public GraphicRaycaster MyRayCaster;
    public List<RaycastResult> HitBuffer;

    void FollowMouse()
    {
        MyRect.anchoredPosition = (Vector2)Input.mousePosition + Offset;
    }
    private void OnMouseDown()
    {
        MyImage.enabled = true;
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pd = new PointerEventData(EventSystem.current);
            pd.position = Input.mousePosition;
            HitBuffer.Clear();
            MyRayCaster.Raycast(pd, HitBuffer);
            foreach(RaycastResult result in HitBuffer)
            {
                Debug.Log($"name: {result.gameObject.name}");

                if (result.gameObject == null)
                    continue;

                if (result.gameObject.tag == GlobalConstants.TAG_BUTTON)
                {
                    MyImage.sprite = result.gameObject.GetComponent<Image>().sprite;
                    MyImage.enabled = true;
                    return;
                }
            }
        }
        if (!Input.GetMouseButton(0))
        {
            MyImage.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HitBuffer = new List<RaycastResult>();
        MyRect = GetComponent<RectTransform>();
        MyImage = GetComponent<Image>();
        MyImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
        CheckClick();
    }
}
