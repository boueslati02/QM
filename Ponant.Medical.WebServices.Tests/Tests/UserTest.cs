namespace Ponant.Medical.WebServices.Tests.Tests
{
    using Ponant.Medical.Data.Auth;
    using Ponant.Medical.Data.Shore;
    using Ponant.Medical.WebServices.Controllers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Results;
    using Xunit;

    public class UserTest : TestBase
    {
        #region Constructor
        public UserTest()
        {
            AspNetUsers aspNetUser1 = _createObject.AspNetUserCreate(1, "test1", "test1", "test1", AppSettings.RoleBoard);
            AspNetUsers aspNetUser2 = _createObject.AspNetUserCreate(2, "test2", "test2", "test2", AppSettings.RoleBoard);
            AspNetUsers aspNetUser3 = _createObject.AspNetUserCreate(1, "test3", "test3", "test3", null);

            _testAuthContext.AspNetUsers.Add(aspNetUser1);
            _testAuthContext.AspNetUsers.Add(aspNetUser2);
            _testAuthContext.AspNetUsers.Add(aspNetUser3);
        }
        #endregion

        #region Tests

        [Fact(DisplayName = "GetUsers_ShouldReturnException")]
        public void GetUsers_ShouldReturnException()
        {
            UserController controller = new UserController(null);
            ExceptionResult result = controller.GetUsers() as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
            Assert.NotNull(result.Exception.Message);
        }

        [Fact(DisplayName = "GetUsers_ShouldReturnAllLines")]
        public void GetUsers_ShouldReturnAllLines()
        {
            UserController controller = new UserController(_testAuthContext);
            JsonResult<List<User>> result = controller.GetUsers() as JsonResult<List<User>>;

            AspNetUsers user = _testAuthContext.AspNetUsers.First();
            User userBoard = result.Content.First();

            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.IsType<List<User>>(result.Content);
            Assert.Equal(2, result.Content.Count);
            Assert.Equal(user.IdShip, userBoard.IdShip);
            Assert.Equal(user.PasswordHash, userBoard.PasswordHash);
            Assert.Equal(user.UserName, userBoard.UserName);
        }

        [Theory(DisplayName = "ChangePassword_ShouldReturnException")]
        [InlineData("test1", "test1")]
        public void ChangePassword_ShouldReturnException(string username, string password)
        {
            UserController controller = new UserController(null);
            UserBoard userBoard = _createObject.UserBoardCreate(username, password);

            ExceptionResult result = controller.ChangePassword(userBoard) as ExceptionResult;

            Assert.NotNull(result);
            Assert.IsType<ExceptionResult>(result);
            Assert.NotNull(result.Exception);
            Assert.NotNull(result.Exception.Message);
        }

        [Fact(DisplayName = "ChangePassword_ShouldReturnBadRequest")]
        public void ChangePassword_ShouldReturnBadRequest()
        {
            UserController controller = new UserController(_testAuthContext);
            BadRequestErrorMessageResult result = controller.ChangePassword(null) as BadRequestErrorMessageResult;

            Assert.NotNull(result);
            Assert.IsType<BadRequestErrorMessageResult>(result);
            Assert.NotNull(result.Message);
            Assert.Equal("UserBoard not defined.", result.Message);
        }

        [Theory(DisplayName = "ChangePassword_ShouldReturnStatusCode")]
        [InlineData("test1", "test2")]
        public void ChangePassword_ShouldReturnStatusCode(string username, string password)
        {
            UserController controller = new UserController(_testAuthContext);
            UserBoard userBoard = _createObject.UserBoardCreate(username, password);

            OkResult result = controller.ChangePassword(userBoard) as OkResult;
            AspNetUsers user = _testAuthContext.AspNetUsers.First();

            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
            Assert.Equal(userBoard.UserName , user.UserName);
            Assert.Equal(userBoard.PasswordHash, user.PasswordHash);
            Assert.Equal(_testHttpContext.httpContext.User.Identity.Name, user.Editor);
        }

        #endregion
    }
}