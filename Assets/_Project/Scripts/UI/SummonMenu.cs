using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SummonMenu : MonoBehaviour
{
    const int CARDS_SIZE = 140;
    const int MAX_WIDTH = 1320;
    const int MIN_WIDTH = 320;
    const int HEIGHT = 164;

    [SerializeField] Transform _cardsContainer;
    [SerializeField] SummonCard _summonCardPrefab;
    

    static List<PiggyType> _typeOrder = new() { PiggyType.Normal, PiggyType.Fat, PiggyType.Fast };

    List<SummonCard> _cards = new();

    public event Action<PiggyData> OnCardClicked;

    public void Init()
    {
        CreateCards();
        SetupContainerWidth();
    }

    void SetupContainerWidth()
    {
        int cardsWidth = _cards.Count * CARDS_SIZE + (_cards.Count - 1) * 15 + 40;
        int containerWidth = Mathf.Min(cardsWidth, MAX_WIDTH);
        containerWidth = Mathf.Max(containerWidth, MIN_WIDTH);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(containerWidth, HEIGHT);
    }

    void CreateCards()
    {
        Dictionary<PiggyType, List<PiggyData>> piggiesByType = new();
        piggiesByType[PiggyType.Normal] = Game.Instance.PlayerPiggies().Where(p => p.Type == PiggyType.Normal).ToList();
        piggiesByType[PiggyType.Fat] = Game.Instance.PlayerPiggies().Where(p => p.Type == PiggyType.Fat).ToList();
        piggiesByType[PiggyType.Fast] = Game.Instance.PlayerPiggies().Where(p => p.Type == PiggyType.Fast).ToList();

        foreach (var type in _typeOrder)
        {
            for (int i = 0; i < 3; i++)
            {
                int quantity = piggiesByType[type].Where(p => p.Rank == i).Count();
                if (quantity > 0)
                {
                    SummonCard card = Instantiate(_summonCardPrefab, _cardsContainer);
                    var anyPig = Game.Instance.GetFreePigWithData(type, i).First();
                    card.SetupCard(piggiesByType[type].Where(p => p.Rank == i).First(), quantity, anyPig.Name);
                    _cards.Add(card);
                    card.OnCardClicked += OnCardClickedHandler;
                }
            }
        }
    }

    void OnCardClickedHandler(PiggyData data)
    {
        OnCardClicked?.Invoke(data);
    }
}
