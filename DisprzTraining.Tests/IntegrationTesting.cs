using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DisprzTraining.Business;
using DisprzTraining.Controllers;
using DisprzTraining.DataAccess;
using DisprzTraining.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DisprzTraining.Tests
{
    public class IntegrationTesting
    {
        private readonly IAppointmentDAL _appointmentsDAL;
        private readonly IAppointmentBL _appointmentsBL;
        private readonly appointmentsController _appointmentController;
        private HttpClient _httpClient;
        Appointment RequestAppointment = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),EventName="Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,22,15,0),EndTimeHrMin=new DateTime(2022,12,15,23,30,0),DescriptionOfEvent="meet with colleagues"
            };
        Appointment ConflictUpdateAppointment = new Appointment(){
            Id=new Guid("9245fe4a-d402-48ac-b1ed-9c1a04247482"),EventName="Meetup",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,15,15,0),EndTimeHrMin=new DateTime(2022,12,15,16,30,0),DescriptionOfEvent="meet with colleagues"
        };
        Appointment ConflictAppointment=new Appointment(){
            Id=new Guid("9845fa4a-d111-451c-b9ed-9c1a04247482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,22,00,0),EndTimeHrMin=new DateTime(2022,12,15,23,00,0),DescriptionOfEvent="meet with colleagues"
        };
        public IntegrationTesting(){
            _appointmentsDAL=new AppointmentDAL();
            _appointmentsBL=new AppointmentBL(_appointmentsDAL);
            _appointmentController = new appointmentsController(_appointmentsBL);

            var IntegrationTesting = new WebApplicationFactory<appointmentsController>();
            _httpClient=IntegrationTesting.CreateDefaultClient();
        }
        [Fact]
        public async Task IntegrationTesting_GetAll_SuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("http://localhost:5169/api/appointments");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task IntegrationTesting_GetByEventDate_SuccessStatusCode()
        {
            var response = await _httpClient.GetAsync("http://localhost:5169/api/appointments/date?date=2022%2012%2014");
            Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        }
        [Fact]
        public async Task IntegrationTesting_Create_CreatedStatusCode_ResponseMessage(){
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",RequestAppointment);
            Assert.Equal(HttpStatusCode.Created,response.StatusCode);
            var result= await response.Content.ReadAsStringAsync();
            Assert.Equal("true",result);
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-451c-b9ed-9c1a04247482");
        }
        [Fact]
        public async Task IntegrationTesting_Create_Conflict_ResponseMessage(){
            await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",RequestAppointment);
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",ConflictAppointment);
            Assert.Equal(HttpStatusCode.Conflict,response.StatusCode);
            var result= await response.Content.ReadAsStringAsync();
            Assert.Equal("Meeting already exists in the given time",result);
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9845fa4a-d111-451c-b9ed-9c1a04247482");
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-451c-b9ed-9c1a04247482");
        }
        [Fact]
        public async Task IntegrationTesting_DeleteById_NoContentStatusCode(){
            await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",RequestAppointment);
            var response = await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-451c-b9ed-9c1a04247482");
            Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        }
        [Fact]
        public async Task IntegrationTesting_DeleteById_NotFound_ResponseMessage(){
            var response = await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/934ae34a-d402-451c-b9ed-9c1a04247482");
            Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
        }
        [Fact]
        public async Task IntegrationTesting_Update_NoContentStatusCode(){
            await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",RequestAppointment);
            var UpdateAppointment = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),EventName="Meeting",EventDate=new DateTime(2022,12,16),StartTimeHrMin=new DateTime(2022,12,16,22,00,0),EndTimeHrMin=new DateTime(2022,12,16,23,00,0),DescriptionOfEvent="meet with colleagues"
            };
            var response = await _httpClient.PutAsJsonAsync("http://localhost:5169/api/appointments",UpdateAppointment);
            Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-451c-b9ed-9c1a04247482");
        }
        [Fact]
        public async Task IntegrationTesting_Update_ConflictStatusCode(){
            await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",RequestAppointment);
            await _httpClient.PostAsJsonAsync("http://localhost:5169/api/appointments",ConflictUpdateAppointment);
            var UpdateAppointment = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),EventName="Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,15,00,0),EndTimeHrMin=new DateTime(2022,12,15,16,00,0),DescriptionOfEvent="meet with colleagues"
            };
            var response = await _httpClient.PutAsJsonAsync("http://localhost:5169/api/appointments",UpdateAppointment);
            Assert.Equal(HttpStatusCode.Conflict,response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("Meeting already exists in the given time",result);
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-451c-b9ed-9c1a04247482");
            await _httpClient.DeleteAsync("http://localhost:5169/api/appointments/9245fe4a-d402-48ac-b1ed-9c1a04247482");
        }
        // [Fact]
        // public async Task IntegrationTesting_GetByEventName_SuccessStatusCode(){
        //     var response = await _httpClient.GetAsync("http://localhost:5169/api/appointments/event?events=standup");
        //     Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        // }
        // [Fact]
        // public async Task IntegrationTesting_GetByEventName_NotFoundStatus_ResponseMessage(){
        //     var response = await _httpClient.GetAsync("http://localhost:5169/api/appointments/event?events=meetup");
        //     Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
        //     var result = await response.Content.ReadAsStringAsync();
        //     Assert.Equal("Event not found", result);
        // }
    }
}