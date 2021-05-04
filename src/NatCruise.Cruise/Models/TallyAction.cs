﻿using FMSC.ORM.EntityModel.Attributes;
using FMSC.Sampling;
using System;

namespace NatCruise.Cruise.Models
{
    public class TallyAction
    {
        public enum CountOrMeasureValue { M, C, I };

        public TallyAction()
        {
        }

        protected TallyAction(TallyPopulation population)
        {
            if (population == null) { throw new ArgumentNullException(nameof(population)); }

            StratumCode = population.StratumCode;
            SampleGroupCode = population.SampleGroupCode;
            SpeciesCode = population.SpeciesCode;
            LiveDead = population.LiveDead;
        }

        public TallyAction(string unitCode, TallyPopulation population) : this(population)
        {
            CuttingUnitCode = unitCode;
        }

        public TallyAction(string unitCode, int plotNumber, TallyPopulation population) : this(unitCode, population)
        {
            PlotNumber = plotNumber;
        }

        public SamplerState SamplerState { get; set; }

        public string CuttingUnitCode { get; set; }

        public int? PlotNumber { get; set; }

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public string SpeciesCode { get; set; }

        public string LiveDead { get; set; }

        public int TreeCount { get; set; }

        public int KPI { get; set; }

        public bool STM { get; set; }

        public SampleResult SampleResult { get; set; }

        public char CountOrMeasure { get; set; }

        //random number generated by the three p sample selector,
        //for debug purposis I think, it was Matt's idea.
        //If they ever ask for this plot based three p methods just say no
        public int ThreePRandomValue { get; set; }

        public string EntryType => TallyLedger.EntryTypeValues.TALLY;

        public bool IsSample => SampleResult == SampleResult.M || SampleResult == SampleResult.I;

        public bool IsInsuranceSample => SampleResult == SampleResult.I;

        public string Initials { get; set; }
    }
}