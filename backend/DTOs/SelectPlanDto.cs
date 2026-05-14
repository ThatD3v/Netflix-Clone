namespace NetflixClone.DTOs
{
    public class SelectPlanDto
    {
        public int PlanId { get; set; }
        public int DurationInMonths { get; set; } = 1; // default 1 month
    }
}

