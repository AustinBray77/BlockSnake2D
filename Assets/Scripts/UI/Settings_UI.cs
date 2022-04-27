using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the settings screen
public class Settings_UI : UI<Settings_UI>
{
    //Stores refrences to quality select and movement type select dropdowns
    [SerializeField] private TMP_Dropdown qualitySelect, movementSelect;

    //Stores reference to sound toggle vsync toggle and controls side toggle
    [SerializeField] private Toggle soundEffectsToggle, vsyncToggle, leftHandedControlsToggle;

    //Stores references to the warning and title text boxes
    [SerializeField] private TextMeshProUGUI warning, title;

    //Stores a reference to each page of the settings
    [SerializeField] private GameObject[] pages;

    //Stores a reference to each of the buttons used to switch pages
    [SerializeField] private Button[] pageButtons;

    //Stores the current page index
    private int currentPageIndex;

    //Method called on object instatiation
    private IEnumerator Start()
    {
        //Waits until the settings have been intialized
        yield return new WaitUntil(() => Serializer.Instance != null);
        yield return new WaitUntil(() => Serializer.Instance.activeData != null);
        yield return new WaitUntil(() => Serializer.Instance.activeData.settings != null);

        //Gets the current platform
        Gamemanager.Platform platform = Gamemanager.Instance.CurrentPlatform;

        //Show a warning about performance if on mobile
        warning.text = (platform == Gamemanager.Platform.Android || platform == Gamemanager.Platform.IOS || platform == Gamemanager.Platform.Debug) ?
            "Warning! Turning the quality above Fast could result in performance issues!" : "";

        //Fills the dropdowns from the associated enums
        FillDropFromEnum<QualityController.QualityLevel>(qualitySelect);
        FillDropFromEnum<Player.MovementType>(movementSelect,
            (platform == Gamemanager.Platform.Android || platform == Gamemanager.Platform.IOS) ?
                new int[] { (int)Player.MovementType.Keyboard } : null);

        //Adds the associated actions to the UI elements
        qualitySelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnQualitySelectChanged));
        movementSelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnMovementSelectChanged));
        soundEffectsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnSoundEffectToggleChanged));
        vsyncToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnVsyncChanged));
        leftHandedControlsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnLeftHandedChanged));

        //Resets the UI elements
        Reset();
    }

    //Overriden method to show the screen
    public override void Show()
    {
        //Resets the UI elements
        Reset();

        //Activates the UI
        base.Show();
    }

    //Method to set the UI elements to reset the UI elements
    private void Reset()
    {
        //Sets values to default values for the UI elements
        qualitySelect.value = (int)Serializer.Instance.activeData.settings.qualityLevel;
        movementSelect.value = (int)Serializer.Instance.activeData.settings.movementType;
        soundEffectsToggle.isOn = Serializer.Instance.activeData.settings.soundEnabled;
        vsyncToggle.isOn = Serializer.Instance.activeData.settings.vsyncEnabled;
        leftHandedControlsToggle.isOn = Serializer.Instance.activeData.settings.leftHandedControls;
        currentPageIndex = 0;

        //Sets colour for the current page button
        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);

        //Sets the title to the title of the page
        title.text = pages[currentPageIndex].name;

        //Loops through every page except the first
        for (int i = 1; i < pages.Length; i++)
        {
            //Deactivate the page
            pages[i].SetActive(false);

            //Assign its colour
            pageButtons[i].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);
        }
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

        //Else sets the quality level form the associated place in the dropdown menu
        Serializer.Instance.activeData.settings.SetMovementType((Player.MovementType)index);
    }

    //Method called when the user clicks to go back
    public void Click_Back()
    {
        //Activates the following method with a fade
        StartCoroutine(ClickWithFade(
            () =>
            {
                //Shows the start UI
                Start_UI.Instance.Show();

                //Hides this UI
                Hide();
            },
            //Fades in fadeTime time
            fadeTime));
    }

    //Method called to change to a different page
    public void SwapToPage(int index)
    {
        //Return if the current pages index is requested
        if (currentPageIndex == index) return;

        //Resets color of current page button
        pageButtons[currentPageIndex].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 0);

        //Deactivates the current page
        pages[currentPageIndex].SetActive(false);

        //Sets the color new page button to be opaque
        pageButtons[index].image.color = new Color(0.2196079f, 0.2196079f, 0.2196079f, 1);

        //Activates the new page
        pages[index].SetActive(true);

        //Sets the title to the title of the page
        title.text = pages[index].name;

        //Saves the current page
        currentPageIndex = index;
    }
}
