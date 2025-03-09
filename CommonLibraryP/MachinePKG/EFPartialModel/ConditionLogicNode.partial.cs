using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class ConditionLogicNode
    {
        public override bool ContentValid => ChildNodes.Count is 2;

        public override string NameAndContent => $"{Name}({(LogicalOperation)LogicalOperation})";

        public ConditionLogicNode() : base()
        {

        }
        public ConditionLogicNode(Guid conditionId)
        {
            ConditionId = conditionId;
        }

        public override object GetNodeValue(MachineService machineService)
        {
            if (ContentValid)
            {
                var childs = new List<ConditionNode>(ChildNodes);
                var LChild = childs[0];
                var RChild = childs[1];
                bool res = LogicalOperation switch
                {
                    1 => LChild.GetNodeValue(machineService).Equals(RChild.GetNodeValue(machineService)),
                    2 => !LChild.GetNodeValue(machineService).Equals(RChild.GetNodeValue(machineService)),
                    _ => false,
                };
                return res;
            }
            return false;

        }
    }
}
