using System;
using System.Collections.Generic;
using System.Text;

namespace Workflows.Models
{
    public class ApprovalResult
    {
        public string OrchestrationId { get; set; }
        public bool Approved { get; set; }
    }
}
