using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace E2GA
{
    /// <summary>
    /// Chromosome represents a list of symbols in order to populate the puzzle
    /// </summary>
    public class E2Chromosome :ChromosomeBase
    {
        // Problem statics
        public static string SymbolsList = "AAAABBBBBBBCCCCCCCCCCCCCDDDDDDEEEEEEEEEFFFFFFFFFGGGGGGGHHHHH";
        public static int Width;
        public static int Height;
        public static int TileCount;        
        public static IEnumerable<Tile> Tiles { get; set; }

        
        private readonly int NumberOfSymbols;
        public int Matches;
        public double FitnessValue;
        
        public E2Chromosome() : base((2 * Width * Height) + (Width + Height))
        {
            // For each tile in rows and columns up to last one we have two new symbols
            // for the last row and column, we have four symbols
            NumberOfSymbols = (2 * Width * Height) + (Width + Height);
            var symbolsIndexes = RandomizationProvider.Current.GetUniqueInts(NumberOfSymbols, 0, NumberOfSymbols);
            
            for (int i = 0; i < NumberOfSymbols; i++)
            {
                ReplaceGene(i, new Gene(SymbolsList[symbolsIndexes[i]]));
            }
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new System.NotImplementedException();
        }

        public override IChromosome CreateNew()
        {
            return new E2Chromosome();
        }

        public string ToSymbolList()
        {
            return String.Concat(this.GetGenes().Select(g => (char) g.Value));
        }

        public string ToSortedSymbolList()
        {
            return String.Concat(this.GetGenes().Select(g => (char) g.Value).OrderBy(c => c));
        }
        
        public override string ToString()
        {
            return this.ToStringIncludingTileMatches(false);
        }
        
        public string ToStringIncludingTileMatches(bool includeMatches)
        {
            // Build visual representation of chromosome
            // eg for a 3x3, i.e. 
            //  X X X  
            // XTXTXTX
            //  X X X
            // XTXTXTX
            //  X X X
            // XTXTXTX
            //  X X X    
            var genes = this.GetGenes();
            StringBuilder sb = new StringBuilder((Width * 2 + 1) * (Height * 2 + 1));
            bool shortLine = true;
            int i = 0;
            int tileIndex = 0;
            var tileMatches = includeMatches ? MatchedTiles().ToHashSet() : null;
            while (i < NumberOfSymbols)
            {
                if (shortLine)sb.Append(" ");

                for (var n = 0; n < Width; n++)
                {
                    sb.Append(genes[i++].ToString());
                    // If this is a long line, then we need to inject the tile match flag
                    if (!shortLine)
                    {
                        if (tileMatches != null && tileMatches.Contains(tileIndex))
                        {
                            sb.Append("X");
                        }
                        else
                        {
                            sb.Append("_");
                        }

                        tileIndex++;
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }

                if (!shortLine) sb.Append(genes[i++].ToString());

                sb.Append("\n");
                shortLine = !shortLine;
            }

            return sb.ToString();
        }

        public static IEnumerable<string> CutTiles(E2Chromosome chromosome)
        {
            // generate set of strings representing the tiles cut from the candidate solution
            for (var y = 0; y < E2Chromosome.Height; y++)
            {
                for (var x = 0; x < E2Chromosome.Width; x++)
                {
                    int root = y * (E2Chromosome.Width * 2 + 1) + x;
                    yield return
                        $"{(char) chromosome.GetGene(root + E2Chromosome.Width).Value}{(char) chromosome.GetGene(root).Value}{(char) chromosome.GetGene(root + E2Chromosome.Width + 1).Value}{(char) chromosome.GetGene(root + (E2Chromosome.Width * 2 + 1)).Value}";
                }
            }
        }

        public IEnumerable<int> MatchedTiles()
        {            
            // Cut tiles from chromosome
            var cutTiles = E2Chromosome.CutTiles(this);
            
            // Create a copy of the real tiles for us to work with
            var comp = Tiles.ToList();
            var tileIndex = 0;

            foreach (var tile in cutTiles)
            {
                // Walk our own list of all the real tiles...
                for (var n = 0; n < comp.Count; n++)
                {
                    if (comp[n].Matches(tile))
                    {
                        yield return tileIndex;
                        comp.RemoveAt(n);
                        break;
                    }
                }

                tileIndex++;
            }

        }
    }
}