namespace DisprzTraining.Models
{
    public class Appointment
    {
        public Guid Id{get;set;}
        public string EventName{get;set;}=string.Empty;
        public DateTime EventDate{get;set;}
        public DateTime StartTimeHrMin{get;set;}
        public DateTime EndTimeHrMin{get;set;}
        public string DescriptionOfEvent{get;set;}=string.Empty;
    }
}
