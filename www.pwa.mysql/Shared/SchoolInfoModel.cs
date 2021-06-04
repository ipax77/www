
using System;
using System.Collections.Generic;

namespace www.pwa.Shared
{
    public class SchoolInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Distance { get; set; }
        public List<SchoolClassInfoModel> Classes { get; set; }
    }

    public class SchoolClassInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Distance { get; set; }
        public List<EntityInfoModel> Entities { get; set; }
    }

    public class EntityInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Distance { get; set; }
        public List<EntityRunInfoModel> Runs { get; set; }

    }

    public class EntityRunInfoModel
    {
        public int Id { get; set; }
        public float Distance { get; set; }
        public DateTime Time { get; set; }
    }
}