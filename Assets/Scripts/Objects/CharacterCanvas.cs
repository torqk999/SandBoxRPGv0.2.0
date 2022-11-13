using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCanvas : MonoBehaviour
{
    public Camera Cam;
    public Character Character;

    public Vector3 Offset;
    public TextMeshProUGUI LevelText;
    public Slider HealthBar;
    public Transform EffectContainer;
    public GameObject EffectImagePrefab;
    public Sprite MissingSprite;
    public int OldEffectsCount;

    void CheckCharacterEffects()
    {
        if (Character.Risiduals == null)
            return;

        if (Character.Risiduals.Count != OldEffectsCount)
        {
            OldEffectsCount = Character.Risiduals.Count;
            BuildEffectImages();
        }
    }

    void BuildEffectImages()
    {
        for (int i = EffectContainer.childCount - 1; i > -1; i--)
        {
            Debug.Log("Destroying effect");
            Destroy(EffectContainer.GetChild(i).gameObject);
        }
            

        foreach(BaseEffect effect in Character.Risiduals)
        {
            GameObject newEffectIcon = Instantiate(EffectImagePrefab, EffectContainer);
            newEffectIcon.SetActive(true);
            Image newEffectImage = newEffectIcon.GetComponent<Image>();
            newEffectImage.sprite = effect.Sprite != null ? effect.Sprite : MissingSprite;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Cam != null)
            transform.rotation = Cam.transform.rotation;

        if (Character != null)
        {
            if (Character.Sheet != null)
                LevelText.text = $"{Character.Sheet.Name}\n Lvl: {Character.Sheet.Level}";

            if (Character.CurrentStats.Stats != null && Character.MaximumStatValues.Stats != null)
                HealthBar.value = Character.CurrentStats.Stats[(int)RawStat.HEALTH] / Character.MaximumStatValues.Stats[(int)RawStat.HEALTH];

            CheckCharacterEffects();
        }
    }
}
