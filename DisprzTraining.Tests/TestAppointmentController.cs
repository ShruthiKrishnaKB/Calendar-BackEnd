using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.Business;
using DisprzTraining.Controllers;
using DisprzTraining.DataAccess;
using DisprzTraining.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DisprzTraining.Tests
{
    public class TestAppointmentController
    {
        private readonly IAppointmentDAL _appointmentsDAL;
        // for DAL
        private readonly IAppointmentBL _appointmentsBL;
        // for BL
        private readonly appointmentsController _appointmentController;
        Appointment expectedItem1 = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),EventName="Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,22,15,0),EndTimeHrMin=new DateTime(2022,12,15,23,30,0),DescriptionOfEvent="meet with colleagues"
        };
        Appointment expectedItem2 = new Appointment(){
            Id=new Guid("9845fa4a-d402-451c-b9ed-9c1a04247482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,17),StartTimeHrMin=new DateTime(2022,12,17,22,00,0),EndTimeHrMin=new DateTime(2022,12,17,23,00,0),DescriptionOfEvent="meet with colleagues"
        };
        Appointment expectedItem3 = new Appointment(){
            Id=new Guid("9845fa4a-d102-451c-b9ed-9c1a04247482"),EventName="Daily Meet",EventDate=new DateTime(2022,12,16),StartTimeHrMin=new DateTime(2022,12,16,22,00,0),EndTimeHrMin=new DateTime(2022,12,16,23,00,0),DescriptionOfEvent="meet with colleagues"
        };
        public TestAppointmentController(){
            _appointmentsDAL=new AppointmentDAL();
            _appointmentsBL=new AppointmentBL(_appointmentsDAL);
            _appointmentController = new appointmentsController(_appointmentsBL);
        }
        [Fact]
        public async Task GetAll_GettingAppointments_OkObjectResult()
        {
            // Act 
            var getresult = await _appointmentController.GetAll();
            // Assert
            Assert.IsType<OkObjectResult>(getresult);
            // Since i'm getting from controller the return type will be OKObjectResult;
        }
        [Fact]
        public async Task GetAll_GettingAppointments_ValueReturnType(){
            var getresult= (OkObjectResult) await _appointmentController.GetAll();
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        [Fact]
        public async Task GetByEventName_WithUnexistingItems_NotFound(){
            var getresult = await _appointmentController.GetByEventName("scrum");
            Assert.IsType<NotFoundObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventName_WithExistingItems_OkObjectResult(){
            var getresult = await _appointmentController.GetByEventName("");
            Assert.IsType<OkObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventName_GettingAppointment_ValueReturnType(){
            var getresult = (OkObjectResult)await _appointmentController.GetByEventName("daily");
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        
        [Fact]
        public async Task GetByEventDate_WithUnexistingItem_NotFound(){
            var getresult = await _appointmentController.GetByEventDate(new DateTime(2022,12,12));
            Assert.IsType<NotFoundObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventDate_WithExistingItem_OkObjectResult(){
            var getresult= await _appointmentController.GetByEventDate(new DateTime(2022,12,14));
            Assert.IsType<OkObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventDate_GettingAppointment_ValueReturnType(){
            var getresult=(OkObjectResult) await _appointmentController.GetByEventDate(new DateTime(2022,12,14));
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        [Fact]
        public async Task Create_CreatingAppointment_CreatedResult(){
            var getresult =  await _appointmentController.Create(expectedItem1);
            Assert.IsType<CreatedResult>(getresult);
        }
        [Fact]
        public async Task Create_CreatingAppointment_ValueReturnType(){
            var getresult =  (CreatedResult)await _appointmentController.Create(expectedItem1);
            Assert.IsType<Appointment>(getresult.Value);
        }
        [Fact]
        public async Task Create_SameEventAlreadyExists_ConflictObjectResult(){
            var item1 =  await _appointmentController.Create(expectedItem2);
            var expectedItem=new Appointment(){
            Id=new Guid("9845fa4a-d111-451c-b9ed-9c1a04247482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,17),StartTimeHrMin=new DateTime(2022,12,17,22,00,0),EndTimeHrMin=new DateTime(2022,12,17,23,00,0),DescriptionOfEvent="meet with colleagues"
        };
            var item2 =  await _appointmentController.Create(expectedItem);
            Assert.IsType<ConflictObjectResult>(item2);
        }
        [Fact]
        public async Task DeleteById_DeletingAppointemnt_NoContentResult(){
            var result =  await _appointmentController.Create(expectedItem1);
            var getresult = await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            Assert.IsType<NoContentResult>(getresult);
        }
        [Fact]
        public async Task DeleteById_WithUnexistingItem_NotFoundObjectResult(){
            var getresult = await _appointmentController.DeleteById(new Guid("9845f94a-d402-451c-b9ed-9c1a04247482"));
            Assert.IsType<NotFoundObjectResult>(getresult);
        }
        [Fact]
        public async Task Update_UpdatingEvent_NoContentResult(){
            await _appointmentController.Create(expectedItem3);
            Appointment UpdatedEvent = new Appointment(){
            Id=new Guid("9845fa4a-d102-451c-b9ed-9c1a04247482"),
            EventName="Daily Meeting",
            EventDate=new DateTime(2022,12,14),
            StartTimeHrMin=new DateTime(2022,12,14,22,00,0),
            EndTimeHrMin=new DateTime(2022,12,14,23,00,0),
            DescriptionOfEvent="meet with colleagues"
        };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
        }
        [Fact]
        public async Task Update_UpdatedTimeAlreadyExisting_ConflictObjectResult(){
            Appointment UpdatedEvent = new Appointment(){
            Id=new Guid("9845fa4a-d102-451c-b9ed-9c1a04247482"),
            EventName="Daily Meet",
            EventDate=new DateTime(2022,12,15),
            StartTimeHrMin=new DateTime(2022,12,15,22,15,0),
            EndTimeHrMin=new DateTime(2022,12,15,23,30,0),
            DescriptionOfEvent="meet with colleagues"
            };
            var getresult=await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<ConflictObjectResult>(getresult);
        }
    }
}