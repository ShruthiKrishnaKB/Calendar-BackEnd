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
        public static List<Appointment> TodayAppointment = new List<Appointment>(){
            new Appointment(){Id=Guid.NewGuid(),EventName="Daily Standup",EventDate=new DateTime(2022,12,14),StartTimeHrMin=new DateTime(2022,12,14,12,10,0),EndTimeHrMin=new DateTime(2022,12,14,12,45,0),DescriptionOfEvent="a meet with EM"},
            new Appointment(){Id=Guid.NewGuid(),EventName="Daily Catchup",EventDate=new DateTime(2022,12,14),StartTimeHrMin=new DateTime(2022,12,14,04,00,0),EndTimeHrMin=new DateTime(2022,12,14,04,30,00),DescriptionOfEvent="a meet with Mentor"}
        };
        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await Task.FromResult(TodayAppointment);
        }
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
                return Task.CompletedTask;
            }
            return null;
        }
    }
}