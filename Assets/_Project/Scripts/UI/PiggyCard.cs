using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PiggyCard : MonoBehaviour
{
    [SerializeField] PiggyData _data;
    [SerializeField] Image _avatar;

    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _rankText;
    [SerializeField] TextMeshProUGUI _healthText;
    [SerializeField] TextMeshProUGUI _speedText;
    [SerializeField] TextMeshProUGUI _fattinessText;
    [SerializeField] TextMeshProUGUI _upgradeCostText;
    [SerializeField] TextMeshProUGUI _makeFatCostText;
    [SerializeField] TextMeshProUGUI _makeFastCostText;

    [SerializeField] Button _upgradeButton;
    [SerializeField] Button _makeFatButton;
    [SerializeField] Button _makeFastButton;


    public PiggyData Data => _data;


    public void SetData(PiggyData data)
    {
        Debug.Log($"Setting data for {data.Name}");
        _data = data;

        _avatar.sprite = _data.Sprite;
        _nameText.text = _data.Name;
        // _rankText.text = _data.Rank.ToString();
        _healthText.text = _data.Health.ToString();
        _speedText.text = _data.Speed.ToString();
        _fattinessText.text = _data.Fattiness.ToString();

        _upgradeCostText.text = _data.UpgradeCost.ToString();

        Debug.Log($"Type: {_data.Type}, Rank: {_data.Rank}");
        if (data.Type == PiggyType.Normal && data.Rank == 0)
        {
            Debug.Log("Normal rank 0");
            _makeFatButton.interactable = true;
            _makeFastButton.interactable = true;
            _makeFatCostText.text = Game.Instance.PiggyEvolutions.MakeFatCost.ToString();
            _makeFastCostText.text = Game.Instance.PiggyEvolutions.MakeFastCost.ToString();
        }
        else
        {
            Debug.Log("Not normal rank 0");
            _makeFatCostText.text = "";
            _makeFastCostText.text = "";
            _makeFatButton.interactable = false;
            _makeFastButton.interactable = false;
        }
        if (data.Rank == 2)
        {
            _upgradeButton.interactable = false;
            _upgradeCostText.text = "";
        }
        else
        {
            _upgradeButton.interactable = true;
        }
    }
}
