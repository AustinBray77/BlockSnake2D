using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//Class to save static refrences to multiple frequently accessed objects.
public class Refrence : MonoBehaviour
{
    //Instance refrences to the objects
    [SerializeField] private Player _player;
    [SerializeField] private Generator _gen;
    [SerializeField] private Camera _cam;
    [SerializeField] private BorderWall _wallTop, _wallBottom;
    [SerializeField] private Death_UI _deathUI;
    [SerializeField] private Game_UI _gameUI;
    [SerializeField] private Finish_UI _finishUI;
    [SerializeField] private Shop_UI _shopUI;
    [SerializeField] private Start_UI _startUI;
    [SerializeField] private Settings_UI _settingsUI;
    [SerializeField] private Credits_UI _creditsUI;
    [SerializeField] private AdManager _adManager;
    [SerializeField] private Card[] _cardTypes;
    [SerializeField] private Skin _baseSkin;

    //Static refrences
    public static Player player;
    public static Generator gen;
    public static Camera cam;
    public static CameraController camController;
    public static BorderWall wallTop, wallBottom;
    public static Death_UI deathUI;
    public static Game_UI gameUI;
    public static Finish_UI finishUI;
    public static Shop_UI shopUI;
    public static Start_UI startUI;
    public static Settings_UI settingsUI;
    public static Credits_UI creditsUI;
    public static AdManager adManager;
    public static Card[] cardTypes;

    //Method called on scene load
    private void Start()
    {
        //Assigns the instance refrences to the static refrences
        player = _player;
        gen = _gen;
        cam = _cam;
        camController = _cam.GetComponent<CameraController>();
        wallTop = _wallTop;
        wallBottom = _wallBottom;
        deathUI = _deathUI;
        gameUI = _gameUI;
        finishUI = _finishUI;
        shopUI = _shopUI;
        startUI = _startUI;
        settingsUI = _settingsUI;
        creditsUI = _creditsUI;
        adManager = _adManager;
        cardTypes = _cardTypes;

        if (cardTypes.Length > 0 && (Gamemode.platform == Gamemode.Platform.Android || Gamemode.platform == Gamemode.Platform.IOS))
        {
            Card[] newCardTypes = new Card[cardTypes.Length - 1];
            Array.Copy(cardTypes, newCardTypes, cardTypes.Length - 1);
            cardTypes = newCardTypes;
        }
    }
}
