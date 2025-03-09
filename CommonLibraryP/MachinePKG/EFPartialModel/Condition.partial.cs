using CommonLibraryP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.MachinePKG
{
    public partial class Condition
    {
        public Condition()
        {

        }
        public Condition(Guid id)
        {
            Id = id;
        }

        public bool NodesValid => ConditionNodes.Count is 1 && ConditionNodes.All(x=>x.AllValid);


        public bool conditionMatch = false;
        public bool ConditionMatch => conditionMatch;

        private DateTime lastCheckTime = DateTime.Now;
        public DateTime LastCheckTime => lastCheckTime;

        private DateTime lastTriggetTime = DateTime.Now;
        public DateTime LastTriggetTime => lastTriggetTime;

        private bool triggered = false;
        public bool Triggered => triggered;

        Thread t;
        private volatile bool monitorFlag = true;

        public Action? UIUpdateAct;
        private void UIUpdate()
            => UIUpdateAct?.Invoke();

        public void StartMonitorThread(MachineService machineService)
        {
            t = new(async () => await Monitor(machineService));
            t.IsBackground = true;
            t.Start();
        }

        public void StopMonitorThread()
        {
            monitorFlag = false;
            t.Join();
        }

        private async Task Monitor(MachineService machineService)
        {
            while (monitorFlag)
            {
                lastCheckTime = DateTime.Now;
                if (Enable && NodesValid)
                {
                    var rootVal = ConditionNodes.FirstOrDefault().GetNodeValue(machineService);
                    conditionMatch = rootVal.Equals(true);
                    if (conditionMatch)
                    {
                        foreach (var command in ConditionActions)
                        {
                            await command.RunCommand(machineService);
                        }
                        triggered = true;
                        lastTriggetTime = DateTime.Now;
                    }
                    else
                    {
                        triggered = false;
                    }
                    UIUpdate();
                }
                await Task.Delay(500);
            }
        }
    }
}
