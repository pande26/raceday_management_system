using Microsoft.AspNetCore.Mvc;
using Moq;
using raceday_management_system.Controllers;
using raceday_management_system.Database.Interfaces;
using Xunit;

namespace raceday_management_system.Tests
{//start of namespace
    public class EventsControllerTests
    {//start of class

        //Participant cannot create event (403)
        [Fact]
        public void participant_cannot_create_event()
        {//start of test

            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = new events(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.create_event(
                "Test", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Run");

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);

        }//end of test

        //Organiser can create event (201)
        [Fact]
        public void organiser_can_create_event()
        {//start of test

            //arrange
            var mock_db = new Mock<Ievents_connection>();
            mock_db.Setup(m => m.create_event(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<decimal>(),
                It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<decimal?>()))
                .Returns("Event created successfully.");

            var controller = new events(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act
            var result = controller.create_event(
                "Test Event", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Run");

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, status_result.StatusCode);

        }//end of test

        //Unauthenticated user cannot view events (401)
        [Fact]
        public void unauthenticated_cannot_view_events()
        {//start of test

            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = new events(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.get_all_events();

            //assert
            Assert.IsType<UnauthorizedObjectResult>(result);

        }//end of test

        //Invalid event type returns 400 Bad Request
        [Fact]
        public void invalid_event_type_returns_400()
        {//start of test

            //arrange
            var mock_db = new Mock<Ievents_connection>();
            var controller = new events(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act - invalid type "Swim"
            var result = controller.create_event(
                "Test Event", "Desc", DateTime.UtcNow.AddDays(30),
                "Cape Town", 42.2m, "Swim");

            //assert
            Assert.IsType<BadRequestObjectResult>(result);

        }//end of test

    }//end of class

}//end of namespace