using Script.Enum;
using Script.Static;

namespace Script.Task {
    public class CreatePlate : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Plate;
            partType = TankPartType.PLATE;
            MaxCount = 1;
        }

        protected override void StartImpl() {
        }

        protected override void UpdateImpl() {
        }

    }
} //END