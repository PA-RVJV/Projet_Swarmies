namespace Script
{
    public enum UnitActionsEnum
    {
        Construire,
        ConstruireEntrepot,
        ConvertirEnWarriors,
        ConvertirEnShooters,
        ConvertirEnHealers,
    }

    public class GetText
    {
        public static string Get(UnitActionsEnum actionsEnum)
        {
            switch (actionsEnum)
            {
                case UnitActionsEnum.Construire:
                    return "+ Caserne";
                case UnitActionsEnum.ConstruireEntrepot:
                    return "+ Entrepot";
                case UnitActionsEnum.ConvertirEnWarriors:
                    return "+ Warriors";
                case UnitActionsEnum.ConvertirEnShooters:
                    return "+ Shooters";
                case UnitActionsEnum.ConvertirEnHealers:
                    return "+ Healers";
            }

            return "Not defined in UnitActionsEnum";
        }

    }
}