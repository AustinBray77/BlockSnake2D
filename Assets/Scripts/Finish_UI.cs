using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish_UI : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    private GameObject[] cardObjects = new GameObject[3];
    private FinishObject finishObj;

    public void Show(Card[] cards, FinishObject _finishObj)
    {
        finishObj = _finishObj;

        for (int i = 0; i < 3; i++)
        {
            cardObjects[i] = Instantiate(cardPrefab, transform);
            cardObjects[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(630 - (i * 630), 0);
            cardObjects[i].GetComponent<Card_Manager>().DataFromCard(cards[i]);
        }
    }

    public void Hide()
    {
        foreach (GameObject cardObejct in cardObjects)
        {
            Destroy(cardObejct);
        }

        Player.isAtFinish = false;
        Refrence.gameUI.SetActive(true);
        gameObject.SetActive(false);
    }
}
