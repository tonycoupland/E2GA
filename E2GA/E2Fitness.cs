using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace E2GA
{
    public class E2Fitness : IFitness
    {
        
        public E2Fitness()
        {
        }
        
        public double Evaluate(IChromosome chromosome)
        {
            // Cast our chromosome to our own type
            E2Chromosome c = (E2Chromosome) chromosome;
            
            // Cut the chromosome into tiles, then compare those to the known set... How many match?
            var matchScore = c.GetMatchedTileScore();
            c.Matches = matchScore.Item1;
            if (c.Matches == 0) return 0; // No matches is a zero fit :(
            
            // Fitness (as a 0..1 number) is the completeness of the solution
            double fitness = (double)matchScore.Item2 / (double)E2Chromosome.TileCount;
            
            // How valid is this solution?
            // We could have a great number of matches, but we might have built this from an invalid symbol
            // set, i.e. too many As, not enough Bs
            
            // Sort the symbols used in this chromosome and compare item by item with the know sorted set
            // validity is the count of matches
            string solSymbols = c.ToSortedSymbolList();
            int validity = Enumerable.Range(0, solSymbols.Length - 1)
                .Count(n => solSymbols[n] == E2Chromosome.SymbolsList[n]);
            
            // If we have some invalidity then divide fitness significantly
            var diff = E2Chromosome.TileCount - validity;
            if (diff > 0)
            {
                fitness /= diff;
            }

            // Record fitness
            c.FitnessValue = fitness;
            return fitness;
        }
    }
}