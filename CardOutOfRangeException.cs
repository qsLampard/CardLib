﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardLib
{
    public class CardOutOfRangeException : Exception
    {
        private Cards deckContents;

        public Cards DeckContents
        {
            get
            {
                return deckContents;
            }
        }

        public CardOutOfRangeException(Cards sourceDeckContents) :
            base("There are only 54 cards in the deck.")
        {
            deckContents = sourceDeckContents;
        }
    }
}
