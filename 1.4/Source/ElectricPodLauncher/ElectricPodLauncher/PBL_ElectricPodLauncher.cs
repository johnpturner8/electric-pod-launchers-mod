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
    public class PBL_ElectricPodLauncher : Building_PodLauncher
    {
		//public override void SpawnSetup(Map map, bool respawningAfterLoad)
  //      {
		//	base.SpawnSetup(map, respawningAfterLoad);

		//	float powerConsumption = this.TryGetComp<PBL_Chargeable>().PowerConsumption;
		//	this.TryGetComp<CompPowerTrader>().PowerOutput = -1 * powerConsumption;
		//}


		public override void RareTick()
		{
			//Debug.Log("Running 1.3");
			float powerConsumption = this.TryGetComp<PBL_Chargeable>().PowerConsumption;
			this.TryGetComp<CompPowerTrader>().PowerOutput = -1 * powerConsumption;
			if (this.TryGetComp<CompPowerTrader>().PowerOn)
			{
				this.TryGetComp<CompRefuelable>().Refuel(chargePerTick * 250 * PBL_ElectricPodLauncher_Settings.chargeScalings * (powerConsumption / 1000 / 1000));
			}
		}
		
		//0.02 is roughly as efficient as a fueled launcher
		public float chargePerTick = 0.1f;
	}
}
