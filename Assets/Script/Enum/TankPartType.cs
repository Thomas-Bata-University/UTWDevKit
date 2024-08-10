using System.Collections.Generic;

namespace Script.Enum {
    public enum TankPartType {

        //Main parts
        HULL = 0,
        TURRET = 1,
        SUSPENSION = 2,
        WEAPONRY = 3,

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