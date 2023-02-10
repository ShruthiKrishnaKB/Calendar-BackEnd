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
        Task<Appointment> CreateAppointment(Appointment request);
        Task<bool> DeleteAppointmentById(Guid id);
        Task UpdateAnAppointment(Appointment request);
    }
}