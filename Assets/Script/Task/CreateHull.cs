using Script.Enum;
using Script.Static;
using UnityEngine;

namespace Script.Task {
    public class CreateHull : ATask {

        protected override void AwakeImpl() {
            Tag = Tags.Part.Hull;
            partType = TankPartType.Hull;
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