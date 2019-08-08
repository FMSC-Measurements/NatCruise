using NatCruise.Wpf.Models;
using System.Collections.Generic;
using System.Linq;

namespace NatCruise.Wpf.Data
{
    public class SetupInfoDataservice : ISetupInfoDataservice
    {
        public IEnumerable<Region> Regions { get; } =
            new Region[]
            {
                new Region { RegionCode = "01", FriendlyName = "Northern",
                    Forests = new[]
                    {
                        new Forest{ ForestCode = "02", ForestName="Beaverhead-Deerlodge" },
                        new Forest{ ForestCode = "03", ForestName="Bitterroot" },
                        new Forest{ ForestCode = "04", ForestName="Idaho Panhandle" },
                        new Forest{ ForestCode = "05", ForestName="Clearwater" },
                        new Forest{ ForestCode = "08", ForestName="Custer" },
                        new Forest{ ForestCode = "10", ForestName="Flathead" },
                        new Forest{ ForestCode = "11", ForestName="Gallatin" },
                        new Forest{ ForestCode = "12", ForestName="Helena" },
                        new Forest{ ForestCode = "14", ForestName="Kootenai" },
                        new Forest{ ForestCode = "15", ForestName="Lewis & Clark" },
                        new Forest{ ForestCode = "16", ForestName="Lolo" },
                        new Forest{ ForestCode = "17", ForestName="Nezperce" },
                    }
                },

                new Region { RegionCode = "02", FriendlyName = "Rocky Mountain",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "02", ForestName = "Bighorn" },
                        new Forest { ForestCode = "03", ForestName = "Black Hills" },
                        new Forest { ForestCode = "04", ForestName = "GMUG" },
                        new Forest { ForestCode = "06", ForestName = "Medicine Bow-Routt" },
                        new Forest { ForestCode = "07", ForestName = "Nebraska" },
                        new Forest { ForestCode = "09", ForestName = "Rio Grande" },
                        new Forest { ForestCode = "10", ForestName = "Arapaho-Roosevelt" },
                        new Forest { ForestCode = "12", ForestName = "Pike-San Isabel" },
                        new Forest { ForestCode = "13", ForestName = "San Juan" },
                        new Forest { ForestCode = "14", ForestName = "Shoshone" },
                        new Forest { ForestCode = "15", ForestName = "White River" },
                    }
                },

