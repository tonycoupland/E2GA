using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;

public class TspFitness : IFitness
{

    public TspFitness(int numberOfCities)
    {
        Cities = new List<TspCity>(numberOfCities);

        for (int i = 0; i < numberOfCities; i++)
        {
            var city = new TspCity { Position = GetCityRandomPosition() };
            Cities.Add(city);
        }
    }

    public IList<TspCity> Cities { get; private set; }
   
    public double Evaluate(IChromosome chromosome)
    {
        var genes = chromosome.GetGenes();
        var distanceSum = 0.0;
        var lastCityIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
        var citiesIndexes = new List<int>();
        citiesIndexes.Add(lastCityIndex);

        // Calculates the total route distance.
        foreach (var g in genes)
        {
            var currentCityIndex = Convert.ToInt32(g.Value, CultureInfo.InvariantCulture);
            distanceSum += CalcDistanceTwoCities(Cities[currentCityIndex], Cities[lastCityIndex]);
            lastCityIndex = currentCityIndex;

            citiesIndexes.Add(lastCityIndex);
        }

        distanceSum += CalcDistanceTwoCities(Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);

        var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));

        ((TspChromosome)chromosome).Distance = distanceSum;

        // There is repeated cities on the indexes?
        var diff = Cities.Count - citiesIndexes.Distinct().Count();

        if (diff > 0)
        {
            fitness /= diff;
        }

        if (fitness < 0)
        {
            fitness = 0;
        }

        return fitness;
    }

    private (float, float) GetCityRandomPosition()
    {
        return (
            RandomizationProvider.Current.GetFloat(0, 1000),
            RandomizationProvider.Current.GetFloat(0, 1000));
    }

    private static double CalcDistanceTwoCities(TspCity one, TspCity two)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(one.Position.Item1 - two.Position.Item1), 2) +
                         Math.Pow(Math.Abs(one.Position.Item2 - two.Position.Item2), 2));
    }
}