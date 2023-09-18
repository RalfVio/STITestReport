using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADORest.BusinessObjects
{
    public class WorkItemBase
    {
        public int Id { get; set; }
        public string WorkItemType { get; set; }
        public int Rev { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public string Reason { get; set; }
        public string StateReason(string sep) {  switch (State) { case "Fixed": return State; case "Open": case "Approved": return State; case "Investigate": return State; default: return string.Format("{0}{2}{1}", State, Reason,sep); }  }
        public string TitleId { get { return string.Format("{1} ({0})", this.Id, this.Title); } }
        public string IdTitle { get { return string.Format("{0}: {1}", this.Id, this.Title); } }
        public string WITypeTitleId { get { return string.Format("{2} {1} ({0})", this.Id, this.Title,this.WorkItemType); } }
        public override string ToString() { return String.Format("{0} {1}: {2}", WorkItemType, Id, Title); }
    }
}
