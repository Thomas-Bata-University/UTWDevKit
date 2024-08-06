using Script.Enum;
using Script.Static;

namespace Script.Task {
    public class CreateWeaponry : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Weaponry;
            partType = TankPartType.WEAPONRY;
        }

        protected override void StartImpl() {
        }

        protected override void UpdateImpl() {
        }

        public override void Create() {
            EnableMaxCheck(base.Create);
        }

    }
} //END