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

        public bool biggerThan(kindsOfCombination koc)
        {
            if (koc == null)
                return true;
            if (koc.kind != Kind.Bomb && this.kind == Kind.Bomb)
                return true;
            return koc.kind == this.kind && koc.assist == this.assist&&this.index>koc.index  ;
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
