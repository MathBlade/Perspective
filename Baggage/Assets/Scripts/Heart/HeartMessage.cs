public class HeartMessage 
{
    public class HealHeartsMessage
    {
        public int AmountToHeal { get; private set; }
        public HealHeartsMessage(int healAmount)
        {
            AmountToHeal = healAmount;
        }
    }

    public class DamageHeartsMessage
    {
        public int DamageAmount { get; private set; }
        public DamageHeartsMessage(int damageAmount)
        {
            DamageAmount = damageAmount;
        }
    }

    public class OutOfHeartsMessage { }
    public class ResetHeartsMessage { }
}
