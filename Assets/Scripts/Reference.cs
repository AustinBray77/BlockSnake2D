using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//Class to save static refrences to multiple frequently accessed objects.
public class Reference : Singleton<Reference>
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
    [SerializeField] private ModeSelect_UI _modeSelectUI;
    [SerializeField] private UnityAdsService _adManager;
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private Card[] _cardTypes;
    [SerializeField] private Skin _baseSkin;
    [SerializeField] private GameObject _smallPrompt, _mediumPrompt, _largePrompt, _infoObject;
    [SerializeField] private RectTransform _canvas;

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
    public static ModeSelect_UI modeSelectUI;
    public static UnityAdsService adManager;
    public static Tutorial tutorial;
    public static Card[] cardTypes;
    public static List<Level.LevelUpTrigger> levelUpTriggers;
    public static GameObject smallPrompt, mediumPrompt, largePrompt, infoObject;
    public static RectTransform canvas;

    //Method called on scene load
    private void Awake()
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
        modeSelectUI = _modeSelectUI;
        adManager = _adManager;
        tutorial = _tutorial;
        cardTypes = _cardTypes;

        for (int i = 0; i < cardTypes.Length; i++)
        {
            int x = i;
            cardTypes[i].SetIndex(x);
        }

        smallPrompt = _smallPrompt;
        mediumPrompt = _mediumPrompt;
        largePrompt = _largePrompt;
        infoObject = _infoObject;
        canvas = _canvas;

        //Generates the level triggers
        if (levelUpTriggers == null)
        {
            levelUpTriggers = new List<Level.LevelUpTrigger>();
            int gears = 3, skinIndex = 0;

            for (int i = 1; i <= 100; i++)
            {
                if (i % 5 == 0)
                {
                    gears++;
                }

                bool unlocksSkin = false;

                if (skinIndex < shopUI._skins.Length)
                {
                    while (shopUI._skins[skinIndex].levelRequirement <= i)
                    {
                        skinIndex++;
                        unlocksSkin = true;

                        if (skinIndex >= shopUI._skins.Length)
                        {
                            break;
                        }
                    }
                }

                levelUpTriggers.Add(new Level.LevelUpTrigger(i, gears, unlocksSkin));
            }
        }
    }
}
