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
        [HttpGet("all-appointments")]
        public async Task<IActionResult> GetAll(){
        // should use async for Task
            return Ok(await _appointmentBL.GettingAllAppointments());
            // return null;
            // problem faced
        }
        [HttpGet("event")]
        public async Task<IActionResult> GetByEventName(string events){
            var response = await _appointmentBL.GettingAppointmentsByEventName(events);
            if(response==null){
                return NotFound("Event not found");
            }
            return Ok(response);
            

        }
        [HttpGet("date")]
        public async Task<IActionResult> GetByEventDate(DateTime events){
            var response = await _appointmentBL.GettingAppointmentsByEventDate(events);
            if(response==null){
                return NotFound("Event not found");
            }
            return Ok(response); 

        }
        [HttpPost("event")]
        public async Task<IActionResult> Create(Appointment request){
            var response = await _appointmentBL.CreatingAppointments(request);
            if(response==null){
                return Conflict("Meeting already exists in the given time");
            }
            return Created("Created",request);

        }
        [HttpDelete("event/{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            var response = await _appointmentBL.DeletingAppointmentById(id);
            if(response==false){
                return NotFound("Id not found");
            }
            return NoContent();

        }
        [HttpPut("event")]
        public async Task<IActionResult> Update(Appointment request)
        {
            var response = await _appointmentBL.UpdatingAnAppointment(request);
            if(response==null){
                return Conflict("Id not found / conflict occured");
            }
            return NoContent();

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
