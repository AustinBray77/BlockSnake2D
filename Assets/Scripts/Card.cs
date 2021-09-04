using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Card
{
    [System.Serializable]
    public class IncrementTrigger
    {
        public float increaseAmount;
        public int levelAmount;
    }

    public string title;
    public int level;
    public Sprite image;
    public float value;
    [SerializeField] private bool isPercentage;
    [SerializeField] private UnityEvent _event;
    [SerializeField] private List<IncrementTrigger> valueCurve;

    private IncrementTrigger lastValueTriggered;

    public void Call()
    {
        _event.Invoke();
        level++;

        if (valueCurve.Count >= 1)
        {
            if (valueCurve[0].levelAmount <= level)
            {
                value += valueCurve[0].increaseAmount;
                lastValueTriggered = valueCurve[0];
                valueCurve.RemoveAt(0);
            }
        }
    }

    public string GetDescription()
    {
        string description = "";

        if (value > 0)
        {
            description += "+";
        }

        if (isPercentage)
        {
            float rawVal = value - 1f;
            description += Mathf.Round(rawVal * 100).ToString() + "%";
        } else
        {
            description += value.ToString();
        }

        return description;
    }

    public void RemoveLevel()
    {
        level--;

        if (lastValueTriggered != null)
        {
            if (lastValueTriggered.levelAmount > level)
            {
                value -= lastValueTriggered.increaseAmount;
                valueCurve.Insert(0, lastValueTriggered);
            }
        }
    }
}
