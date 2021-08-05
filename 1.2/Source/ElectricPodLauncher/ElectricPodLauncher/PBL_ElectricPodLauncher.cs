using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace ElectricPodLauncher
{
    public class PBL_ElectricPodLauncher : Building
    {
		public override void TickRare()
		{
			float powerConsumption = this.TryGetComp<PBL_Chargeable>().powerConsumption;
			if (this.TryGetComp<CompPowerTrader>().PowerOn)
			{
				this.TryGetComp<CompPowerTrader>().PowerOutput = -1 * powerConsumption;
				this.TryGetComp<CompRefuelable>().Refuel(chargePerTick * (powerConsumption / 1000));
			}
		}
		
		//0.02 is roughly as efficient as a fueled launcher
		public float chargePerTick = 0.1f;
	}
}
