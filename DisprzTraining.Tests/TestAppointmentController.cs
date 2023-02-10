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
        Appointment expectedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),EventName="Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,22,15,0),EndTimeHrMin=new DateTime(2022,12,15,23,30,0),DescriptionOfEvent="meet with colleagues"
        };
        Appointment exampleEvent = new Appointment(){
            Id=new Guid("9845fa4a-d402-451c-b9ed-9c1a04247482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,20,00,0),EndTimeHrMin=new DateTime(2022,12,15,21,00,0),DescriptionOfEvent="meet with colleagues"
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
            // Since getting from controller the return type will be OKObjectResult;
        }
        [Fact]
        public async Task GetAll_GettingAppointments_ValueReturnType(){
            var getresult= (OkObjectResult) await _appointmentController.GetAll();
            Assert.Equal(200 , getresult.StatusCode);
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        [Fact]
        public async Task GetByEventDate_WithNonExistingItem_OkObjectResult(){
            var getresult = await _appointmentController.GetByEventDate(new DateTime(2022,12,12));
            Assert.IsType<OkObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventDate_WithNonExistingItem_ValueReturnType(){
            var getresult =(OkObjectResult) await _appointmentController.GetByEventDate(new DateTime(2022,12,12));
            Assert.Equal(200,getresult.StatusCode);
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        [Fact]
        public async Task GetByEventDate_WithExistingItem_OkObjectResult(){
            var getresult= await _appointmentController.GetByEventDate(new DateTime(2022,12,14));
            Assert.IsType<OkObjectResult>(getresult);
        }
        [Fact]
        public async Task GetByEventDate_GettingAppointment_ValueReturnType(){
            var getresult=(OkObjectResult) await _appointmentController.GetByEventDate(new DateTime(2022,12,14));
            Assert.Equal(200,getresult.StatusCode);
            Assert.IsType<List<Appointment>>(getresult.Value);
        }
        [Fact]
        public async Task Create_CreatingAppointment_CreatedResult(){
            var getresult =  await _appointmentController.Create(expectedEvent);
            Assert.IsType<CreatedResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Create_CreatingAppointment_ValueReturnType(){
            var getresult =  (CreatedResult)await _appointmentController.Create(expectedEvent);
            Assert.Equal(201,getresult.StatusCode);
            Assert.IsType<bool>(getresult.Value);
            Assert.Equal(getresult.Value,true);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Create_SameEventAlreadyExists_ConflictObjectResult(){
            var item1 =  await _appointmentController.Create(expectedEvent);
            var expectedItem=new Appointment(){
            Id=new Guid("9845fa4a-d111-451c-b9ed-9c1a04247482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,15),StartTimeHrMin=new DateTime(2022,12,15,22,00,0),EndTimeHrMin=new DateTime(2022,12,15,23,00,0),DescriptionOfEvent="meet with colleagues"
        };
            var item2 =  await _appointmentController.Create(expectedItem);
            Assert.IsType<ConflictObjectResult>(item2);
            var item3 = (ConflictObjectResult)await _appointmentController.Create(expectedItem);
            Assert.Equal(409,item3.StatusCode);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Create_InvalidTimeInterval_BadRequestObjectResult(){
            var expectedItem = new Appointment(){
                Id=new Guid("9845fa4a-d111-451c-b9ed-9c1a0425a482"),EventName="Daily Meeting",EventDate=new DateTime(2022,12,17),StartTimeHrMin=new DateTime(2022,12,17,22,00,0),EndTimeHrMin=new DateTime(2022,12,17,21,00,0),DescriptionOfEvent="meet with colleagues"
            };
            var item = await _appointmentController.Create(expectedItem);
            Assert.IsType<BadRequestObjectResult>(item);
            await _appointmentController.DeleteById(new Guid("9845fa4a-d111-451c-b9ed-9c1a0425a482"));
        }
        [Fact]
        public async Task DeleteById_DeletingAppointemnt_NoContentResult(){
            var item1 =  await _appointmentController.Create(expectedEvent);
            var getresult = await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            Assert.IsType<NoContentResult>(getresult);
        }
        [Fact]
        public async Task DeleteById_WithNonExistingItem_NotFoundObjectResult(){
            var getresult = await _appointmentController.DeleteById(new Guid("9845f94a-d402-451c-b9ed-9c1a04247482"));
            Assert.IsType<NotFoundObjectResult>(getresult);
        }
        [Fact]
        public async Task Update_UpdatingEvent_ByChangingEventName_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
            Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
            EventName="Meet",
            EventDate=new DateTime(2022,12,15),
            StartTimeHrMin=new DateTime(2022,12,15,22,15,0),
            EndTimeHrMin=new DateTime(2022,12,15,23,30,0),
            DescriptionOfEvent="meet with colleagues"
        };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdatingEvent_ByChangingEventDate_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                EventName="Meeting",
                EventDate=new DateTime(2022,12,16),
                StartTimeHrMin=new DateTime(2022,12,16,22,15,0),
                EndTimeHrMin=new DateTime(2022,12,16,23,30,0),
                DescriptionOfEvent="meet with colleagues"
            };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdatingEvent_ByChangingEventTime_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                EventName="Meeting",
                EventDate=new DateTime(2022,12,15),
                StartTimeHrMin=new DateTime(2022,12,15,12,15,0),
                EndTimeHrMin=new DateTime(2022,12,15,13,30,0),
                DescriptionOfEvent="meet with colleagues"
            };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdatingEvent_ByChangingDescription_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                EventName="Meeting",
                EventDate=new DateTime(2022,12,15),
                StartTimeHrMin=new DateTime(2022,12,15,22,15,0),
                EndTimeHrMin=new DateTime(2022,12,15,23,30,0),
                DescriptionOfEvent="meeting with colleagues"
            };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdatingEvent_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                EventName="Meet",
                EventDate=new DateTime(2022,12,16),
                StartTimeHrMin=new DateTime(2022,12,16,12,15,0),
                EndTimeHrMin=new DateTime(2022,12,16,13,30,0),
                DescriptionOfEvent="meet with colleagues"
            };
            var getresult = await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdatingEvent_TimeSimilarToEvent_NoContentResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
            Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
            EventName="Daily Meet",
            EventDate=new DateTime(2022,12,15),
            StartTimeHrMin=new DateTime(2022,12,15,22,00,0),
            EndTimeHrMin=new DateTime(2022,12,15,23,00,0),
            DescriptionOfEvent="meet with colleagues"
            };
            var getresult=await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<NoContentResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_AlreadyExistingEvent_ConflictObjectResult(){
            await _appointmentController.Create(expectedEvent);
            await _appointmentController.Create(exampleEvent);
            Appointment UpdatedEvent = new Appointment(){
            Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
            EventName="Daily Meet",
            EventDate=new DateTime(2022,12,15),
            StartTimeHrMin=new DateTime(2022,12,15,20,00,0),
            EndTimeHrMin=new DateTime(2022,12,15,21,00,0),
            DescriptionOfEvent="meet with colleagues"
            };
            var getresult=await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<ConflictObjectResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            await _appointmentController.DeleteById(new Guid("9845fa4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_UpdateTimeOverlapping_WithMoreThanOneEvent_ConflictObjectResult(){
            await _appointmentController.Create(expectedEvent);
            await _appointmentController.Create(exampleEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"),
                EventName="Daily Meet",
                EventDate=new DateTime(2022,12,15),
                StartTimeHrMin=new DateTime(2022,12,15,19,00,0),
                EndTimeHrMin=new DateTime(2022,12,15,23,00,0),
                DescriptionOfEvent="meet with colleagues"
            };
            var getresult=await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<ConflictObjectResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
            await _appointmentController.DeleteById(new Guid("9845fa4a-d402-451c-b9ed-9c1a04247482"));
        }
        [Fact]
        public async Task Update_InvalidId_ConflictObjectResult(){
            await _appointmentController.Create(expectedEvent);
            Appointment UpdatedEvent = new Appointment(){
                Id=Guid.NewGuid(),
                EventName="Daily Meet",
                EventDate=new DateTime(2022,12,17),
                StartTimeHrMin=new DateTime(2022,12,17,19,00,0),
                EndTimeHrMin=new DateTime(2022,12,17,23,00,0),
                DescriptionOfEvent="meet with colleagues"
            };
            var getresult=await _appointmentController.Update(UpdatedEvent);
            Assert.IsType<ConflictObjectResult>(getresult);
            await _appointmentController.DeleteById(new Guid("9245fe4a-d402-451c-b9ed-9c1a04247482"));
        }
        // [Fact]
        // public async Task GetByEventName_WithNonExistingItem_NotFound(){
        //     var getresult = await _appointmentController.GetByEventName("scrum");
        //     Assert.IsType<NotFoundObjectResult>(getresult);
        // }
        // [Fact]
        // public async Task GetByEventName_WithExistingItems_OkObjectResult(){
        //     var getresult = await _appointmentController.GetByEventName("daily");
        //     Assert.IsType<OkObjectResult>(getresult);
        // }
        // [Fact]
        // public async Task GetByEventName_GettingAppointment_ValueReturnType(){
        //     var getresult = (OkObjectResult)await _appointmentController.GetByEventName("daily");
        //     Assert.Equal(200,getresult.StatusCode);
        //     Assert.IsType<List<Appointment>>(getresult.Value);
        // }
    }
}