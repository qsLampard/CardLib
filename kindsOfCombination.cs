using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardLib
{
    public class kindsOfCombination
    {
        public Kind kind;
        public int assist;   //for number of three;straight;straightPair;four
        public int index;
        public kindsOfCombination(Kind k,int a,int i)
        {
            kind = k;
            assist = a;
            index = i;
        }

        bool bigger(kindsOfCombination koc)
        {
            if (this.kind == Kind.Bomb)
            {
                if (koc.kind != Kind.Bomb)
                    return false;
                return koc.index>this.index;
            }
            if (koc.kind == Kind.Bomb)
                return true;
            if (koc.kind != this.kind || koc.assist != this.assist)
                return false;
            return koc.index > this.index;
        }
    }
    public enum Kind
   {
      
      Single=1,
      Pair,
      Three,
      Bomb,
      Straight,
      StraightPair,
      Four,//
      Plane//
   }
}
