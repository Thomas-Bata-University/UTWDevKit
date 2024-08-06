using Script.Enum;
using Script.Static;

namespace Script.Task {
    public class CreateTurret : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Turret;
            partType = TankPartType.TURRET;
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