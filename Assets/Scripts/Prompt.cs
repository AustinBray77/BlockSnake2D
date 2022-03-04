using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Prompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button actionBtn, exitBtn;
    [SerializeField] private List<TextMeshProUGUI> descs;
    [SerializeField] private List<Image> images;

    public void OnAwake()
    {
        transform.localScale = new Vector3(0, 0);
        gameObject.LeanScale(new Vector3(1, 1), UI.fadeTime);
    }

    public void AddButtonListener(UnityEngine.Events.UnityAction action)
    {
        actionBtn.onClick.AddListener(action);
    }

    public void SetImages(List<Sprite> sprites)
    {
        for (int i = 0; i < images.Count && i < sprites.Count; i++)
        {
            images[i].sprite = sprites[i];
        }
    }

    public void SetTitleText(string text, int size = 0)
    {
        title.text = text;

        if (size != 0)
        {
            title.fontSize = size;
        }
    }

    public void SetDescriptions(string[] texts, int[] sizes = null)
    {
        for (int i = 0; i < descs.Count && i < texts.Length; i++)
        {
            descs[i].text = texts[i];

            if (sizes != null)
            {
                if (sizes.Length > i)
                {
                    descs[i].fontSize = sizes[i];
                }
            }
        }
    }

    public void ToCloseOnClick()
    {
        actionBtn.onClick = exitBtn.onClick;
    }

    public void OnClickClose()
    {
        StartCoroutine(ClosePrompt());
    }

    public IEnumerator ClosePrompt()
    {
        gameObject.LeanScale(new Vector3(0, 0), UI.fadeTime);

        yield return new WaitForSeconds(UI.fadeTime);

        Destroy(gameObject);
    }

    public void ForceInteraction()
    {
        exitBtn.gameObject.SetActive(false);
    }
}
