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
	[StaticConstructorOnStartup]
	public class Command_SetTargetPowerLevel : Command
	{
		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			if (this.chargeables == null)
			{
				this.chargeables = new List<PBL_Chargeable>();
			}
			if (!this.chargeables.Contains(this.chargeable))
			{
				this.chargeables.Add(this.chargeable);
			}
			int num = int.MaxValue;
			for (int i = 0; i < this.chargeables.Count; i++)
			{
				if ((int)this.chargeables[i].Props.maxPowerConsumption < num)
				{
					num = (int)this.chargeables[i].Props.maxPowerConsumption;
				}
			}
			int startingValue = 0;
			for (int j = 0; j < this.chargeables.Count; j++)
			{
				if ((int)this.chargeables[j].PowerConsumption <= num)
				{
					startingValue = (int)this.chargeables[j].PowerConsumption;
					break;
				}
			}
			Func<int, string> textGetter;
			textGetter = ((int x) => "SetTargetChargeLevel".Translate(x));
			Dialog_Slider window = new Dialog_Slider(textGetter, 100, num, delegate (int value)
			{
				for (int k = 0; k < this.chargeables.Count; k++)
				{
					this.chargeables[k].PowerConsumption = (float)value;
				}
			}, startingValue);
			Find.WindowStack.Add(window);
		}

		public override bool InheritInteractionsFrom(Gizmo other)
		{
			if (this.chargeables == null)
			{
				this.chargeables = new List<PBL_Chargeable>();
			}
			this.chargeables.Add(((Command_SetTargetPowerLevel)other).chargeable);
			return false;
		}

		public PBL_Chargeable chargeable;

		private List<PBL_Chargeable> chargeables;
	}
}
