﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Verse;
using RimWorld;

namespace SelfHediffVerb {
    public class CompVerbWithCooltime : ThingComp, IVerbOwner {
        private int remainCooltimeTicks = -1;
        public CompProperties_VerbWithCooltime PropsVWC => props as CompProperties_VerbWithCooltime;

        public override void CompTick() {
            base.CompTick();
            if (this.remainCooltimeTicks >= 0) {
                this.remainCooltimeTicks--;
            }
        }
        public void UsedOnce() {
            remainCooltimeTicks = PropsVWC.ticksCooldown;
        }
        public bool CanBeUsed {
            get {
                return remainCooltimeTicks < 0;
            }
        }

        private VerbTracker verbTracker;
        public VerbTracker VerbTracker {
            get {
                if (this.verbTracker == null) {
                    this.verbTracker = new VerbTracker(this);
                }
                return this.verbTracker;
            }
        }

        public List<VerbProperties> VerbProperties => this.parent.def.Verbs;

        public List<Tool> Tools => this.parent.def.tools;

        public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;

        public Thing ConstantCaster => Wearer;
        Pawn Wearer {
            get {
                Pawn_ApparelTracker pawn_ApparelTracker = this.ParentHolder as Pawn_ApparelTracker;
                if (pawn_ApparelTracker != null) {
                    return pawn_ApparelTracker.pawn;
                }
                return null;
            }
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra() {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra()) {
                yield return gizmo;
            }
            if (parent.TryGetComp<CompReloadable>() != null) yield break;

            ThingWithComps gear = this.parent;
            foreach (Verb verb in this.VerbTracker.AllVerbs) {
                if (verb.verbProps.hasStandardCommand) {
                    if (verb.caster == null) {
                        verb.caster = Wearer as Thing;
                    }
                    yield return this.CreateVerbTargetCommand(gear, verb);
                }
            }
            yield break;
        }
        private Command_VerbTarget CreateVerbTargetCommand(Thing gear, Verb verb) {
            Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
            command_VerbTarget.verb = verb;
            var verbProp = verb.verbProps;
            if (verbProp.label != null) {
                command_VerbTarget.defaultDesc = gear.def.description;
            }
            if (verb.verbProps.defaultProjectile != null) {
                command_VerbTarget.icon = verb.verbProps.defaultProjectile.uiIcon;
            }
            if (!this.Wearer.IsColonistPlayerControlled) {
                command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
            } else if (verb.verbProps.violent && this.Wearer.WorkTagIsDisabled(WorkTags.Violent)) {
                command_VerbTarget.Disable("IsIncapableOfViolenceLower".Translate(this.Wearer.LabelShort, this.Wearer).CapitalizeFirst() + ".");
            }
            return command_VerbTarget;
        }

        public override void PostExposeData() {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.remainCooltimeTicks, "remainCooltimeTicks", -1, false);
        }

        public override string CompInspectStringExtra() {
            var result = base.CompInspectStringExtra();
            if (this.remainCooltimeTicks >= 0) {
                result += "Cooltime: " + remainCooltimeTicks.ToStringSecondsFromTicks();
            }
            return result;
        }

        public string UniqueVerbOwnerID() {
            throw new NotImplementedException();
        }

        public bool VerbsStillUsableBy(Pawn p) {
            throw new NotImplementedException();
        }
    }
    public class CompProperties_VerbWithCooltime : CompProperties {
        public int ticksCooldown = 60;

        public CompProperties_VerbWithCooltime() {
            this.compClass = typeof(CompVerbWithCooltime);
        }
    }
}
