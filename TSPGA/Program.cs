using System;
using System.Diagnostics;
using System.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Infrastructure.Framework.Threading;

namespace TSPGA
{
    class Program
    {
        private static GeneticAlgorithm m_ga;
        private static Thread m_gaThread;
        public static int m_numberOfCities = 250;

        static void Main(string[] args)
        {

            var fitness = new TspFitness(m_numberOfCities);
            var chromosome = new TspChromosome(m_numberOfCities);

            // This operators are classic genetic algorithm operators that lead to a good solution on TSP,
            // but you can try others combinations and see what result you get.
            var crossover = new GeneticSharp.Domain.Crossovers.OrderedCrossover();
            var mutation = new GeneticSharp.Domain.Mutations.ReverseSequenceMutation();
            var selection = new GeneticSharp.Domain.Selections.RouletteWheelSelection();
            var population = new GeneticSharp.Domain.Populations.Population(500, 1000, chromosome);
       
            m_ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            m_ga.Termination = new GeneticSharp.Domain.Terminations.TimeEvolvingTermination(System.TimeSpan.FromHours(1));

            // The fitness evaluation of whole population will be running on parallel.
            m_ga.TaskExecutor = new ParallelTaskExecutor
            {
                MinThreads = 100,
                MaxThreads = 200
            };

            // Every time a generation ends, we log the best solution.
            m_ga.GenerationRan += delegate
            {
                var distance = ((TspChromosome)m_ga.BestChromosome).Distance;
                Console.WriteLine($"Generation: {m_ga.GenerationsNumber} - Distance: ${distance}");
            };

            // Starts the genetic algorithm in a separate thread.
            m_gaThread = new Thread(() => m_ga.Start());
            m_gaThread.Start();

            
            Console.ReadLine();
            
            
            // When the script is destroyed we stop the genetic algorithm and abort its thread too.
            m_ga.Stop();
            m_gaThread.Abort();
        }
    }
}