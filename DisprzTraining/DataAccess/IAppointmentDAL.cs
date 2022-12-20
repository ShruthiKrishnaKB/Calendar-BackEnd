using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.Models;

namespace DisprzTraining.DataAccess
{
    public interface IAppointmentDAL
    {
        Task<List<Appointment>> GetAllAppointments();
        Task<List<Appointment>> GetAppointmentByEventName(string getEvent);
        Task<List<Appointment>> GetAppointmentByEventDate(DateTime getEvent);
        Task<bool> CheckAlreadyExistingEvent(Appointment request);
        Task<Appointment> CreateAppointment(Appointment request);
        Task DeleteAppointmentById(Guid id);
        Task UpdateAnAppointment(Appointment request);
    }
}