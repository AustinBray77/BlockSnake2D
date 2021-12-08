using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Class to control the settings screen
public class Settings_UI : UI
{
    //Stores refrences to the UI
    [SerializeField] private TMP_Dropdown qualitySelect;
    [SerializeField] private Toggle soundEffectsToggle, vsyncToggle;
    [SerializeField] private TextMeshProUGUI warning;

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

        //Adds the items to the quality select dropdown
        foreach (QualityController.QualityLevel qualityLevel in System.Enum.GetValues(typeof(QualityController.QualityLevel)))
        {
            //Adds the item text
            qualitySelect.options.Add(new TMP_Dropdown.OptionData(qualityLevel.ToString()));

            //Adds the associated action
            qualitySelect.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<int>(OnQualitySelectChanged));
        }

        //Assigns the toggle functions
        soundEffectsToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnSoundEffectToggleChanged));
        vsyncToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OnVsyncChanged));
    }

    //Overriden method to show the screen
    public override void Show()
    {
        //Sets the base values for the UI elements
        qualitySelect.value = (int)Serializer.activeData.settings.qualityLevel;
        vsyncToggle.isOn = Serializer.activeData.settings.vsyncEnabled;
        soundEffectsToggle.isOn = Serializer.activeData.settings.soundEnabled;

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
    
    //Method called when the user clicks to go back
    public void Click_Back()
    {
        //Goes to the main screen with fade
        StartCoroutine(ClickWithFade(
            () => {
                Refrence.startUI.Show();
                Hide();
            }, fadeTime));
    }
}
