using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum UIType
{
    MAIN_PANEL,
    SUB_PANEL,
    MENU_BUTTON,
    SLOT_HOLDER,
    SCROLL_BAR
}

[Serializable]
public struct UIGraphic
{
    public UIType GraphicType;
    public Sprite GraphicSprite;
}

public class UIAssistant : MonoBehaviour
{
    public UIType myType;
    public UIManager myManager;
    public Image myTargetImage;
    

    bool PullMyImage()
    {
        myTargetImage = this.gameObject.GetComponent<Image>();
        return myTargetImage != null;
    }

    bool DiscoverUIManager()
    {
        Transform currentParent = this.gameObject.transform.parent;
        while(currentParent != null)
        {
            myManager = currentParent.GetComponent<UIManager>();
            if (myManager != null)
                return true;
            currentParent = currentParent.parent;
        }
        return false;
    }

    bool FindAndSetMyImage()
    {
        if (myManager.UIGraphicBin.Count == 0)
            return false;

        UIGraphic? mySprite = myManager.UIGraphicBin.Find(x => x.GraphicType == myType);
        if (!mySprite.HasValue)
            return false;

        myTargetImage.sprite = mySprite.Value.GraphicSprite;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PullMyImage())
        {
            Debug.Log($"{this.gameObject.name} Has no Image Comp!");
            return;
        }
            

        if (!DiscoverUIManager())
        {
            Debug.Log($"{this.gameObject.name} Has no UIManager in family tree!");
            return;
        }
        
        if (FindAndSetMyImage())
            Debug.Log($"{this.gameObject.name} Found their sprite!");
        else
            Debug.Log($"{this.gameObject.name} Misplaced their sprite!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
