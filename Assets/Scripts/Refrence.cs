using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refrence : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Generator _gen;
    [SerializeField] private Destroyer _des;
    [SerializeField] private Camera _cam;
    [SerializeField] private BorderWall _wallTop, _wallBottom;
    [SerializeField] private GameObject _deathUI, _gameUI, _finishUI;
    [SerializeField] private AdManager _adManager;
    [SerializeField] private Card[] _cardTypes;


    public static Player player;
    public static Generator gen;
    public static Destroyer des;
    public static Camera cam;
    public static BorderWall wallTop, wallBottom;
    public static GameObject deathUI, gameUI, finishUI;
    public static AdManager adManager;
    public static Card[] cardTypes;

    private void Start()
    {
        player = _player;
        gen = _gen;
        des = _des;
        cam = _cam;
        wallTop = _wallTop;
        wallBottom = _wallBottom;
        deathUI = _deathUI;
        gameUI = _gameUI;
        finishUI = _finishUI;
        adManager = _adManager;
        cardTypes = _cardTypes;
    }
}
