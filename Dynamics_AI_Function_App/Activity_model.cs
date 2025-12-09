using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_AI_Function_App
{
    public class CreateActivityInput
    {
        // Logical name of the activity entity: "task", "email", "phonecall", etc.
        public string ActivityType { get; set; }

        public string Subject { get; set; }
        public string Description { get; set; }

        // Regarding object (Account, Contact, Lead)
        public string RegardingEntityLogicalName { get; set; }
        public Guid RegardingEntityId { get; set; }

        // Owner of the activity (optional)
        public Guid? OwnerId { get; set; }

        // Schedule / Priority
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public int? PriorityCode { get; set; } // 1=High, 2=Normal, 3=Low

        // Recipients (for Email / Task / PhoneCall)
        public List<ActivityPartyInput> To { get; set; } = new();
        public List<ActivityPartyInput> From { get; set; } = new();
        public List<ActivityPartyInput> CC { get; set; } = new();
        public List<ActivityPartyInput> BCC { get; set; } = new();
    }

    public class ActivityPartyInput
    {
        // Logical name of the entity: "contact", "account", "systemuser", "lead"
        public string EntityLogicalName { get; set; }
        public Guid EntityId { get; set; }
    }

}
