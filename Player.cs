using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections;

namespace CardLib
{
  [Serializable]
  public class Player : INotifyPropertyChanged
  {
    public int Index { get; set; }
    public bool Landlord { get; set; }
    protected Cards Hand { get; set; }
    protected Cards Chosen { get; set; }
    private string _name;
    private PlayerState _state;

    public Player()
    {
        Landlord = false;
    }
    //public event EventHandler<CardEventArgs> OnCardDiscarded;
    public event EventHandler<CardEventArgs> OnCardPlayed;
    public event EventHandler<PlayerEventArgs> OnPlayerHasWon;

    public PlayerState State
    {
      get
      {
        return _state;
      }
      set
      {
        _state = value;
        OnPropertyChanged("State");
      }
    }

    public virtual string PlayerName
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
        OnPropertyChanged("PlayerName");
      }
    }

    public void AddCard(Card card)
    {
      Hand.Add(card);
      //if (Hand.Count > 7)
        //State = PlayerState.MustDiscard;
    }

    public void ChooseCard(Card card)
    {
        if (Chosen.Contains(card))
            Chosen.Remove(card);
        else
            Chosen.Add(card);
        

    }
    public kindsOfCombination validCards()
    {
        if (Chosen.Count == 0)
            return null;
        if (Chosen.Count == 1)    //single card
            return new kindsOfCombination(Kind.Single,0,(int)Chosen[0].rank);
        Chosen.Sort();
        if (Chosen.Count == 2)
        {
            if (Chosen[0].rank == (Rank)16 && Chosen[1].rank == (Rank)17)
                return new kindsOfCombination(Kind.Bomb, 0, (int)Chosen[0].rank);    //a kind
            if (Chosen[0].rank == Chosen[1].rank)
                return new kindsOfCombination(Kind.Pair, 0, (int)Chosen[0].rank);    //pair
            return null;
        }
        if (Chosen.Count == 3)
        {
            if (Chosen[0].rank == Chosen[1].rank && Chosen[0].rank == Chosen[2].rank)
                return new kindsOfCombination(Kind.Three, 0, (int)Chosen[0].rank);   //3 a kind
            return null;
        }
        if (Chosen.Count == 4)
        {
            if (Chosen[0].rank == Chosen[1].rank && Chosen[0].rank == Chosen[2].rank && Chosen[0].rank == Chosen[3].rank)
                return new kindsOfCombination(Kind.Bomb, 0, (int)Chosen[0].rank);    //a kind
            if (Chosen[0].rank == Chosen[1].rank && Chosen[0].rank == Chosen[2].rank)
                return new kindsOfCombination(Kind.Three, 1, (int)Chosen[0].rank);   //3 a kind
            if (Chosen[1].rank == Chosen[2].rank && Chosen[1].rank == Chosen[3].rank)
                return new kindsOfCombination(Kind.Three, 1, (int)Chosen[1].rank);   //3 a kind
            return null;
        }
        if (Chosen.Count == 5)
        {
            if (Chosen[0].rank == Chosen[1].rank && Chosen[0].rank == Chosen[2].rank && Chosen[3].rank == Chosen[4].rank)
                return new kindsOfCombination(Kind.Three, 2, (int)Chosen[0].rank);    //full house
            if (Chosen[0].rank == Chosen[1].rank && Chosen[2].rank == Chosen[3].rank && Chosen[2].rank == Chosen[4].rank)
                return new kindsOfCombination(Kind.Three, 2, (int)Chosen[2].rank);   //full house
        }
        if (Chosen.Count >= 6 && Chosen.Count % 2 == 0)
        {   
            bool straightPair=true;
            if(Chosen[0].rank != Chosen[1].rank)
                straightPair=false;
            for(int i=2;i<Chosen.Count % 2;i+=2)
                if (Chosen[i].rank != (Chosen[i - 1].rank +1)|| Chosen[i].rank != Chosen[i + 1].rank)
                {
                    straightPair = false;
                    break;
                }
            if (straightPair && Chosen[Chosen.Count-1].rank<=(Rank)14)
                return new kindsOfCombination(Kind.StraightPair, Chosen.Count % 2, (int)Chosen[0].rank);   //StraightPair
        }
        bool straight = true;
        for (int i = 1; i < Chosen.Count ; i ++)
            if (Chosen[i].rank != Chosen[i - 1].rank+1)
            {
                straight = false;
                break;
            }
        if (straight && Chosen[Chosen.Count - 1].rank <= (Rank)14)
            return new kindsOfCombination(Kind.Straight, Chosen.Count, (int)Chosen[0].rank);   //Straight
        return null;
    }

    public void DrawCard(Deck deck)
    {
      AddCard(deck.Draw());
    }
    /*
    public void DiscardCard(Card card)
    {
      Hand.Remove(card);
      if (HasWon && OnPlayerHasWon != null)
        OnPlayerHasWon(this, new PlayerEventArgs { Player = this, State = PlayerState.Winner });
      if (OnCardDiscarded != null)
        OnCardDiscarded(this, new CardEventArgs { Card = card });
    }
      */
    public void play(kindsOfCombination k)
    {
        kindsOfCombination result = validCards();
        if (result == null)
            return;
        if (!result.biggerThan(k))
            return;
        for (int i = 0; i < Chosen.Count;i++ )
            Hand.Remove(Chosen[i]);
        CardEventArgs args = new CardEventArgs();
        args.Card = new Card((Suit)1,(Rank)3);
        args.Cards = (Cards)Chosen.Clone();
        args.index = Index;
        args.CurrentCombination = result;
        OnCardPlayed(this, args); 
        Chosen.Clear();
        if (HasWon && OnPlayerHasWon != null)
            OnPlayerHasWon(this, new PlayerEventArgs { Player = this, State = PlayerState.Winner });
    }

    public void pass(int CardsPlayer)
    {
        if (Index == CardsPlayer)
            return;
        Chosen.Clear();
        this._state = PlayerState.Pass;
        OnCardPlayed(this, new CardEventArgs { Cards=new Cards() }); 
    }

    public void DrawNewHand(Deck deck)
    {
      Hand = new Cards();
      Chosen = new Cards();
      for (int i = 0; i < 17; i++)
        Hand.Add(deck.Draw());
      if (Landlord)
      {
          for (int i = 0; i < 3; i++)
              Hand.Add(deck.Draw());
      }
      Hand.Sort();
    }


    public bool HasWon
    {
        get { return Hand.Count == 0; } 
        /*
      get
      {
        if (Hand.Count == 7)
        {
          var suit = Hand[0].suit;
          for (int i = 1; i < Hand.Count; i++)
            if (suit != Hand[i].suit)
              return false;
          return true;
        }
        return false;
      }*/
    }

    public Cards GetCards()
    {
      return Hand.Clone() as Cards;
    }

    public Cards GetChosenCards()
    {
        return Chosen.Clone() as Cards;
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }

}
