using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the settings screen
public class Settings_UI : UI<Settings_UI>
{
    //Stores refrences to the UI
    [SerializeField] private TMP_Dropdown qualitySelect, movementSelect;
    [SerializeField] private Toggle soundEffectsToggle, vsyncToggle, leftHandedControlsToggle;
    [SerializeField] private TextMeshProUGUI warning, title;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button[] pageButtons;

    private int currentPageIndex;

    //Method called on object instatiation
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Serializer.Instance != null);
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);
        yield return new WaitUntil(() => Serializer.Instance.activeData.settings != null);

        Log(Serializer.Instance.activeData == null);
        Log(Serializer.Instance.activeData.settings == null);

        Gamemanager.Platform platform = Gamemanager.Instance.CurrentPlatform;

        //If on mobile, give warning about graphics setting, else do not
        if (platform == Gamemanager.Platform.Android || platform == Gamemanager.Platform.IOS || platform == Gamemanager.Platform.Debug)
        {
            warning.text = "Warning! Turning the quality above Fast could result in performance issues!";
        }
        else
        {
            warning.text = "";
        }

        //Fills the dropdowns from the associated enums
        FillDropFromEnum<QualityController.QualityLevel>(qualitySelect);
        FillDropFromEnum<Player.MovementType>(movementSelect,
            (platform == Gamemanager.Platform.Android || platform == Gamemanager.Platform.IOS) ?
                new int[] { (int)Player.MovementType.Keyboard } : null);

        //Adds the associated actions
        qualitySelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnQualitySelectChanged));
        movementSelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnMovementSelectChanged));
        soundEffectsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnSoundEffectToggleChanged));
        vsyncToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnVsyncChanged));
        leftHandedControlsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnLeftHandedChanged));

        //Sets values to default values
        qualitySelect.value = (int)Serializer.Instance.activeData.settings.qualityLevel;
        movementSelect.value = (int)Serializer.Instance.activeData.settings.movementType;
        soundEffectsToggle.isOn = Serializer.Instance.activeData.settings.soundEnabled;
        vsyncToggle.isOn = Serializer.Instance.activeData.settings.vsyncEnabled;
        leftHandedControlsToggle.isOn = Serializer.Instance.activeData.settings.leftHandedControls;
        currentPageIndex = 0;

        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
        title.text = pages[currentPageIndex].name;

        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            pageButtons[i].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);
        }
    }

    //Overriden method to show the screen
    public override void Show()
    {
        //Sets the base values for the UI elements
        qualitySelect.value = (int)Serializer.Instance.activeData.settings.qualityLevel;
        movementSelect.value = (int)Serializer.Instance.activeData.settings.movementType;
        soundEffectsToggle.isOn = Serializer.Instance.activeData.settings.soundEnabled;
        vsyncToggle.isOn = Serializer.Instance.activeData.settings.vsyncEnabled;
        leftHandedControlsToggle.isOn = Serializer.Instance.activeData.settings.leftHandedControls;
        currentPageIndex = 0;

        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
        title.text = pages[currentPageIndex].name;

        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            pageButtons[i].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);
        }
        //Activates the UI
        base.Show();
    }

    //Method called when the user changes the value in quality select
    private void OnQualitySelectChanged(int index)
    {
        //Sets the quality level from the associated place in the dropdown menu
        Serializer.Instance.activeData.settings.SetQualityLevel((QualityController.QualityLevel)index);
    }

    //Method called when the user changes the value in the sound effects toggle
    private void OnSoundEffectToggleChanged(bool value)
    {
        //Sets the sound effects
        Serializer.Instance.activeData.settings.EnableSound(value);
    }

    //Method called when the user changes the value in the vsync toggle
    private void OnVsyncChanged(bool value)
    {
        //Sets the vsync
        Serializer.Instance.activeData.settings.EnableVsync(value);
    }

    //Method called when the user changes the value in the left handed controls toggle
    private void OnLeftHandedChanged(bool value)
    {
        //Sets the controls to left or right
        Serializer.Instance.activeData.settings.EnableLeftHanded(value);
    }

    //Method called when the user changes the value in the movement select
    private void OnMovementSelectChanged(int index)
    {
        //If user is on mobile and selects keyboard, return
        if ((Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.Android || Gamemanager.Instance.CurrentPlatform == Gamemanager.Platform.IOS)
            && (Player.MovementType)index == Player.MovementType.Keyboard)
        {
            return;
        }

        //Sets the quality level form the associated place in the dropdown menu
        Serializer.Instance.activeData.settings.SetMovementType((Player.MovementType)index);
    }

    //Method called when the user clicks to go back
    public void Click_Back()
    {
        //Goes to the main screen with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Start_UI.Instance.Show();
                Hide();
            }, fadeTime));
    }

    public void SwapToPage(int index)
    {
        if (currentPageIndex == index) return;

        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);
        pages[currentPageIndex].SetActive(false);
        pageButtons[index].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
        pages[index].SetActive(true);

        title.text = pages[index].name;

        currentPageIndex = index;
    }
}
