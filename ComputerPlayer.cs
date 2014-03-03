using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CardLib
{
  [Serializable]
  public class ComputerPlayer : Player
  {
    private Random _random = new Random();

    public ComputerSkillLevel Skill { get; set; }
    public static ArrayList names = new ArrayList { "Pep Guardiola", "José Mourinho", "Carlo Ancelotti", "Arsène Wenger", "Jürgen Klopp", "Rafael Benítez" };
    string _PlayerName;

    public ComputerPlayer()
    {
        Random rand = new Random();
        int index = rand.Next(names.Count);
        _PlayerName = (String)names[index];
        names.RemoveAt(index);
    }

    public static void ResetPlayerNames()
    {
        names = new ArrayList { "Pep Guardiola", "José Mourinho", "Carlo Ancelotti", "Arsène Wenger", "Jürgen Klopp", "Rafael Benítez" };
    }

    public override string PlayerName
    {
      get {

          return _PlayerName; 
      }
      set { /* Do nothing, name is calculated */ }
    }

    public void Perform(kindsOfCombination combination, int CardPlayer)
    {
      switch (Skill)
      {
        case ComputerSkillLevel.Easy:
          decision(combination,CardPlayer);//DrawCard(deck);
          break;
        default:
          pass(0);//DrawBestCard(deck, availableCard, (Skill == ComputerSkillLevel.Hard));
          break;
      }
    }
/*
    public void PerformDiscard(Deck deck)
    {
      switch (Skill)
      {
        case ComputerSkillLevel.Easy:
          int discardIndex = _random.Next(Hand.Count);
          DiscardCard(Hand[discardIndex]);
          break;
        default:
          DiscardWorstCard();
          break;
      }
    }
*/

    private void decision(kindsOfCombination combination, int CardPlayer)
    {
        if(Index == CardPlayer)
            combination=null;
        if (combination == null)  //free to play
            Chosen.Add(Hand[0]);
        else
        {
            if (combination.kind == Kind.Single)
                DealwithSingle(combination, CardPlayer);
            else if (combination.kind == Kind.Pair)
                DealwithPair(combination, CardPlayer);
            else if (combination.kind == Kind.Three)
                DealwithThree(combination, CardPlayer);
            else if (combination.kind == Kind.Bomb)
                DealwithBomb(combination, CardPlayer);
            else if (combination.kind == Kind.Straight)
                DealwithStraight(combination, CardPlayer);
            else if (combination.kind == Kind.StraightPair)
                DealwithStraightPair(combination, CardPlayer);
        }
        if (Chosen.Count > 0)
            play(null);
        else
            pass(CardPlayer);
    }


    private void DealwithSingle(kindsOfCombination combination, int CardPlayer)
    {
        foreach (Card c in Hand)
        {
            if (c.rank > (Rank)combination.index)
            {
                Chosen.Add(c);
                break;
            }
        }
    }

    private void DealwithPair(kindsOfCombination combination, int CardPlayer)
    {
        for (int i = 0; i < Hand.Count - 1; i++)
        {
            if (Hand[i].rank == Hand[i + 1].rank && Hand[i].rank > (Rank)combination.index)
            {
                Chosen.Add(Hand[i]);
                Chosen.Add(Hand[i + 1]);
                break;
            }
        }
    }

    private void DealwithThree(kindsOfCombination combination, int CardPlayer)
    {
        for (int i = 0; i < Hand.Count - 2; i++)
        {
            if (Hand[i].rank == Hand[i + 1].rank && Hand[i].rank == Hand[i + 2].rank && Hand[i].rank > (Rank)combination.index)
            {
                Chosen.Add(Hand[i]);
                Chosen.Add(Hand[i + 1]);
                Chosen.Add(Hand[i + 2]);
                break;
            }
        }
        if (combination.assist == 1)
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (!Chosen.Contains(Hand[i]))
                {
                    Chosen.Add(Hand[i]);
                    break;
                }
            }
            if (Chosen.Count != 4)
                Chosen.Clear();
        }
        else if (combination.assist == 2)
        {
            for (int i = 0; i < Hand.Count - 1; i++)
            {
                if (!Chosen.Contains(Hand[i]) && !Chosen.Contains(Hand[i + 1]))
                {
                    Chosen.Add(Hand[i]);
                    Chosen.Add(Hand[i + 1]);
                    break;
                }
            }
            if (Chosen.Count != 5)
                Chosen.Clear();
        }
        if (Chosen.Count == 0 && combination.index >= 10)   //use bomb
            DealwithBomb(combination, CardPlayer);
    }

    private void DealwithBomb(kindsOfCombination combination, int CardPlayer)
    {
        for (int i = 0; i < Hand.Count - 3; i++)
        {
            if (Hand[i].rank == Hand[i + 1].rank && Hand[i].rank == Hand[i + 2].rank && Hand[i].rank == Hand[i + 3].rank && Hand[i].rank > (Rank)combination.index)
            {
                Chosen.Add(Hand[i]);
                Chosen.Add(Hand[i + 1]);
                Chosen.Add(Hand[i + 2]);
                Chosen.Add(Hand[i + 3]);
                break;
            }
        }
        if (Chosen.Count != 4 && Hand.Count >= 2 && Hand[Hand.Count - 1].rank == Rank.Joker && Hand[Hand.Count - 2].rank == Rank.joker)
        {
            Chosen.Add(Hand[Hand.Count - 1]);
            Chosen.Add(Hand[Hand.Count - 2]);
        }
    }
      
    private void DealwithStraight(kindsOfCombination combination, int CardPlayer)
    {
        for (int i = 0; i < Hand.Count ; i++)
        {
            if (Chosen.Count == 0)
            {
                if (Hand[i].rank > (Rank)combination.index)
                    Chosen.Add(Hand[i]);
            }
            else if (Hand[i].rank == Chosen.Last().rank + 1 && Hand[i].rank < (Rank)15)
                Chosen.Add(Hand[i]);
            else if (Hand[i].rank > Chosen.Last().rank + 1)
                Chosen.Clear();
            if (Chosen.Count == combination.assist)
                break;
        }
        if (Chosen.Count != combination.assist)
            Chosen.Clear();
        if (Chosen.Count == 0 && combination.assist >= 7)   //use bomb
            DealwithBomb(combination, CardPlayer);
    }

    private void DealwithStraightPair(kindsOfCombination combination, int CardPlayer)
    {
        bool chooseFirst = true;
        for (int i = 0; i < Hand.Count - 3; i++)
        {
            if (Chosen.Count == 0)
            {
                if (Hand[i].rank > (Rank)combination.index)
                    Chosen.Add(Hand[i]);
            }
            else if (chooseFirst&&Hand[i].rank == Chosen.Last().rank && Hand[i].rank < (Rank)15)
            {
                chooseFirst = false;
                Chosen.Add(Hand[i]);
            }
            else if (chooseFirst && Hand[i].rank > Chosen.Last().rank && Hand[i].rank < (Rank)15)
                Chosen.Clear();
            else if (!chooseFirst && Hand[i].rank == Chosen.Last().rank + 1 && Hand[i].rank < (Rank)15)
            {
                chooseFirst = true;
                Chosen.Add(Hand[i]);
            }
            else if (!chooseFirst&&Hand[i].rank > Chosen.Last().rank + 1)
                Chosen.Clear();
            if (Chosen.Count == combination.assist)
                break;
        }
        if (Chosen.Count != combination.assist*2)
            Chosen.Clear();
        if (Chosen.Count == 0 && combination.assist >= 4)   //use bomb
            DealwithBomb(combination, CardPlayer);
    }

    private void DrawBestCard(Deck deck, Card availableCard, bool cheat = false)
    {
      var bestSuit = CalculateBestSuit();
      if (availableCard.suit == bestSuit)
        AddCard(availableCard);
      else if (cheat == false)
        DrawCard(deck);
      else
        AddCard(deck.SelectCardOfSpecificSuit(bestSuit));
    }

    private void DiscardWorstCard()
    {
      var worstSuit = CalculateWorstSuit();
      foreach (Card card in Hand)
      {
        if (card.suit == worstSuit)
        {
          //DiscardCard(card);
          break;
        }
      }
    }

    private Suit CalculateBestSuit()
    {
      Dictionary<Suit, List<Card>> cardSuits = new Dictionary<Suit, List<Card>>();
      cardSuits.Add(Suit.Club, new List<Card>());
      cardSuits.Add(Suit.Diamond, new List<Card>());
      cardSuits.Add(Suit.Heart, new List<Card>());
      cardSuits.Add(Suit.Spade, new List<Card>());
      int max = 0;
      Suit currentSuit = Suit.Club;

      foreach (Card card in Hand)
      {
        cardSuits[card.suit].Add(card);
        if (cardSuits[card.suit].Count > max)
        {
          max = cardSuits[card.suit].Count;
          currentSuit = card.suit;
        }
      }
      return currentSuit;
    }

    private Suit CalculateWorstSuit()
    {
      Dictionary<Suit, List<Card>> cardSuits = new Dictionary<Suit, List<Card>>();
      cardSuits.Add(Suit.Club, new List<Card>());
      cardSuits.Add(Suit.Diamond, new List<Card>());
      cardSuits.Add(Suit.Heart, new List<Card>());
      cardSuits.Add(Suit.Spade, new List<Card>());
      int min = Hand.Count;
      Suit currentSuit = Suit.Club;

      foreach (Card card in Hand)
      {
        cardSuits[card.suit].Add(card);
      }
      foreach (var item in cardSuits)
      {
        if (item.Value.Count > 0 && item.Value.Count < min)
        {
          min = item.Value.Count;
          currentSuit = item.Key;
        }
      }
      return currentSuit;
    }
  }

}
