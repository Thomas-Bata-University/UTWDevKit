using System.Collections.Generic;

namespace Script.Enum {
    public enum TankPartType {

        //Main parts
        HULL,
        TURRET,
        SUSPENSION,
        WEAPONRY,

        //Others
        PLATE

    }

    public class MainParts {

        public static List<TankPartType> GetMainParts() {
            return new List<TankPartType>()
                { TankPartType.HULL, TankPartType.TURRET, TankPartType.SUSPENSION, TankPartType.WEAPONRY };
        }

    }
} //END