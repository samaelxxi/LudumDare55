using System.Collections;
using System.Collections.Generic;
using SingularityGroup.HotReload;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class EvolutionsMenu : MonoBehaviour
{
    [SerializeField] PeacefulPiggy _piggyPrefab;
    [SerializeField] GameObject _piggiesHome;
    [SerializeField] BoxCollider2D _piggiesHomeCollider;
    [SerializeField] PiggyCard _piggyCard;
    [SerializeField] TMPro.TextMeshProUGUI _cornText;
    [SerializeField] SpriteRenderer _selectedArrow;
    [SerializeField] Button _buyNewPiggyButton;
    [SerializeField] TMPro.TextMeshProUGUI _newPiggyCostText;

    List<PeacefulPiggy> _piggies = new();
    PeacefulPiggy _selectedPiggy;

    public event Action OnEvolutionsEnd;

    void Start()
    {
        CreatePeacefulPiggies();
        SelectPiggy(_piggies[Random.Range(0, _piggies.Count)]);
        Game.Instance.OnCornChanged += (corn) => _cornText.text = corn.ToString();
        _cornText.text = Game.Instance.PlayerCorn.ToString();
        _newPiggyCostText.text = Game.Instance.NewPiggyCost.ToString();

        if (Game.Instance.PiggiesQuantity == Game.Instance.PiggyEvolutions.MaxPiggies)
            _buyNewPiggyButton.interactable = false;

        Game.Instance.OnMouseClick += OnMouseClick;
    }

    void OnDestroy()
    {
        Game.Instance.OnMouseClick -= OnMouseClick;
    }

    void Update()
    {
        _selectedArrow.transform.position = _selectedPiggy.transform.position + Vector3.up * 1;
    }

    public void OnMouseClick(Vector3 mousePos)
    {
        // Debug.Log($"Mouse clicked at {mousePos}");
        var probablyPiggy = Physics2D.OverlapCircle(mousePos, 0.5f, LayerMask.GetMask("Piggy"));
        // Debug.Log($"Probably piggy: {probablyPiggy}");
        if (probablyPiggy != null && probablyPiggy.TryGetComponent<PeacefulPiggy>(out var reallyPiggy))
        {
            _piggyCard.SetData(reallyPiggy.Data);
            _selectedPiggy = reallyPiggy;
            // Debug.Log($"Piggy clicked: {probablyPiggy.name}");
        }
    }

    void SelectPiggy(PeacefulPiggy piggy)
    {
        _piggyCard.SetData(piggy.Data);
        _selectedPiggy = piggy;
    }

    void CreatePeacefulPiggies()
    {
        foreach (var piggyData in Game.Instance.PlayerPiggies())
        {
            // Debug.Log($"Creating piggy {piggyData.Name}");
            var piggy = CreateNewPiggy();
            piggy.SetData(piggyData);
            piggy.SetWalkArea(_piggiesHomeCollider);
            _piggies.Add(piggy);
        }
    }

    PeacefulPiggy CreateNewPiggy()
    {
        Vector3 position = default;
        int i = 0;
        while (i < 100)
        {
        position = new Vector3(Random.Range(_piggiesHomeCollider.bounds.min.x, _piggiesHomeCollider.bounds.max.x),
                                Random.Range(_piggiesHomeCollider.bounds.min.y, _piggiesHomeCollider.bounds.max.y), 0);
            if (Physics2D.OverlapCircle(position, 0.5f, LayerMask.GetMask("Piggy")) == null) break;
            i++;
        }
        var piggy = Instantiate(_piggyPrefab, position, Quaternion.identity, _piggiesHome.transform);
        return piggy;
    }

    public void EndEvolutions()
    {
        OnEvolutionsEnd?.Invoke();
    }

    public void BuyNewPiggy()
    {
        Log.Info("BuyNewPiggy");
        if (Game.Instance.CanBuyNewPiggy)
        {
            var newPiggy = Game.Instance.BuyNewPiggy();
            var newPiggyAvatar = CreateNewPiggy();
            newPiggyAvatar.SetData(newPiggy);
            newPiggyAvatar.SetWalkArea(_piggiesHomeCollider);

            if (Game.Instance.PiggiesQuantity == Game.Instance.PiggyEvolutions.MaxPiggies)
                _buyNewPiggyButton.interactable = false;
            _newPiggyCostText.text = Game.Instance.NewPiggyCost.ToString();
        }
        else
            Debug.Log("Not enough corn or a lot piggies");
    }

    public void UpgradeRank()
    {
        Log.Info("UpgradeRank");
        if (_selectedPiggy == null) return;
        if (Game.Instance.PlayerCorn >= _selectedPiggy.Data.UpgradeCost)
        {
            Debug.Log($"Changing piggy type to fast {_selectedPiggy.Data.Rank}");
            Game.Instance.UpgradePiggyRank(_selectedPiggy.Data);
            Debug.Log($"Changing piggy type to fast {_selectedPiggy.Data.Rank}");
            _piggyCard.SetData(_selectedPiggy.Data);
            _selectedPiggy.UpdateData();
            Log.Info("Rank upgraded");
        }
        else
            Debug.Log("Not enough corn");
    }

    public void MakeFat()
    {
        Log.Info("MakeFat");
        if (_selectedPiggy == null) return;
        if (Game.Instance.PlayerCorn >= Game.Instance.PiggyEvolutions.MakeFatCost)
        {
            Game.Instance.ChangePiggyType(_selectedPiggy.Data, PiggyType.Fat);
            _piggyCard.SetData(_selectedPiggy.Data);
            _selectedPiggy.UpdateData();
        }
        else
            Debug.Log("Not enough corn");
    }

    public void MakeFast()
    {
        Log.Info("MakeFast");
        if (_selectedPiggy == null) return;
        if (Game.Instance.PlayerCorn >= Game.Instance.PiggyEvolutions.MakeFastCost)
        {
            Debug.Log($"Changing piggy type to fast {_selectedPiggy.Data.Speed}");
            Game.Instance.ChangePiggyType(_selectedPiggy.Data, PiggyType.Fast);
            Debug.Log($"Changing piggy type to fast {_selectedPiggy.Data.Speed}");
            _piggyCard.SetData(_selectedPiggy.Data);
            _selectedPiggy.UpdateData();
        }
        else
            Debug.Log("Not enough corn");
    }
}
