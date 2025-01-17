namespace kafkaTest
{
    public class Individual
    {
        public TimePeriodAlive aliveDuring { get; set; }
        public IndividualIdentificationCollection IdentifiedBy { get; set; }
        public IndividualNameCollection NameUsing { get; set; }
    }
}
