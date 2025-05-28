namespace Models
{
    public class ActiveRateEffect
    {
        public float RateAmount;
        public float RemainingDuration;

        public ActiveRateEffect(float rateAmount, float duration)
        {
            RateAmount = rateAmount;
            RemainingDuration = duration;
        }
        
    }
}