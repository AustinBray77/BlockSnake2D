using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to save static refrences to multiple frequently accessed objects.
public class Refrence : MonoBehaviour
{
    //Instance refrences to the objects
    [SerializeField] private Player _player;
    [SerializeField] private Generator _gen;
    [SerializeField] private Destroyer _des;
    [SerializeField] private Camera _cam;
    [SerializeField] private BorderWall _wallTop, _wallBottom;
    [SerializeField] private Death_UI _deathUI;
    [SerializeField] private Game_UI _gameUI;
    [SerializeField] private Finish_UI _finishUI;
    [SerializeField] private Shop_UI _shopUI;
    [SerializeField] private Start_UI _startUI;
    [SerializeField] private Settings_UI _settingsUI;
    [SerializeField] private AdManager _adManager;
    [SerializeField] private Card[] _cardTypes;
    [SerializeField] private Skin _baseSkin;

    //Static refrences
    public static Player player;
    public static Generator gen;
    public static Destroyer des;
    public static Camera cam;
    public static CameraController camController;
    public static BorderWall wallTop, wallBottom;
    public static Death_UI deathUI;
    public static Game_UI gameUI;
    public static Finish_UI finishUI;
    public static Shop_UI shopUI;
    public static Start_UI startUI;
    public static Settings_UI settingsUI;
    public static AdManager adManager;
    public static Card[] cardTypes;

    //Method called on scene load
    private void Start()
    {
        //Assigns the instance refrences to the static refrences
        player = _player;
        gen = _gen;
        des = _des;
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
        adManager = _adManager;
        cardTypes = _cardTypes;
    }
}
