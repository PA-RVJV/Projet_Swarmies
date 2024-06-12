namespace Script
{
    public enum UnitActionsEnum
    {
        Construire,
    }

    public class GetText
    {
        public static string Get(UnitActionsEnum actionsEnum)
        {
            switch (actionsEnum)
            {
                case UnitActionsEnum.Construire:
                    return "Construire";
            }

            return "Not defined in UnitActionsEnum";
        }

    }
}