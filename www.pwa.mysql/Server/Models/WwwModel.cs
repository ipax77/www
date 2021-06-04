using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using www.pwa.Server.Services;
using www.pwa.Shared;

namespace www.pwa.Server.Models
{
    public class WwwWalk
    {
        public int ID { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public float TotalDistance { get; set; }
        public DateTime Start { get; set; }
        public float TotalRuns { get; set; }
        public int TotalEntities { get; set; } = 0;
        public bool isActive { get; set; }
        public WwwWalkData NextPoint { get; set; }
        public string Credential { get; set; }
        public virtual ICollection<WwwSchool> WwwSchools { get; set; }
        public virtual ICollection<WwwWalkData> Points { get; set; }
        public virtual ICollection<WalkSponsor> Sponsors { get; set; }
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
        public bool hasSponsors { get; set; } = false;
        public virtual WwwClass WwwClass { get; set; }
        public virtual ICollection<WwwRun> WwwRuns { get; set; }
        public virtual ICollection<EntitySponsor> Sponsors { get; set; }
    }

    public class WwwRun
    {
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public float Distance { get; set; }
        public virtual WwwEntity WwwEntity { get; set; }
    }

    public class WwwSponsor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double CentPerKm { get; set; }
        public bool Verified { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class EntitySponsor : WwwSponsor
    {
        public WwwEntity Entity { get; set; }
    }

    public class WalkSponsor : WwwSponsor
    {
        public WwwWalk Walk { get; set; }
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
