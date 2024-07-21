namespace Script
{
    public enum UnitActionsEnum
    {
        ConstruireCaserne,
        ConstruireEntrepot,
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
                    return "";
                case UnitActionsEnum.ConstruireEntrepot:
                    return "";
                case UnitActionsEnum.ConvertirEnWarriors:
                    return "";
                case UnitActionsEnum.ConvertirEnShooters:
                    return "";
                case UnitActionsEnum.ConvertirEnHealers:
                    return "";
                case UnitActionsEnum.ConvertirEnTanks :
                    return "";
                case UnitActionsEnum.Demolir:
                    return "";
                case UnitActionsEnum.PausePlayProduction:
                    return "";
            }

            return "Not defined in UnitActionsEnum";
        }

    }
}