                new Region { RegionCode ="03", FriendlyName="Southwestern",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "01", ForestName = "Apache-Sitgreaves" },
                        new Forest { ForestCode = "02", ForestName = "Carson" },
                        new Forest { ForestCode = "03", ForestName = "Cibola" },
                        new Forest { ForestCode = "04", ForestName = "Coconino" },
                        new Forest { ForestCode = "05", ForestName = "Coronado" },
                        new Forest { ForestCode = "06", ForestName = "Gila" },
                        new Forest { ForestCode = "07", ForestName = "Kaibab" },
                        new Forest { ForestCode = "08", ForestName = "Lincoln" },
                        new Forest { ForestCode = "09", ForestName = "Prescott" },
                        new Forest { ForestCode = "10", ForestName = "Santa Fe" },
                        new Forest { ForestCode = "12", ForestName = "Tonto" },
                    }
                },

                new Region { RegionCode = "04", FriendlyName = "Intermountain",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "01", ForestName = "Ashley"},
                        new Forest { ForestCode = "02", ForestName = "Boise"},
                        new Forest { ForestCode = "03", ForestName = "Bridger-Teton"},
                        new Forest { ForestCode = "07", ForestName = "Dixie"},
                        new Forest { ForestCode = "08", ForestName = "Fishlake"},
                        new Forest { ForestCode = "10", ForestName = "Manti-LaSal"},
                        new Forest { ForestCode = "12", ForestName = "Payette"},
                        new Forest { ForestCode = "13", ForestName = "Challis"},
                        new Forest { ForestCode = "13", ForestName = "Salmon"},
                        new Forest { ForestCode = "14", ForestName = "Sawtooth"},
                        new Forest { ForestCode = "15", ForestName = "Caribou-Targhee"},
                        new Forest { ForestCode = "16", ForestName = "Humboldt-Toiyabe"},
                        new Forest { ForestCode = "17", ForestName = "Uinta-Wasatch-Cache"},
                    },
                },

                new Region { RegionCode = "05", FriendlyName = "Pacific Southwest",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "01", ForestName = "Angeles"},
                        new Forest { ForestCode = "02", ForestName = "Cleveland"},
                        new Forest { ForestCode = "03", ForestName = "Eldorado"},
                        new Forest { ForestCode = "04", ForestName = "Inyo"},
                        new Forest { ForestCode = "05", ForestName = "Klamath"},
                        new Forest { ForestCode = "06", ForestName = "Lassen"},
                        new Forest { ForestCode = "07", ForestName = "Los Padres"},
                        new Forest { ForestCode = "08", ForestName = "Mendocino"},
                        new Forest { ForestCode = "09", ForestName = "Modoc"},
                        new Forest { ForestCode = "10", ForestName = "Six Rivers"},
                        new Forest { ForestCode = "11", ForestName = "Plumas"},
                        new Forest { ForestCode = "12", ForestName = "San Bernardino"},
                        new Forest { ForestCode = "13", ForestName = "Sequoia"},
                        new Forest { ForestCode = "14", ForestName = "Shasta-Trinity"},
                        new Forest { ForestCode = "15", ForestName = "Sierra"},
                        new Forest { ForestCode = "16", ForestName = "Stanislaus"},
                        new Forest { ForestCode = "17", ForestName = "Tahoe"},
                        new Forest { ForestCode = "19", ForestName = "Lake Tahoe Basin"},
                    },
                },

                new Region { RegionCode = "06", FriendlyName = "Pacific Northwest",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "01", ForestName = "Deschutes"},
                        new Forest { ForestCode = "02", ForestName = "Fremont-Winema"},
                        new Forest { ForestCode = "03", ForestName = "Gifford Pinchot"},
                        new Forest { ForestCode = "04", ForestName = "Malheur"},
                        new Forest { ForestCode = "05", ForestName = "Mt. Baker-Snoqualmie"},
                        new Forest { ForestCode = "06", ForestName = "Mt. Hood"},
                        new Forest { ForestCode = "07", ForestName = "Ochoco"},
                        new Forest { ForestCode = "09", ForestName = "Olympic"},
                        new Forest { ForestCode = "10", ForestName = "Rogue River-Siskiyou"},
                        new Forest { ForestCode = "12", ForestName = "Siuslaw"},
                        new Forest { ForestCode = "14", ForestName = "Umatilla"},
                        new Forest { ForestCode = "15", ForestName = "Umpqua"},
                        new Forest { ForestCode = "16", ForestName = "Wallowa-Whitman"},
                        new Forest { ForestCode = "17", ForestName = "Okanogan-Wenatchee"},
                        new Forest { ForestCode = "18", ForestName = "Willamette"},
                        new Forest { ForestCode = "21", ForestName = "Colville"},
                    },
                },

                new Region { RegionCode = "08", FriendlyName = "Southern",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "01", ForestName = "National Forests Alabama"},
                        new Forest { ForestCode = "02", ForestName = "Daniel Boone"},
                        new Forest { ForestCode = "03", ForestName = "Chattahoochee-Oconee"},
                        new Forest { ForestCode = "04", ForestName = "Cherokee"},
                        new Forest { ForestCode = "05", ForestName = "National Forests Florida"},
                        new Forest { ForestCode = "06", ForestName = "Kisatchie"},
                        new Forest { ForestCode = "07", ForestName = "National Forests Mississippi"},
                        new Forest { ForestCode = "08", ForestName = "George Washington-Jefferson"},
                        new Forest { ForestCode = "09", ForestName = "Ouachita"},
                        new Forest { ForestCode = "10", ForestName = "Ozark-St. Francis"},
                        new Forest { ForestCode = "11", ForestName = "National Forests N.Carolina"},
                        new Forest { ForestCode = "12", ForestName = "Francis Marion-Sumter"},
                        new Forest { ForestCode = "13", ForestName = "National Forests Texas"},
                        new Forest { ForestCode = "16", ForestName = "El Yunque"},
                        new Forest { ForestCode = "60", ForestName = "Land Between the Lakes"},
                    },
                },

                new Region { RegionCode = "09", FriendlyName = "Eastern",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "03", ForestName = "Chippewa"},
                        new Forest { ForestCode = "04", ForestName = "Huron-Manistee"},
                        new Forest { ForestCode = "05", ForestName = "Mark Twain"},
                        new Forest { ForestCode = "07", ForestName = "Ottawa"},
                        new Forest { ForestCode = "08", ForestName = "Shawnee"},
                        new Forest { ForestCode = "09", ForestName = "Superior"},
                        new Forest { ForestCode = "10", ForestName = "Hiawatha"},
                        new Forest { ForestCode = "12", ForestName = "Hoosier"},
                        new Forest { ForestCode = "13", ForestName = "Chequamegon/Nicolet"},
                        new Forest { ForestCode = "14", ForestName = "Wayne"},
                        new Forest { ForestCode = "19", ForestName = "Allegheny"},
                        new Forest { ForestCode = "20", ForestName = "Green Mountain-Finger Lakes"},
                        new Forest { ForestCode = "21", ForestName = "Monongahela"},
                        new Forest { ForestCode = "22", ForestName = "White Mountain"},
                    },
                },

                new Region { RegionCode = "10", FriendlyName = "Alaska",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "04", ForestName = "Chugach"},
                        new Forest { ForestCode = "05", ForestName = "Tongass"},
                    },
                },

                new Region { RegionCode = "07", FriendlyName = "BLM",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "00", ForestName = "-"},
                    },
                },

                new Region { RegionCode = "11", FriendlyName = "DOD",
                    Forests = new[]
                    {
                        new Forest { ForestCode = "00", ForestName = "-"},
                    },
                },
            };

        public IEnumerable<Forest> GetForests(string regionCode)
        {
            return Regions.Where(x => x.RegionCode == regionCode)
                .SingleOrDefault()?.Forests
                ?? new Forest[0];
        }

        public IEnumerable<Purpose> GetPurposes()
        {
            return new[]
            {
                new Purpose { PurposeCode = "01", FriendlyName = "Sawtimber"},
                new Purpose { PurposeCode = "02", FriendlyName = "Pulpwood"},
                new Purpose { PurposeCode = "03", FriendlyName = "Poles"},
                new Purpose { PurposeCode = "04", FriendlyName = "Pilings"},
                new Purpose { PurposeCode = "05", FriendlyName = "Mine Props"},
                new Purpose { PurposeCode = "06", FriendlyName = "Posts"},
                new Purpose { PurposeCode = "07", FriendlyName = "Fuelwood"},
                new Purpose { PurposeCode = "08", FriendlyName = "Non-sawtimber"},
                new Purpose { PurposeCode = "09", FriendlyName = "Ties"},
                new Purpose { PurposeCode = "10", FriendlyName = "Coop Bolts"},
                new Purpose { PurposeCode = "11", FriendlyName = "Acid/Dist."},
                new Purpose { PurposeCode = "12", FriendlyName = "Float Logs"},
                new Purpose { PurposeCode = "13", FriendlyName = "Trap Float"},
                new Purpose { PurposeCode = "14", FriendlyName = "Misc-Conv."},
                new Purpose { PurposeCode = "15", FriendlyName = "Christmas Trees"},
                new Purpose { PurposeCode = "16", FriendlyName = "Nav Stores"},
                new Purpose { PurposeCode = "17", FriendlyName = "Non Conv."},
                new Purpose { PurposeCode = "18", FriendlyName = "Cull Logs"},
                new Purpose { PurposeCode = "19", FriendlyName = "Sm Rnd Wd"},
                new Purpose { PurposeCode = "20", FriendlyName = "Grn Bio Cv"},
                new Purpose { PurposeCode = "21", FriendlyName = "Dry Bio Cv"},
                new Purpose { PurposeCode = "26", FriendlyName = "Sp Wood Pr"},
            };
        }

        public IEnumerable<Region> GetRegions()
        {
            return Regions;
        }

        public IEnumerable<UOM> GetUOMCodes()
        {
            return new[]
            {
                new UOM {UOMCode = "01", FriendlyName = "Board feet"},
                new UOM {UOMCode = "02", FriendlyName = "Cords"},
                new UOM {UOMCode = "03", FriendlyName = "Cubic feet"},
                new UOM {UOMCode = "04", FriendlyName = "Piece count"},
                new UOM {UOMCode = "05", FriendlyName = "Weight"},
            };
        }
    }
}