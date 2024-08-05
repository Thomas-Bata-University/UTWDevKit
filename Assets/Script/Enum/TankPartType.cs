using System.Collections.Generic;

namespace Script.Enum {
    public enum TankPartType {

        //Main parts
        Hull,
        Turret,
        Suspension,
        Weaponry,

        //Others
        Plate

    }

    public class MainParts {

        public static List<TankPartType> GetMainParts() {
            return new List<TankPartType>()
                { TankPartType.Hull, TankPartType.Turret, TankPartType.Suspension, TankPartType.Weaponry };
        }

    }
} //END