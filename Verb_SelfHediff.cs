using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace SelfHediffVerb {
    public class Verb_SelfHediff : Verb {
        protected override bool TryCastShot() {
            if (!(verbProps is VerbProperties_SelfHediff)) {
                Log.Error("Verb_SelfHediff must have VerbProperties_SelfHediff!");
                return false;
            }
            if (CasterIsPawn) {
                var compReloadable = base.ReloadableCompSource;
                var compCooltime = EquipmentSource.TryGetComp<CompVerbWithCooltime>();
                if((compCooltime != null && !compCooltime.CanBeUsed)) {
                    Messages.Message("SelfHediffVerb_CooltimeRemain".Translate(compCooltime.remainCooltimeTicks.ToStringSecondsFromTicks("F0")), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                if ((compReloadable != null && !compReloadable.CanBeUsed)) {
                    return false;
                }
                compReloadable?.UsedOnce();
                compCooltime?.UsedOnce();

                var props = ((VerbProperties_SelfHediff)verbProps);
                var hediff = CasterPawn.health.AddHediff(props.hediffDef, CasterPawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).FirstOrFallback((BodyPartRecord p) => p.def == props.part, null));
                var hediffComp_RemoveIfApparelDropped = hediff.TryGetComp<HediffComp_RemoveIfApparelDropped>();
                if (hediffComp_RemoveIfApparelDropped != null && EquipmentSource != null && EquipmentSource is Apparel)
                    hediffComp_RemoveIfApparelDropped.wornApparel = (Apparel)this.EquipmentSource;
                return true;
            }
            return false;
        }
    }
    public class VerbProperties_SelfHediff : VerbProperties {
        public HediffDef hediffDef;
        public BodyPartDef part;
        public bool inDanger = false;
    }
}
