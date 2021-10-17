using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop_UI : UI
{
    [SerializeField] private Skin[] _skins;
    [SerializeField] private GameObject skinCardPrefab;
    [SerializeField] private TMP_Text gearText;

    private List<GameObject> skinCards;
    private List<Skin_Manager> skin_Managers;

    public static Skin[] skins;
    public static int activeSkinCard = 0;

    public void Start()
    {
        skins = _skins;
        gearText.text = Serializer.activeData.gearCount.ToString();
    }

    public void Show()
    {
        activeSkinCard = 0;

        skinCards = new List<GameObject>();
        skin_Managers = new List<Skin_Manager>();

        for(int i = 0; i < _skins.Length; i++)
        {
            skinCards.Add(CreateSkinCard(new Vector2(i * 625, 0), new Vector3(i == 0 ? 1 : 0.9f, i == 0 ? 1 : 0.9f), _skins[i]));
            skin_Managers.Add(skinCards[i].GetComponent<Skin_Manager>());
        }
    }

    public void Hide()
    {
        while(skinCards.Count > 0)
        {
            Destroy(skinCards[0]);
            skinCards.RemoveAt(0);
        }

        gameObject.SetActive(false);
    }

    public void MoveCards(int direction)
    {
        activeSkinCard += direction;

        for(int i = 0; i < skinCards.Count; i++)
        {
            RectTransform rt = skinCards[i].GetComponent<RectTransform>();
            rt.LeanMove(new Vector3((i - activeSkinCard) * 625, 0), 1f);
            rt.LeanScale(new Vector3(i - activeSkinCard == 0 ? 1 : 0.9f, i - activeSkinCard == 0 ? 1 : 0.9f), 1f);
        }   
    }

    public void UpdateAllCards()
    {
        gearText.text = Serializer.activeData.gearCount.ToString();

        foreach (Skin_Manager sm in skin_Managers)
        {
            sm.UpdateData();
        }
    }

    private GameObject CreateSkinCard(Vector2 location, Vector3 scale, Skin skin)
    {
        GameObject skinCard = Instantiate(skinCardPrefab, transform);
        RectTransform rt = skinCard.GetComponent<RectTransform>();
        rt.anchoredPosition = location;
        rt.localScale = scale;
        skinCard.GetComponent<Skin_Manager>().FromSkinObject(skin);

        return skinCard;
    }
}
