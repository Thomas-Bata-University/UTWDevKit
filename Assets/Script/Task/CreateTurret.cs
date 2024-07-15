using Script.Static;

namespace Script.Task {
    public class CreateTurret : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Turret;
        }

        protected override void StartImpl() {
            Create();
        }

        protected override void UpdateImpl() {
        }

        public override void Create() {
            EnableMaxCheck(base.Create);
        }

    }
} //END