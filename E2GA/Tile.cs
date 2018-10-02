using System.Linq;

namespace E2GA
{
    public class Tile
    {
        protected string original;
        protected string rot90;
        protected string rot180;
        protected string rot270;
        
        public Tile(string symbols)
        {
            original = symbols;
            rot90 = symbols.Substring(1) + symbols[0];
            rot180 = symbols.Substring(2) + symbols.Substring(0,2);
            rot270 = symbols[2] + symbols.Substring(0,3);
        }

        public bool Matches(string symbols)
        {
            // check each rotation of our symbols to see if it matches
            if (symbols == original) return true;
            if (symbols == rot90) return true;
            if (symbols == rot180) return true;
            if (symbols == rot270) return true;
            return false;
        }

        public int SymbolMatches(string symbols)
        {
            var symbolmatchcount = 0;
            var hs = symbols.ToHashSet();
            foreach (var c in original)
            {
                if (hs.Contains(c))
                {
                    hs.Remove(c);
                    symbolmatchcount++;
                }
            }

            return symbolmatchcount;
        }
    }
}