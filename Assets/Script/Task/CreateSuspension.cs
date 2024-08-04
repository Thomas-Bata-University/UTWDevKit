using Script.Enum;
using Script.Static;

namespace Script.Task {
    public class CreateSuspension : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Suspension;
            partType = TankPartType.Suspension;
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