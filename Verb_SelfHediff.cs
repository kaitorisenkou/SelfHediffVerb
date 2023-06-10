using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace SelfHediffVerb
{
    // Token: 0x020018AE RID: 6318
    public class Verb_SelfHediff : Verb {
        protected override bool TryCastShot() {
            if (!(verbProps is VerbProperties_SelfHediff)) {
                Log.Error("Verb_SelfHediff must have VerbProperties_SelfHediff!");
                return false;
            }
            if (CasterIsPawn) {
                var compReloadable = base.ReloadableCompSource;
                if(compReloadable!=null) {
                    if (!compReloadable.CanBeUsed) return false;
                    compReloadable.UsedOnce();
                }
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
    }
}
