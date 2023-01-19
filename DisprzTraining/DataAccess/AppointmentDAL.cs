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
            // return (FindEvent.Count() == 0) ? null:(List<Appointment>)await Task.FromResult(FindEvent);
            return FindEvent.Any() ?(List<Appointment>)await Task.FromResult(FindEvent) : null;
        }
        // public async Task<List<Appointment>> GetAppointmentByEventDate(DateTime getEvent)
        // {
        //     var FindEvent = TodayAppointment.Where(events => events.EventDate.ToShortDateString().Contains(getEvent.ToShortDateString())).ToList();
        //     // return (FindEvent.Count() == 0) ? null : (List<Appointment>)await Task.FromResult(FindEvent);
        //     return FindEvent.Any() ? (List<Appointment>)await Task.FromResult(FindEvent) : null;
        // }
        public async Task<List<Appointment>> GetAppointmentByEventDate(DateTime getEvent)
        {
            // var FindEvent = TodayAppointment.Where(events => events.EventDate.ToShortDateString().Contains(getEvent.ToShortDateString())).ToList();
            var FindEvent = TodayAppointment.Where(events => events.EventDate.Date == getEvent.Date).ToList();
            // return (FindEvent.Count() == 0) ? null : (List<Appointment>)await Task.FromResult(FindEvent);
            // return FindEvent.Any() ? (List<Appointment>)await Task.FromResult(FindEvent) : null;
            return await Task.FromResult(FindEvent);
        }
        public async Task<bool> CheckAlreadyExistingEvent(Appointment request)
        {
            var CompareEvent =TodayAppointment.Where(events=>
                            (request.StartTimeHrMin>=events.StartTimeHrMin&&request.StartTimeHrMin<events.EndTimeHrMin) || 
                            (request.EndTimeHrMin>events.StartTimeHrMin&&request.EndTimeHrMin<=events.EndTimeHrMin) ||
                            (request.StartTimeHrMin<=events.StartTimeHrMin&&request.EndTimeHrMin>=events.EndTimeHrMin));
            // return ((CompareEvent==null)||(CompareEvent.Id==request.Id)) ? await Task.FromResult(false) : await Task.FromResult(true);
            // return CompareEvent.Any() ? await Task.FromResult(true): await Task.FromResult(false);
            if(CompareEvent.Any()){
                var CompareId = CompareEvent.Where(item=>item.Id!=request.Id);
                return CompareId.Any() && await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
            // if(CompareEvent.Any()){
            //     foreach (var item in CompareEvent)
            //     {
            //         return item.Id != request.Id && await Task.FromResult(true); 
            //     }
            // }
        public async Task<Appointment> CreateAppointment(Appointment request)
        {
                TodayAppointment.Add(request);
                return await Task.FromResult(request);
        }
        public async Task<bool> DeleteAppointmentById(Guid id)
        {
            var DeleteEvent = TodayAppointment.Find(events=> events.Id.Equals(id));
            if(DeleteEvent==null)
            {
                return false;
            }
            TodayAppointment.Remove(DeleteEvent);
            return await Task.FromResult(true);
        }
        public Task UpdateAnAppointment(Appointment request)
        {
            var UpdateEvent = TodayAppointment.Find(events=>events.Id==request.Id);
            if(UpdateEvent != null)
            {
                UpdateEvent.EventName=request.EventName;
                UpdateEvent.EventDate=request.EventDate;
                UpdateEvent.StartTimeHrMin=request.StartTimeHrMin;
                UpdateEvent.EndTimeHrMin=request.EndTimeHrMin;
                UpdateEvent.DescriptionOfEvent=request.DescriptionOfEvent;
                // return await Task.FromResult(UpdateEvent);
                return Task.CompletedTask;
            }
            // else{
            //     return Task.FromResult(false);
            // }
            // return Task.FromException(new Exception("Id Not Found"));
            return null;
        }
    }
}