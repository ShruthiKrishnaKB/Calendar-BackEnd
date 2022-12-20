using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.Models;

namespace DisprzTraining.DataAccess
{
    public class AppointmentDAL:IAppointmentDAL
    {
        // remainders------
        private static List<Appointment> TodayAppointment = new List<Appointment>(){
            new Appointment(){Id=Guid.NewGuid(),EventName="Daily Standup",EventDate=new DateTime(2022,12,14),StartTimeHrMin=new DateTime(2022,12,14,12,10,0),EndTimeHrMin=new DateTime(2022,12,14,12,45,0),DescriptionOfEvent="a meet with EM"},
            new Appointment(){Id=Guid.NewGuid(),EventName="Daily Catchup",EventDate=new DateTime(2022,12,14),StartTimeHrMin=new DateTime(2022,12,14,04,00,0),EndTimeHrMin=new DateTime(2022,12,14,04,30,00),DescriptionOfEvent="a meet with Mentor"}
        };
        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await Task.FromResult(TodayAppointment);
        }
        public async Task<List<Appointment>> GetAppointmentByEventName(string getEvent)
        {
            var FindEvent = TodayAppointment.Where(events => events.EventName.ToLower().Contains(getEvent.ToLower())).ToList();
            if (FindEvent.Count() == 0)
            {
                return null;
            }
            return (List<Appointment>)await Task.FromResult(FindEvent);
        }
        public async Task<List<Appointment>> GetAppointmentByEventDate(DateTime getEvent)
        {
            var FindEvent = TodayAppointment.Where(events => events.EventDate.ToShortDateString().Contains(getEvent.ToShortDateString())).ToList();
            if (FindEvent.Count() == 0)
            {
                return null;
            }
            return (List<Appointment>)await Task.FromResult(FindEvent);
        }
        public async Task<bool> CheckAlreadyExistingEvent(Appointment request){
            var CompareEvent =TodayAppointment.SingleOrDefault(events=>
                            (request.StartTimeHrMin>=events.StartTimeHrMin&&request.StartTimeHrMin<events.EndTimeHrMin) || 
                            (request.EndTimeHrMin>events.StartTimeHrMin&&request.EndTimeHrMin<=events.EndTimeHrMin) ||
                            (request.StartTimeHrMin<=events.StartTimeHrMin&&request.EndTimeHrMin>=events.EndTimeHrMin));
            if((CompareEvent==null)||(CompareEvent.Id==request.Id))
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
        public async Task<Appointment> CreateAppointment(Appointment request)
        {
            if(request.StartTimeHrMin<request.EndTimeHrMin){
                TodayAppointment.Add(request);
                return await Task.FromResult(request);
            }
            return null;
        }
        public Task DeleteAppointmentById(Guid id)
        {
            var DeleteEvent = TodayAppointment.Find(events=> events.Id.Equals(id));
            if(DeleteEvent==null)
            {
                return null;
            }
            TodayAppointment.Remove(DeleteEvent);
            return Task.CompletedTask;
        }
        public Task UpdateAnAppointment(Appointment request)
        {
            var UpdateEvent = TodayAppointment.Find(events=>events.Id==request.Id);
            if((request.StartTimeHrMin<request.EndTimeHrMin)&&(UpdateEvent != null)){
                UpdateEvent.EventName=request.EventName;
                UpdateEvent.EventDate=request.EventDate;
                UpdateEvent.StartTimeHrMin=request.StartTimeHrMin;
                UpdateEvent.EndTimeHrMin=request.EndTimeHrMin;
                UpdateEvent.DescriptionOfEvent=request.DescriptionOfEvent;
                // return await Task.FromResult(UpdateEvent);
                return Task.CompletedTask;
            }
            else{
                return null;
            }
        }
    }
}