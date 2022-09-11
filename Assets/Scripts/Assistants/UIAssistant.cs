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
public struct UIProfile
{
    public UIType UIType;
    public Sprite UISprite;
    public Text UIText;
}

public class UIAssistant : MonoBehaviour
{
    public UIType myType;
    public UIManager myManager;
    public Image myTargetImage;
    public Text myTargetText;

    bool PullMyImage()
    {
        myTargetImage = gameObject.GetComponent<Image>();
        return myTargetImage != null;
    }

    bool PullMyTextChild()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Text>() != null)
            {
                myTargetText = transform.GetChild(i).GetComponent<Text>();
                return true;
            }
        }
        return false;
    }

    bool DiscoverUIManager()
    {
        Transform currentParent = gameObject.transform.parent;
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

        UIProfile? mySprite = myManager.UIGraphicBin.Find(x => x.UIType == myType);
        if (!mySprite.HasValue)
            return false;

        myTargetImage.sprite = mySprite.Value.UISprite;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PullMyImage() && !PullMyTextChild())
        {
            Debug.Log($"{gameObject.name} Has nothing to work with Sam!");
            return;
        }       

        if (!DiscoverUIManager())
        {
            Debug.Log($"{gameObject.name} Has no UIManager in family tree!");
            return;
        }
        
        if (FindAndSetMyImage())
            Debug.Log($"{gameObject.name} Found their sprite!");
        else
            Debug.Log($"{gameObject.name} Misplaced their sprite!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
