using System;
using System.Linq;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace E2GA
{
    class Program
    {
        
        private static GeneticAlgorithm ga;
        private static Thread gaThread;
        
        
        static void Main(string[] args)
        {

//            var chromosome = new E2Chromosome(5, 5);
//            Console.WriteLine(chromosome.ToString());
//
//            int n = 0;
//            foreach (var cutTile in E2Chromosome.CutTiles(chromosome))
//            {
//                Console.Write(cutTile + " ");
//                if ( ++n % 5 == 0)Console.WriteLine();
//            }
//            
//            Console.ReadLine();
            
            // Setup problem
            E2Chromosome.Width = 5;
            E2Chromosome.Height = 5;
            E2Chromosome.TileCount = E2Chromosome.Width * E2Chromosome.Height;
            E2Chromosome.Tiles = new string[]
            {
                "ABDC", "DEBF", "BGGE", "GCFF", "FEED",
                "CCBF", "BFCC", "CEGG", "GFAH", "ADDC",
                "FFAC", "ACHE", "HGBC", "BHAF", "ACHG",
                "ECFC", "FEDB", "DCGB", "GFCA", "CGFD",
                "ECBE", "BBGC", "GBCH", "CAHD", "HDCE"
            }.Select(t => new Tile(t));

            // Create chromosome and fitness
            var chromosome = new E2Chromosome();
            var fitness = new E2Fitness();

            // This operators are classic genetic algorithm operators that lead to a good solution on TSP,
            // but you can try others combinations and see what result you get.
            var crossover = new GeneticSharp.Domain.Crossovers.OnePointCrossover();
            var mutation = new GeneticSharp.Domain.Mutations.PartialShuffleMutation();
            var selection = new GeneticSharp.Domain.Selections.RouletteWheelSelection();
            var population = new GeneticSharp.Domain.Populations.Population(5000, 10000, chromosome);
       
            ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.1f;
            ga.Termination = new GeneticSharp.Domain.Terminations.FitnessThresholdTermination(1.0);

            // The fitness evaluation of whole population will be running on parallel.
            ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 100,
                MaxThreads = 200
            };

            // Every time a generation ends, we log the best solution.
            int bestMatches = 0;
            DateTime startTime = DateTime.Now;
            ga.GenerationRan += delegate
            {
                var best = ((E2Chromosome) ga.BestChromosome);
                var Matches = best.Matches;
                var Fitness = best.FitnessValue;

                if (Matches > bestMatches || ga.GenerationsNumber % 250 == 0)
                {
                    Console.WriteLine(
                        $"{DateTime.Now.Subtract(startTime).TotalSeconds}s Generation: {ga.GenerationsNumber} - Matches: {Matches} - Fitness: {Fitness}");

                    if (Matches > bestMatches)
                    {
                        Console.WriteLine(((E2Chromosome) ga.BestChromosome).ToStringIncludingTileMatches(true));
                    }

                    bestMatches = Matches;
                }
            };

            // Starts the genetic algorithm in a separate thread.
            gaThread = new Thread(() => ga.Start());
            gaThread.Start();

            
            Console.ReadLine();
            
            
            // When the script is destroyed we stop the genetic algorithm and abort its thread too.
            ga.Stop();
            gaThread.Abort();            
            
        }
    }
}