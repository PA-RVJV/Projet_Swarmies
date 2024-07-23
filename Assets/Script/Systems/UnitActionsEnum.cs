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
                    return "Barrack";
                case UnitActionsEnum.ConstruireEntrepot:
                    return "Storage";
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
    
    public class GetIcone
    {
        public static string Get(UnitActionsEnum actionsEnum)
        {
            switch (actionsEnum)
            {
                case UnitActionsEnum.ConstruireCaserne:
                    return "Construire une caserne";
                case UnitActionsEnum.ConstruireEntrepot:
                    return "Construire un entrepot";
                case UnitActionsEnum.ConvertirEnWarriors:
                    return "Fabriquer des guerriers";
                case UnitActionsEnum.ConvertirEnShooters:
                    return "Fabriquer des tireurs";
                case UnitActionsEnum.ConvertirEnHealers:
                    return "Fabriquer des Healeurs";
                case UnitActionsEnum.ConvertirEnTanks :
                    return "Fabriquer des tanks";
                case UnitActionsEnum.Demolir:
                    return "Démolir";
                case UnitActionsEnum.PausePlayProduction:
                    return "Mettre en pause/marche la production";
            }

            return "Not defined in UnitActionsEnum";
        }
    }
}