using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class SummonCard : MonoBehaviour
{
    [SerializeField] Image _avatar;
    [SerializeField] List<GameObject> _stars;
    [SerializeField] TextMeshProUGUI _quantity;
    [SerializeField] TextMeshProUGUI _name;

    public event Action<PiggyData> OnCardClicked;
    public int Quantity { get; private set; }

    PiggyData _data;

    

    public void SetupCard(PiggyData data, int quantity, string name)
    {
        _avatar.sprite = data.Avatar;
        _quantity.text = quantity.ToString();
        _name.text = name;
        for (int i = 0; i < 3; i++)
        {
            _stars[i].SetActive(i <= data.Rank);
        }
        Quantity = quantity;
        _data = data;
        GetComponent<Button>().onClick.AddListener(ClickOnCard);
    }

    void ClickOnCard()
    {
        if (Quantity > 0)
        {
            OnCardClicked?.Invoke(_data);
            Quantity--;
            _quantity.text = Quantity.ToString();
            if (Quantity == 0)
            {
                _name.text = "";
                GetComponent<Button>().interactable = false;
            }
            else
            {
                var piggy = Game.Instance.GetFreePigWithData(_data.Type, _data.Rank).Where(p => p.Name != _name.text).First();
                _data = piggy;
                _name.text = piggy.Name;
            }
        }
    }
}
