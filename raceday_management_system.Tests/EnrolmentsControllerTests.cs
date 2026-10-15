using Microsoft.AspNetCore.Mvc;
using Moq;
using raceday_management_system.Controllers;
using raceday_management_system.Database.Interfaces;
using raceday_management_system.Models;
using Xunit;

namespace raceday_management_system.Tests
{//start of namespace
    public class EnrolmentsControllerTests
    {//start of class

        //Participant can enrol in event (201)
        [Fact]
        public void participant_can_enrol()
        {//start of test

            //arrange
            var mock_db = new Mock<Ienrolments_connection>();
            mock_db.Setup(m => m.category_belongs_to_event(1, 1)).Returns(true);
            mock_db.Setup(m => m.get_my_enrolment_for_event(5, 1)).Returns((Enrolments?)null);
            mock_db.Setup(m => m.create_enrolment(5, 1, 1, null)).Returns("Enrolment successful.");

            var controller = new enrolments(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.enrol_in_event(1, 1, null);

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, status_result.StatusCode);

        }//end of test

        //Organiser cannot enrol (403)
        [Fact]
        public void organiser_cannot_enrol()
        {//start of test

            //arrange
            var mock_db = new Mock<Ienrolments_connection>();
            var controller = new enrolments(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Organiser", 1)
            };

            //act
            var result = controller.enrol_in_event(1, 1, null);

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, status_result.StatusCode);

        }//end of test

        //Duplicate enrolment is rejected (400)
        [Fact]
        public void duplicate_enrolment_rejected()
        {//start of test

            //arrange
            var mock_db = new Mock<Ienrolments_connection>();
            mock_db.Setup(m => m.category_belongs_to_event(1, 1)).Returns(true);
            mock_db.Setup(m => m.get_my_enrolment_for_event(5, 1))
                .Returns(new Enrolments
                {
                    Enrolment_id = 99,
                    Participant_id = 5,
                    Event_id = 1,
                    Category_id = 1,
                    Enr_status = "Confirmed"
                });

            var controller = new enrolments(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.enrol_in_event(1, 1, null);

            //assert
            Assert.IsType<BadRequestObjectResult>(result);

        }//end of test

        //Invalid category for event returns 400
        [Fact]
        public void invalid_category_returns_400()
        {//start of test

            //arrange
            var mock_db = new Mock<Ienrolments_connection>();
            mock_db.Setup(m => m.category_belongs_to_event(99, 1)).Returns(false);

            var controller = new enrolments(mock_db.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context("Participant", 5)
            };

            //act
            var result = controller.enrol_in_event(1, 99, null);

            //assert
            Assert.IsType<BadRequestObjectResult>(result);

        }//end of test

    }//end of class

}//end of namespace