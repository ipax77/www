using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace www.pwa.Server.Models
{
    public class WwwWalk
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float TotalDistance { get; set; }
        public DateTime Start { get; set; }
        public float TotalRuns { get; set; }
        public int TotalEntities { get; set; } = 0;
        public bool isActive { get; set; }
        public virtual ICollection<WwwSchool> WwwSchools { get; set; }
    }

    public class WwwSchool
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float TotalRuns { get; set; }
        public int TotalEntities { get; set; } = 0;
        public virtual WwwWalk WwwWalk { get; set; }
        public virtual ICollection<WwwClass> WwwClasses { get; set; }
    }

    public class WwwClass
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public float TotalRuns { get; set; }
        public int TotalEntities { get; set; } = 0;
        public virtual WwwSchool WwwSchool { get; set; }
        public virtual ICollection<WwwEntity> WwwEntities { get; set; }
    }

    public class WwwEntity
    {
        public int ID { get; set; }
        public string Pseudonym { get; set; }
        public float TotalRuns { get; set; }
        public int Runs { get; set; } = 0;
        public virtual WwwClass WwwClass { get; set; }
        public virtual ICollection<WwwRun> WwwRuns { get; set; }
    }

    public class WwwRun
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public float Distance { get; set; }
        public virtual WwwEntity WwwEntity { get; set; }
    }

    public class WwwCounter
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class WwwCounterQueue
    {
        public int ID { get; set; }
        public string CounterName { get; set; }
        public int ValueChange { get; set; }
    }

}
