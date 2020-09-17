using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NameOMatic.Database;
using NameOMatic.Extensions;
using System.Text.RegularExpressions;

namespace NameOMatic.Formats.DB2
{
    internal class ChrCustomization : IDB2
    {
        public IDictionary<int, string> FileNames { get; }

        public bool IsValid
        {
            get
            {
                return DBContext.Instance["ChrCustomizationMaterial"] != null &&
                       DBContext.Instance["ChrCustomizationElement"] != null &&
                       DBContext.Instance["ChrCustomizationChoice"] != null &&
                       DBContext.Instance["ChrCustomizationOption"] != null &&
                       DBContext.Instance["ChrRaceXChrModel"] != null &&
                       DBContext.Instance["ChrRaces"] != null &&
                       DBContext.Instance["ChrModel"] != null &&
                       DBContext.Instance["TextureFileData"] != null;
            }
        }

        public ChrCustomization() => FileNames = new Dictionary<int, string>();

        public void Enumerate()
        {
            var chrmat = DBContext.Instance["ChrCustomizationMaterial"];
            var chrele = DBContext.Instance["ChrCustomizationElement"];
            var chrchoice = DBContext.Instance["ChrCustomizationChoice"];
            var chropt = DBContext.Instance["ChrCustomizationOption"];
            var racexmodel = DBContext.Instance["ChrRaceXChrModel"];
            var races = DBContext.Instance["ChrRaces"];
            var model = DBContext.Instance["ChrModel"];
            var textureFileData = DBContext.Instance["TextureFileData"];
            var listfile = ListFile.Instance;

            var temp = from tfd in textureFileData
                       join cm in chrmat on tfd.Value.FieldAs<int>("MaterialResourcesID") equals cm.Value.FieldAs<int>("MaterialResourcesID")
                       join ce in chrele on cm.Key equals ce.Value.FieldAs<int>("ChrCustomizationMaterialID")
                       join cc in chrchoice on ce.Value.FieldAs<int>("ChrCustomizationChoiceID") equals cc.Key
                       join co in chropt on cc.Value.FieldAs<int>("ChrCustomizationOptionID") equals co.Key
                       join rm in racexmodel on co.Value.FieldAs<int>("ChrModelID") equals rm.Key
                       join r in races on rm.Value.FieldAs<int>("ChrRacesID") equals r.Key
                       where !listfile.ContainsKey(tfd.Key)

                       select new Record
                       {
                           FileDataId = tfd.Key,
                           Race = r.Value.Field<string>("ClientFileString"),
                           Model = co.Value.FieldAs<int>("ChrModelID"),
                           Choice = cc.Key,
                           AltChoice = ce.Value.FieldAs<int>("RelatedChrCustomizationChoiceID"),
                           Option = co.Value.Field<string>("Name_lang"),
                           OrderIndex = cc.Value.FieldAs<int>("OrderIndex").ToString("D2"),
                       };

            foreach (var records in temp.GroupBy(x => x.FileDataId))
            {
                var entries = records.ToArray();

                var result = new Record()
                {
                    FileDataId = entries[0].FileDataId
                };

                for (var i = 0; i < entries.Length; i++)
                {
                    if ((result.Race ??= entries[i].Race) != entries[i].Race)
                        result.Race = "";

                    if (model.TryGetValue(entries[i].Model, out var modelrow))
                    {
                        var gender = modelrow.FieldAs<bool>("Sex") ? "female" : "male";
                        if ((result.Gender ??= gender) != gender)
                            result.Gender = "";
                    }
                    else
                    {
                        result.Gender = "";
                    }


                    if (chrchoice.TryGetValue(entries[i].AltChoice, out var choicerow))
                    {
                        entries[i].OrderIndex += "_" + choicerow.FieldAs<int>("OrderIndex").ToString("D2");

                        if (chropt.TryGetValue(choicerow.FieldAs<int>("ChrCustomizationOptionID"), out var optrow))
                        {
                            var option2 = optrow.Field<string>("Name_lang");
                            if (!string.IsNullOrEmpty(option2))
                                entries[i].Option = option2;
                        }
                    }

                    if ((result.Option ??= entries[i].Option) != result.Option)
                        result.Option = "";
                    if ((result.OrderIndex ??= entries[i].OrderIndex) != result.OrderIndex)
                        result.OrderIndex = "";
                }

                var filename = $"character/{result.Race}/{result.Gender}/{result.Race}{result.Gender}_{result.Option}_{result.OrderIndex}_{result.FileDataId}.blp";
                FileNames[result.FileDataId] = filename.Replace("//", "/").Replace(" ", "_").Replace("__", "_");
            }
        }

        private class Record
        {
            public int FileDataId { get; set; }
            public string Race { get; set; }
            public int Model { get; set; }
            public int Choice { get; set; }
            public int AltChoice { get; set; }
            public string Option { get; set; }
            public string OrderIndex { get; set; }
            public string Gender { get; set; }
        }
    }
}
