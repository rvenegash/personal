namespace com.dtvla.newk2view.clases
{
    public class IndividualIdentification
    {
        public string issueDate { get; set; }
        public string scan { get; set; }
        public TimePeriod validFor { get; set; }
        //desde acá son las instancias National, Internet y Fiscal
        public string cardNr { get; set; }
        public string number { get; set; }
        public string password { get; set; }
    }
}
