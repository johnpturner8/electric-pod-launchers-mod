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
    public class PBL_Chargeable : ThingComp
	{
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_SetTargetPowerLevel
            {
                chargeable = this,
                defaultLabel = "CommandSetTargetChargeLevel".Translate(),
                defaultDesc = "CommandSetTargetChargeLevelDesc".Translate(),
                icon = PBL_Chargeable.SetTargetPowerLevelCommand
            };
            yield break;
        }

        public float PowerConsumption
        {
            get
            {
                return powerConsumption;
            }
            set
            {
                this.powerConsumption = Mathf.Clamp(value, Props.minPowerConsumption, Props.maxPowerConsumption);
            }
        }

        //Saves power consumption settings
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref this.powerConsumption, "powerConsumption");
        }

        public PBL_ChargeableProperties Props => (PBL_ChargeableProperties)this.props;

        private float powerConsumption = 100f;

        public float minPowerConsumption => Props.minPowerConsumption;
        public float maxPowerConsumption => Props.maxPowerConsumption;

        private static readonly Texture2D SetTargetPowerLevelCommand = ContentFinder<Texture2D>.Get("SetPower", true);
    }

    public class PBL_ChargeableProperties : CompProperties
    {
        public float minPowerConsumption;
        public float maxPowerConsumption;

        public PBL_ChargeableProperties()
        {
            this.compClass = typeof(PBL_Chargeable);
        }

        public PBL_ChargeableProperties(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}

