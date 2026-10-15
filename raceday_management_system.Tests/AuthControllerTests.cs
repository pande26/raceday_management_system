using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using raceday_management_system.Controllers;
using raceday_management_system.Database.Interfaces;
using raceday_management_system.Models;
using raceday_management_system.Services;
using Xunit;

namespace raceday_management_system.Tests
{//start of namespace
    public class AuthControllerTests
    {//start of class

        //Register with valid data returns 201 Created
        [Fact]
        public void register_with_valid_data_returns_201()
        {//start of test

            //arrange
            var mock_db = new Mock<Iauth_connection>();
            mock_db.Setup(m => m.email_exists(It.IsAny<string>())).Returns(false);
            mock_db.Setup(m => m.store_user_info(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly?>(),
                It.IsAny<string>())).Returns("User registered successfully.");

            var hasher = new password_hasher();
            var controller = new authenticator(mock_db.Object, hasher);

            //act
            var result = controller.register_user(
                "test@raceday.co.za", "Pass123!", "Test", "User",
                authenticator.roles.Participant);

            //assert
            var status_result = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, status_result.StatusCode);

        }//end of test

        //Register with duplicate email returns 409 Conflict
        [Fact]
        public void register_with_duplicate_email_returns_409()
        {//start of test

            //arrange
            var mock_db = new Mock<Iauth_connection>();
            mock_db.Setup(m => m.email_exists(It.IsAny<string>())).Returns(true);

            var hasher = new password_hasher();
            var controller = new authenticator(mock_db.Object, hasher);

            //act
            var result = controller.register_user(
                "existing@raceday.co.za", "Pass123!", "Test", "User",
                authenticator.roles.Participant);

            //assert
            Assert.IsType<ConflictObjectResult>(result);

        }//end of test

        //login with non-existent user returns 401 Unauthorized
        [Fact]
        public void login_with_invalid_user_returns_401()
        {//start of test

            //arrange
            var mock_db = new Mock<Iauth_connection>();
            mock_db.Setup(m => m.get_user_by_email(It.IsAny<string>()))
                .Returns((Users?)null);

            var hasher = new password_hasher();
            var controller = new authenticator(mock_db.Object, hasher);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.login_user("wrong@raceday.co.za", "WrongPass");

            //assert
            Assert.IsType<UnauthorizedObjectResult>(result);

        }//end of test

        //Login with wrong password returns 401 Unauthorized
        [Fact]
        public void login_with_wrong_password_returns_401()
        {//start of test

            //arrange
            var hasher = new password_hasher();
            string correct_hash = hasher.hash_password("CorrectPass123");

            var mock_db = new Mock<Iauth_connection>();
            mock_db.Setup(m => m.get_user_by_email(It.IsAny<string>()))
                .Returns(new Users
                {
                    User_id = 1,
                    Email = "user@raceday.co.za",
                    Password_hash = correct_hash,
                    Firstname = "Test",
                    Surname = "User",
                    Role = "Participant",
                    Is_active = true
                });

            var controller = new authenticator(mock_db.Object, hasher);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.login_user("user@raceday.co.za", "WrongPass");

            //assert
            Assert.IsType<UnauthorizedObjectResult>(result);

        }//end of test

        //Login with correct password returns 200 OK
        [Fact]
        public void login_with_correct_password_returns_200()
        {//start of test

            //arrange
            var hasher = new password_hasher();
            string correct_hash = hasher.hash_password("CorrectPass123");

            var mock_db = new Mock<Iauth_connection>();
            mock_db.Setup(m => m.get_user_by_email(It.IsAny<string>()))
                .Returns(new Users
                {
                    User_id = 42,
                    Email = "user@raceday.co.za",
                    Password_hash = correct_hash,
                    Firstname = "Test",
                    Surname = "User",
                    Role = "Participant",
                    Is_active = true
                });

            var controller = new authenticator(mock_db.Object, hasher);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = SessionHelper.make_context()
            };

            //act
            var result = controller.login_user("user@raceday.co.za", "CorrectPass123");

            //assert
            Assert.IsType<OkObjectResult>(result);

        }//end of test

    }//end of class

}//end of namespace