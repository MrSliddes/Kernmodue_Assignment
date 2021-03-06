﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using TMPro;
using UnityEngine;

public class Script_Mono : MonoBehaviour, IScoreManager
{
    //Gedeelte van mikey
    [Header("Power Up Components")]
    /// <summary>
    /// Prefab for the powerups to use
    /// </summary>
    public GameObject _powerUpPrefab;

    /// <summary>
    /// Reference to the ball object
    /// </summary>
    public GameObject _ballGameObject;

    /// <summary>
    /// Reference to the flashbang powerup
    /// </summary>
    public GameObject _panel;

    /// <summary>
    /// 2 Colors to lerp the background between.
    /// </summary>
    public Color _color1 = Color.black;
    public Color _color2 = Color.blue;

    /// <summary>
    /// Lerp time
    /// </summary>
    private float _duration = 3.0F;

    /// <summary>
    /// Reference to the powerup class
    /// </summary>
    private PowerUpBase _powerUps;


    //Gedeelte Tymon (Yes we know, the order of public/protected/private is invalid here)
    public static Script_Mono INSTANCE { get; set; }
    [Header("Vars")]
    /// <summary>
    /// The score of the player and ai, x = ai, y = player
    /// </summary>
    public Vector2 _score;

    /// <summary>
    /// The speed of the ball, only added when created (cannot be updated)
    /// </summary>
    public float _ballSpeed = 5f;

    [Header("Needed Components")]
    /// <summary>
    /// The transform of the pongball (the ball that bounces over the screen)
    /// </summary>
    public Transform _pongball;

    /// <summary>
    /// The transform of the player, this one is located on the right side of the screen
    /// </summary>
    public Transform _player;

    /// <summary>
    /// The transform of the enemy, this one is located on the left side of the screen
    /// </summary>
    public Transform _enemy;

    /// <summary>
    /// The textmeshprogui component that displays the score
    /// </summary>
    public TextMeshProUGUI _uiScore;

    /// <summary>
    /// Particle that plays when point scored on left side
    /// </summary>
    public ParticleSystem _scoredLeftParticle;
    /// <summary>
    /// Particle that plays when point scored on right side
    /// </summary>
    public ParticleSystem _scoredRightParticle;

    /// <summary>
    /// Button that triggers singleplayer mode
    /// </summary>
    public GameObject _buttonSingleplayer;
    /// <summary>
    /// Button that triggers multiplayer mode
    /// </summary>
    public GameObject _buttonMultiplayer;

    /// <summary>
    /// Reference to the pongball class
    /// </summary>
    private Tymon_Pongball _tymon_pongball;

    /// <summary>
    /// Reference to the player class
    /// </summary>
    private Paddle _playerPaddle;

    /// <summary>
    /// If there is a player 2 this is the class
    /// </summary>
    private Paddle _playerPaddle_1;

    /// <summary>
    /// Reference to the enemy class
    /// </summary>
    private Paddle _enemyPaddle;

    public void Start()
    {
        INSTANCE = this;

        //Instantiates the powerup class to run the startup to add everything to the dictonary
        _powerUps = new PowerUpBase();
        _powerUps.StartUp(_powerUpPrefab, _ballGameObject, _panel);
    }

    public void Update()
    {
        // Update the classes via the Update Methode (Not the monobehaivor Update()!)
        _tymon_pongball?.Update();
        _playerPaddle?.Update();
        _playerPaddle_1?.Update();
        _enemyPaddle?.Update();

        //Updates the powerups
        _powerUps.UpdateAll();
        ChangeBackGround();
    }

    /// <summary>
    /// Change the background color of the camera
    /// </summary>
    private void ChangeBackGround()
    {
        float t = Mathf.PingPong(Time.time, _duration) / _duration;
        Camera.main.backgroundColor = Color.Lerp(_color1, _color2, t);
    }
       

    /// <summary>
    /// Triggerd when the player presses the singleplayer button, sets the game to singleplayer
    /// </summary>
    public void ButtonPressedSingleplayer()
    {
        // Disable buttons
        _buttonSingleplayer.SetActive(false);
        _buttonMultiplayer.SetActive(false);
        // Create a transform array for the ball to check for collisions
        Transform[] arr = { _player, _enemy };
        // Set the class references/ create the classes
        _tymon_pongball = new Tymon_Pongball(_pongball, _ballSpeed, arr, this);
        // obselete _tymon_player = new Tymon_Player(_player, 10f, 8f, false);
        // obselete _tymon_enemy = new Tymon_Enemy(_enemy, _pongball, -7f, 9f);
        _playerPaddle = new Paddle(10f, 8f, _player, new PaddleControllerPlayer(KeyCode.W, KeyCode.S));
        _enemyPaddle = new Paddle(9f, -7f, _enemy, new PaddleControllerAI(_enemy, _pongball));
    }

    /// <summary>
    /// Triggerd when the player presses the multiplayer button, sets the game to multiplayer
    /// </summary>
    public void ButtonPressedMultiplayer()
    {
        // Disable buttons
        _buttonSingleplayer.SetActive(false);
        _buttonMultiplayer.SetActive(false);
        // Create a transform array for the ball to check for collisions
        Transform[] arr = { _player, _enemy };
        // Set the class references/ create the classes
        _tymon_pongball = new Tymon_Pongball(_pongball, _ballSpeed, arr, this);
        // obselete _tymon_player = new Tymon_Player(_player, 10f, 8f, false);
        // obselete _tymon_player_2 = new Tymon_Player(_enemy, 10f, -7f, true);
        _playerPaddle = new Paddle(10f, 8f, _player, new PaddleControllerPlayer(KeyCode.UpArrow, KeyCode.DownArrow));
        _enemyPaddle = new Paddle(10f, -7f, _enemy, new PaddleControllerPlayer(KeyCode.W, KeyCode.S));
    }

    /// <summary>
    /// IScoreManager Interface implementation
    /// </summary>
    /// <param name="scoreToAdd">Vector 2 score meaning 1, 0 is left won and 0, 1 is right won</param>
    public void UpdateScore(Vector2 scoreToAdd)
    {
        // Add score
        _score += scoreToAdd;
        // Update ui
        _uiScore.text = _score.x.ToString() + " | " + _score.y.ToString();
        // Play particle
        if(scoreToAdd.x > 0)
        {
            _scoredRightParticle.Play();
        }
        else
        {
            _scoredLeftParticle.Play();
        }
    }
}