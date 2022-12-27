using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.Models;

namespace DisprzTraining.Business
{
    public interface IAppointmentBL
    {
        Task<List<Appointment>> GettingAllAppointments();
        Task<List<Appointment>> GettingAppointmentsByEventName(string events);
        Task<List<Appointment>> GettingAppointmentsByEventDate(DateTime events);
        Task<Appointment> CreatingAppointments(Appointment request);
        Task<bool> DeletingAppointmentById(Guid id);
        Task<Task> UpdatingAnAppointment(Appointment request);
    }
}