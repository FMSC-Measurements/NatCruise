using CruiseDAL.Schema;
using FluentAssertions;
using FMSC.Sampling;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Core.Test.Logic
{
    public class TreeBasedTallyLogic_test
    {
        //[Theory]
        ////WHEN frequency 1 in 1 (guarintee sample)
        ////AND no insurance trees
        ////THEN expect measure tree
        //[InlineData("M")]

        ////WHEN frequency is 1 in 2 
        ////AND insurance frequency is 1 in 1
        //[InlineData("I")]//if freq is 1 sampler wont do insurance //TODO it would be nice to have a better way to guarntee a insurance sample

        //[InlineData("C")]
        //public void TallyStandard(string resultCountMeasure)
        //{
        //    var samplerMock = new Mock<SampleSelecter>();

        //    if (resultCountMeasure == "M")
        //    {
        //        samplerMock.Setup(x => x.NextItem()).Returns(new boolItem(0, false, true));
        //    }
        //    else if (resultCountMeasure == "I")
        //    {
        //        samplerMock.Setup(x => x.NextItem()).Returns(new boolItem(0, true, true));
        //    }
        //    else
        //    {
        //        samplerMock.Setup(x => x.NextItem()).Returns((boolItem)null);
        //    }

        //    var sampleResult = new TallyEntry();
        //    var nonSampleResult = new TallyEntry();

        //    var dataServiceMock = new Mock<ICuttingUnitDataService>();
        //    dataServiceMock.Setup(x => x.CreateTally(It.IsAny<TallyPopulation>(), It.Is<int>(i => i == 1), It.Is<int>(i => i == 0), It.Is<bool>(b => b == false)))
        //        .Returns(nonSampleResult);
        //    dataServiceMock.Setup(x => x.CreateTallyWithTree(It.IsAny<TallyPopulation>(), It.IsAny<string>(), It.Is<int>(i => i == 1), It.Is<int>(i => i == 0), It.Is<bool>(b => b == false)))
        //        .Returns(sampleResult);

        //    string stratumCode;
        //    string sgCode;
        //    string liveDead;
        //    string species;
        //    var tallyPopulation = new TallyPopulation()
        //    {
        //        StratumCode = stratumCode = "st1",
        //        SampleGroupCode = sgCode = "sg1",
        //        LiveDead = liveDead = "L",
        //        Species = species = "sp1",
        //        Sampler = samplerMock.Object
        //    };

        //    var tallyEntry = TreeBasedTallyLogic.TallyStandard(tallyPopulation, dataServiceMock.Object);

        //    if (resultCountMeasure == "C")
        //    {
        //        dataServiceMock.Verify(x => x.CreateTally(It.IsAny<TallyPopulation>(), It.Is<int>(i => i == 1), It.Is<int>(i => i == 0), It.Is<bool>(b => b == false)));
        //    }
        //    else
        //    {
        //        dataServiceMock.Verify(x => x.CreateTallyWithTree(It.IsAny<TallyPopulation>(), It.IsAny<string>(), It.Is<int>(i => i == 1), It.Is<int>(i => i == 0), It.Is<bool>(b => b == false)));
        //    }
        //}

        //[Theory(Skip = "not implemented")]
        //[InlineData(2, 0, "M")]
        //public void TallyThreeP(int kpi, int insuranceFreq, string resultCountMeasure)
        //{
        //    //var unit = new CuttingUnit();

        //    //var sampleGroup = new SampleGroup
        //    //{
        //    //    Sampler = new FMSC.Sampling.ThreePSelecter(1, 1, 0)
        //    //};

        //    //var count = new CountTree()
        //    //{
        //    //    CuttingUnit_CN = 1
        //    //};

        //    //var pop = new TallyPopulation
        //    //{
        //    //    Method = "3P",
        //    //    SampleGroup = sampleGroup,
        //    //    Count = count
        //    //};


        //    //var dataServiceMock = new Mock<ICuttingUnitDataService>();
        //    //dataServiceMock.Setup(x => x.CreateTree(It.IsAny<TallyPopulation>())).Returns(() => new Tree());

        //    //ICuttingUnitDataService dataService = dataServiceMock.Object;

        //    //var tallyEntry = TreeBasedTallyLogic.TallyThreeP(pop, kpi, dataService);
        //    //var tree = tallyEntry.Tree;
        //    //tree.KPI.Should().Be(kpi);
        //    //tree.CountOrMeasure.Should().Be(resultCountMeasure);
        //}

        //[Fact(Skip = "redo")]
        //public void TallyThreeP_STM()
        //{
        //    //int kpi = -1;

        //    //var unit = new CuttingUnit();
        //    //var sampleGroup = new SampleGroup
        //    //{
        //    //    Sampler = new FMSC.Sampling.ThreePSelecter(0, 0, 0)
        //    //};
        //    //var count = new CountTree()
        //    //{
        //    //    CuttingUnit_CN = 1
        //    //};
        //    //var pop = new TallyPopulation
        //    //{
        //    //    Method = "3P",
        //    //    SampleGroup = sampleGroup,
        //    //    Count = count
        //    //};


        //    //var dataServiceMock = new Mock<ICuttingUnitDataService>();
        //    //dataServiceMock.Setup(x => x.CreateTree(It.IsAny<TallyPopulation>())).Returns(() => new Tree());

        //    //ICuttingUnitDataService dataService = dataServiceMock.Object;

        //    //var tallyEntry = TreeBasedTallyLogic.TallyThreeP(pop, kpi, dataService);
        //    //var tree = tallyEntry.Tree;
        //    //tree.STM.Should().Be("Y");
        //    //tree.KPI.Should().Be(0);
        //    //tree.CountOrMeasure.Should().Be("M");
        //}


    //    private CruiseDAL.DAL CreateDatastore(string cruiseMethod, int freqORkz, int insuranceFreq)
    //    {
    //        var ds = new CruiseDAL.DAL();
    //        try
    //        {
    //            var sale = new SaleDO()
    //            {
    //                DAL = ds,
    //                SaleNumber = "12345",
    //                Region = "1",
    //                Forest = "1",
    //                District = "1",
    //                Purpose = "something",
    //                LogGradingEnabled = true
    //            };
    //            sale.Save();

    //            var stratum = new StratumDO()
    //            {
    //                DAL = ds,
    //                Code = "01",
    //                Method = cruiseMethod
    //            };
    //            stratum.Save();

    //            var cuttingUnit = new CuttingUnitDO()
    //            {
    //                DAL = ds,
    //                Code = "01"
    //            };
    //            cuttingUnit.Save();

    //            var cust = new CuttingUnitStratumDO()
    //            {
    //                DAL = ds,
    //                CuttingUnit = cuttingUnit,
    //                Stratum = stratum
    //            };
    //            cust.Save();

    //            var sampleGroup = new SampleGroupDO()
    //            {
    //                DAL = ds,
    //                Stratum = stratum,
    //                Code = "01",
    //                PrimaryProduct = "01",
    //                UOM = "something",
    //                CutLeave = "something",
    //                InsuranceFrequency = insuranceFreq
    //            };

    //            if (CruiseMethods.THREE_P_METHODS.Contains(cruiseMethod))
    //            {
    //                sampleGroup.KZ = freqORkz;
    //            }
    //            else
    //            {
    //                sampleGroup.SamplingFrequency = freqORkz;
    //            }

    //            sampleGroup.Save();

    //            var tally = new TallyDO()
    //            {
    //                DAL = ds,
    //                Hotkey = "A",
    //                Description = "something"
    //            };
    //            tally.Save();

    //            var count = new CountTreeDO()
    //            {
    //                DAL = ds,
    //                CuttingUnit = cuttingUnit,
    //                SampleGroup = sampleGroup,
    //                Tally = tally
    //            };
    //            count.Save();

    //            return ds;
    //        }
    //        catch
    //        {
    //            ds.Dispose();
    //            throw;
    //        }
    //    }
    }
}
