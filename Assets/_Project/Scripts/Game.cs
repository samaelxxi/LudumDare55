using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Assets.AudioManager;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class Game : Singleton<Game>
{
    [field: SerializeField] public PiggyEvolutions PiggyEvolutions { get; private set; }

    [SerializeField] int _currentLevel = 1;

    public IEnumerable<PiggyData> PlayerPiggies() => _player.GetPiggies();
    public int PlayerCorn => _player.Corn;
    public int PiggiesQuantity => _player.GetPiggies().Count();
    public int NewPiggyCost => PiggyEvolutions.BasePiggyCost + (PiggiesQuantity - PiggyEvolutions.InitialPiggies) * PiggyEvolutions.ExtraNewPiggyCost;
    public bool CanBuyNewPiggy => PlayerCorn >= NewPiggyCost && PiggiesQuantity < PiggyEvolutions.MaxPiggies;

    public Level Level => _level;
    public RoadManager RoadManager => _level.RoadManager;

    Player _player;
    Level _level;
    EvolutionsMenu _evolutionsMenu;
    [SerializeField] AudioManager _audioManager;
    public AudioManager AudioManager => _audioManager;


    public event Action<Vector3> OnMouseClick;
    public event Action<int> OnKeyNumberPressed;
    public event Action OnRestartLevel;

    public bool IsAwaken = false;

    public int PiggiesToAdd = 0;

    bool _setupedThisScene = false;

    enum GameState { Harvest, Upgrade, Menu }

    GameState _state = GameState.Harvest;

    int _maxLevel = -1;

    public event System.Action<int> OnCornChanged
    {
        add { _player.OnCornChanged += value; }
        remove { _player.OnCornChanged -= value; }
    }

    public override void Awake()
    {
        // Debug.Log("Game awake" + gameObject.name);
        base.Awake();
        gameObject.name = "PiggyHarvest";

        if (Game.Instance.IsAwaken)
            return;
        if (PiggyEvolutions == null)
            PiggyEvolutions = Resources.Load<PiggyEvolutions>("SO/PiggyEvolutions");
        if (_audioManager == null)
        {
            Debug.Log("Loading audio manager");
            _audioManager = FindAnyObjectByType<AudioManager>();
            if (_audioManager == null)
            {
                _audioManager = Resources.Load<AudioManager>("AudioManager");
                _audioManager = Instantiate(_audioManager);
            }
            AudioPool.Initialise(_audioManager.AudioManagerFile.soundPrefab, 10);
            DontDestroyOnLoad(_audioManager);
        }

        _player = new Player(PiggyEvolutions.Normal[0], PiggyEvolutions.InitialPiggies);
        _player.ReceiveCorn(PiggyEvolutions.StartFood);
        // InitTestPigs();
        
        // Debug.Log("Game awake 2" + gameObject.name);
        SceneManager.sceneLoaded += (scene, mode) => OnSceneLoaded();

        DetectLevelsNumber();
        IsAwaken = true;

        Debug.Log("Game awake" + gameObject.name + _setupedThisScene);
        if (!_setupedThisScene)
            OnSceneLoaded();  // idk sometimes it subscribes before scene loaded sometimes after
    }

    void Start()
    {
        Game.Instance.AudioManager.Play("piggyHarvest2", loop: true, volume: 0.6f);
    }

    void DetectLevelsNumber()
    {
        _maxLevel = PiggyEvolutions.LevelsNum;
        Debug.Log("Found " + _maxLevel + " levels");
    }

    void OnSceneLoaded()
    {
        if (_setupedThisScene)
            return;
        Debug.Log("Scene loaded" + SceneManager.GetActiveScene().name);
        if (FindFirstObjectByType<Level>() != null)
        {
            Debug.Log("Level loaded");
            StartLevel();
        }
        else if (FindAnyObjectByType<EvolutionsMenu>() != null)
        {
            Debug.Log("Evolutions loaded");
            StartEvolution();
        }
        else
            _state = GameState.Menu;  // idk
        _setupedThisScene = true;
    }

    void InitTestPigs()
    {
        // add all types of piggies
        _player.UpgradePiggyRank(_player.GetPiggies().First());
        _player.UpgradePiggyRank(_player.GetPiggies().First());
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(1));

        _player.ChangePiggyType(_player.GetPiggies().ElementAt(4), PiggyType.Fast);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(5), PiggyType.Fast);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(6), PiggyType.Fast);
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(5));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(6));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(6));

        _player.ChangePiggyType(_player.GetPiggies().ElementAt(3), PiggyType.Fat);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(7), PiggyType.Fat);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(8), PiggyType.Fat);
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(7));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(8));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(8));
    }

    void StartLevel()
    {
        _level = FindFirstObjectByType<Level>();
        _level.OnLevelCompleted += CompleteLevel;
        _level.OnRestartLevel += RestartLevel;
        _state = GameState.Harvest;
        _currentLevel = int.Parse(SceneManager.GetActiveScene().name.Substring(5));
    }

    void RestartLevel()
    {
        if (_currentLevel > 1)
            GoToEvolutionsScene();
        else
            GoToNextLevelScene();
    }

    void CompleteLevel()
    {
        Debug.Log("Level completed" + _currentLevel + " totalLevels: " + _maxLevel);
        if (_currentLevel == _maxLevel)
        {
            Debug.Log("Game completed");
            SceneManager.LoadScene("Final");
            return;
        }
        _currentLevel = Mathf.Min(_currentLevel + 1, _maxLevel);
        PiggiesToAdd = _level.PiggiesReward;
        _player.ReceiveCorn(_level.TotalFoodCollected);
        _level = null;
        GoToEvolutionsScene();
    }

    void StartEvolution()
    {
        _evolutionsMenu = FindAnyObjectByType<EvolutionsMenu>();
        // Debug.Log("Starting evolutions" + PiggiesToAdd + " " + _evolutionsMenu);
        if (PiggiesToAdd > 0)
        {
            StartCoroutine(_evolutionsMenu.QueueNewPigs(PiggiesToAdd));
            PiggiesToAdd = 0;
        }
        _state = GameState.Upgrade;
        Debug.Log("Starting evolutions");
        _evolutionsMenu.OnEvolutionsEnd += ExitEvolutions;
    }

    void ExitEvolutions()
    {
        Debug.Log("Exiting evolutions");
        _evolutionsMenu = null;
        GoToNextLevelScene();
    }

    void GoToEvolutionsScene()
    {
        _setupedThisScene = false;
        SceneManager.LoadScene("Evolutions");
    }

    void GoToNextLevelScene()
    {
        _setupedThisScene = false;
        if (SceneManager.GetSceneByName("Level" + _currentLevel) != null)
            SceneManager.LoadScene("Level" + _currentLevel);
        else
        {
            Debug.LogError("No more levels, goin to last one");
            SceneManager.LoadScene("Level" + (_currentLevel - 1));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            OnMouseClick?.Invoke(worldPosition);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            OnKeyNumberPressed?.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            OnKeyNumberPressed?.Invoke(2);
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
            OnKeyNumberPressed?.Invoke(3);
    }

    public IEnumerable<PiggyData> GetFreePigWithData(PiggyType type, int rank)
    {
        var allPigs = _player.GetPigsWithData(type, rank);
         return allPigs.Where(p => !Game.Instance.Level.IsPiggySummoned(p.Name));
    }

    public void UpgradePiggyRank(PiggyData piggyData)
    {
        _player.UpgradePiggyRank(piggyData);
    }

    public void ChangePiggyType(PiggyData piggyData, PiggyType type)
    {
        _player.ChangePiggyType(piggyData, type);
    }

    public PiggyData BuyNewPiggy()
    {
        return _player.AddNewPiggy();
    }

    public void PiggyGotSomeFood(int food)
    {
        _player.ReceiveCorn(food);
    }
}
