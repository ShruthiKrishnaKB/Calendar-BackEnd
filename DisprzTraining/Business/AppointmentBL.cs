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
        public async Task<List<Appointment>> GettingAppointmentsByEventName(string events)
        {
            return await _appointmentDAL.GetAppointmentByEventName(events);
        }
        public async Task<List<Appointment>> GettingAppointmentsByEventDate(DateTime events)
        {
            return await _appointmentDAL.GetAppointmentByEventDate(events);
        }
        public async Task<Appointment> CreatingAppointments(Appointment request)
        {
            var CreateEvent = await _appointmentDAL.CheckAlreadyExistingEvent(request);
            if(!CreateEvent){
                return await _appointmentDAL.CreateAppointment(request);
            }
            else{
                return null;
            }
        }
        public Task DeletingAppointmentById(Guid id)
        {
            return  _appointmentDAL.DeleteAppointmentById(id);
        }

        public async Task<Task> UpdatingAnAppointment(Appointment request)
        {
            var check = await _appointmentDAL.CheckAlreadyExistingEvent(request);
            if(!check){
                return _appointmentDAL.UpdateAnAppointment(request);
            }
            else{
                return null;
            }
        }
    }
}