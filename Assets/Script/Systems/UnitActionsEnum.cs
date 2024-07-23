namespace Script
{
    public enum UnitActionsEnum
    {
        ConstruireCaserne,
        ConstruireEntrepot,
        ConstruireTour,
        ConvertirEnWarriors,
        ConvertirEnShooters,
        ConvertirEnHealers,
        ConvertirEnTanks,
        Demolir,
        PausePlayProduction
    }

    public class GetText
    {
        public static string Get(UnitActionsEnum actionsEnum)
        {
            switch (actionsEnum)
            {
                case UnitActionsEnum.ConstruireCaserne:
                    return "Barrack";
                case UnitActionsEnum.ConstruireEntrepot:
                    return "Storage";
                case UnitActionsEnum.ConstruireTour:
                    return "GuardTower";
                case UnitActionsEnum.ConvertirEnWarriors:
                    return "Warriors";
                case UnitActionsEnum.ConvertirEnShooters:
                    return "Shooters";
                case UnitActionsEnum.ConvertirEnHealers:
                    return "Healeurs";
                case UnitActionsEnum.ConvertirEnTanks :
                    return "Tanks";
                case UnitActionsEnum.Demolir:
                    return "Démolir";
                case UnitActionsEnum.PausePlayProduction:
                    return "Start/Stop Production";
            }

            return "Not defined in UnitActionsEnum";
        }
    }
}