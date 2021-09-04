using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Card_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void DataFromCard(Card data)
    {
        title.text = data.title.ToUpper() + " - LEVEL " + (data.level + 1).ToString();
        description.text = data.GetDescription();
        image.sprite = data.image;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(new UnityAction(data.Call));
    }
}
