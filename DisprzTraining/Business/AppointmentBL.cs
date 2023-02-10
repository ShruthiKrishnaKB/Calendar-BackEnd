using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.DataAccess;
using DisprzTraining.Models;

namespace DisprzTraining.Business
{
    public class AppointmentBL:IAppointmentBL
    {
        // dependency injection
        private readonly IAppointmentDAL _appointmentDAL;
        // Need to add scope in the program.cs
        // constructor
        public AppointmentBL(IAppointmentDAL appointmentDAL)
        {
            _appointmentDAL=appointmentDAL;
        }
        public async Task<List<Appointment>> GettingAllAppointments()
        {
            return await _appointmentDAL.GetAllAppointments();
        }
        public async Task<List<Appointment>> GettingAppointmentsByEventDate(DateTime getEvent)
        {
            var GetResult = await _appointmentDAL.GetAllAppointments();
            var FindEvent = GetResult.Where(events => events.EventDate.Date == getEvent.Date).OrderBy(events=>events.StartTimeHrMin).ToList();
            return await Task.FromResult(FindEvent);
        }
        public async Task<bool> CheckAlreadyExistingEvent(Appointment request)
        {
            var GetResult = await _appointmentDAL.GetAllAppointments();
            var CompareEvent =GetResult.Where(events=>
                            (request.StartTimeHrMin>=events.StartTimeHrMin&&request.StartTimeHrMin<events.EndTimeHrMin) || 
                            (request.EndTimeHrMin>events.StartTimeHrMin&&request.EndTimeHrMin<=events.EndTimeHrMin) ||
                            (request.StartTimeHrMin<=events.StartTimeHrMin&&request.EndTimeHrMin>=events.EndTimeHrMin));
            if(CompareEvent.Any()){
                var CompareId = CompareEvent.Where(item=>item.Id!=request.Id);
                return CompareId.Any() && await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
        public async Task<Appointment> CreatingAppointments(Appointment request)
        {
            var check = await CheckAlreadyExistingEvent(request);
            return (!check) ? await _appointmentDAL.CreateAppointment(request):null;
        }
        public Task<bool> DeletingAppointmentById(Guid id)
        {
            return  _appointmentDAL.DeleteAppointmentById(id);
        }
        public async Task<Task> UpdatingAnAppointment(Appointment request)
        {
            var Check = await CheckAlreadyExistingEvent(request);
            return (!Check) ?  _appointmentDAL.UpdateAnAppointment(request): null;
        }
        // public async Task<List<Appointment>> GettingAppointmentsByEventName(string getEvent)
        // {
        //     var GetResult = await _appointmentDAL.GetAllAppointments();
        //     var FindEvent = GetResult.Where(events => events.EventName.ToLower().Contains(getEvent.ToLower())).ToList();
        //     return FindEvent.Any() ?(List<Appointment>)await Task.FromResult(FindEvent) : null;
        // }
    }
}