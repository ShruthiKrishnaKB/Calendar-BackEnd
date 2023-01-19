using DisprzTraining.Business;
using DisprzTraining.Models;
using Microsoft.AspNetCore.Mvc;

namespace DisprzTraining.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class appointmentsController : ControllerBase
    {
        private readonly IAppointmentBL _appointmentBL;
        public appointmentsController(IAppointmentBL appointmentBL)
        {
            _appointmentBL=appointmentBL;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(){
        // should use async for Task
            return Ok(await _appointmentBL.GettingAllAppointments());
            // return null;
            // problem faced
        }
        [HttpGet("event")]
        public async Task<IActionResult> GetByEventName(string events){
            var response = await _appointmentBL.GettingAppointmentsByEventName(events);
            return (response==null)? NotFound("Event not found"): Ok(response);
        }
        // [HttpGet("date")]
        // public async Task<IActionResult> GetByEventDate(DateTime date){
        //     var response = await _appointmentBL.GettingAppointmentsByEventDate(date);
        //     return (response==null) ? Ok() : Ok(response); 
        // }
        [HttpGet("date")]
        public async Task<IActionResult> GetByEventDate(DateTime date){
            // var response = await _appointmentBL.GettingAppointmentsByEventDate(date);
            // return Ok(response); 
            return Ok(await _appointmentBL.GettingAppointmentsByEventDate(date));
        }
        [HttpPost]
        public async Task<IActionResult> Create(Appointment request){
            if(request.StartTimeHrMin<request.EndTimeHrMin){
                var response = await _appointmentBL.CreatingAppointments(request);
                return response==null ? Conflict("Meeting already exists in the given time") : Created("Created",request);
            }
            return BadRequest("Invalid Time Interval");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            var response = await _appointmentBL.DeletingAppointmentById(id);
            return (response==false) ? NotFound("Id not found") : NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> Update(Appointment request)
        {
            var response = await _appointmentBL.UpdatingAnAppointment(request);
            // return ((response==null) ? Conflict("Meeting already exists in the given time") : ((response==false)  ? NotFound("Id not found") : NoContent()));             
            return (response==null) ? Conflict("Meeting already exists in the given time") : NoContent();             
        }
        //design - GET /api/appointments
        //- POST /api/appointments
        //- DELETE /api/appointments

        //refer hello world controller for BL & DAL logic 

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Appointment))]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GerAppointments()
        //{
        //    return Ok();
        //}

    }
}
