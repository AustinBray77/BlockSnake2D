using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the settings screen
public class Settings_UI : UI
{
    //Stores refrences to the UI
    [SerializeField] private TMP_Dropdown qualitySelect, movementSelect;
    [SerializeField] private Toggle soundEffectsToggle, vsyncToggle, leftHandedControlsToggle;
    [SerializeField] private TextMeshProUGUI warning, title;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button[] pageButtons;

    private int currentPageIndex;

    //Method called on object instatiation
    private void Start()
    {
        //If on mobile, give warning about graphics setting, else do not
        if (Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.IOS || Gamemode.platform == Gamemode.Platform.Debug)
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
            (Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.IOS) ?
                new int[] { (int)Player.MovementType.Keyboard } : null);

        //Adds the associated actions
        qualitySelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnQualitySelectChanged));
        movementSelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnMovementSelectChanged));
        soundEffectsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnSoundEffectToggleChanged));
        vsyncToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnVsyncChanged));
        leftHandedControlsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnLeftHandedChanged));

        //Sets values to default values
        qualitySelect.value = (int)Serializer.activeData.settings.qualityLevel;
        movementSelect.value = (int)Serializer.activeData.settings.movementType;
        soundEffectsToggle.isOn = Serializer.activeData.settings.soundEnabled;
        vsyncToggle.isOn = Serializer.activeData.settings.vsyncEnabled;
        leftHandedControlsToggle.isOn = Serializer.activeData.settings.leftHandedControls;
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
        qualitySelect.value = (int)Serializer.activeData.settings.qualityLevel;
        movementSelect.value = (int)Serializer.activeData.settings.movementType;
        soundEffectsToggle.isOn = Serializer.activeData.settings.soundEnabled;
        vsyncToggle.isOn = Serializer.activeData.settings.vsyncEnabled;
        leftHandedControlsToggle.isOn = Serializer.activeData.settings.leftHandedControls;
        currentPageIndex = 0;

        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);
        title.text = pages[currentPageIndex].name;

        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            pageButtons[i].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);
        }
        //Activates the UI
        UIContainer.SetActive(true);
    }

    //Method called when the user changes the value in quality select
    private void OnQualitySelectChanged(int index)
    {
        //Sets the quality level from the associated place in the dropdown menu
        Serializer.activeData.settings.SetQualityLevel((QualityController.QualityLevel)index);
    }

    //Method called when the user changes the value in the sound effects toggle
    private void OnSoundEffectToggleChanged(bool value)
    {
        //Sets the sound effects
        Serializer.activeData.settings.EnableSound(value);
    }

    //Method called when the user changes the value in the vsync toggle
    private void OnVsyncChanged(bool value)
    {
        //Sets the vsync
        Serializer.activeData.settings.EnableVsync(value);
    }

    //Method called when the user changes the value in the left handed controls toggle
    private void OnLeftHandedChanged(bool value)
    {
        //Sets the controls to left or right
        Serializer.activeData.settings.EnableLeftHanded(value);
    }

    //Method called when the user changes the value in the movement select
    private void OnMovementSelectChanged(int index)
    {
        //If user is on mobile and selects keyboard, return
        if ((Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.IOS) && (Player.MovementType)index == Player.MovementType.Keyboard)
        {
            return;
        }

        //Sets the quality level form the associated place in the dropdown menu
        Serializer.activeData.settings.SetMovementType((Player.MovementType)index);
    }

    //Method called when the user clicks to go back
    public void Click_Back()
    {
        //Goes to the main screen with fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                Refrence.startUI.Show();
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
