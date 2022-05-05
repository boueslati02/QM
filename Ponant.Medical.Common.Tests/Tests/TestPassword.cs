namespace Ponant.Medical.Common.Tests.Tests
{
    using System;
    using System.Security.Cryptography;
    using Xunit;

    public class TestPassword : TestBase
    {
        #region Tests

        [Theory(DisplayName = "CalculateHash_VerifyHashedPasswordLength")]
        [InlineData("fakePassword")]
        public void CalculateHash_VerifyHashedPasswordLength(string password)
        {
            string hashedPassword = UserHelper.CalculateHash(password);

            Assert.NotNull(hashedPassword);
            Assert.Equal(68, hashedPassword.Length);
        }

        [Theory(DisplayName = "CheckPassword_EmptyPassword")]
        [InlineData("")]
        public void CheckPassword_EmptyPassword(string password)
        {
            string hashedPassword = UserHelper.CalculateHash(password);

            bool result = UserHelper.CheckPassword(hashedPassword, password);
            Assert.True(result);
        }

        [Theory(DisplayName = "CheckPassword_FilledPassword")]
        [InlineData("FilledPassword")]
        public void CheckPassword_FilledPassword(string password)
        {
            string hashedPassword = UserHelper.CalculateHash(password);

            bool result = UserHelper.CheckPassword(hashedPassword, password);
            Assert.True(result);
        }

        [Theory(DisplayName = "CheckPassword_WithWrongPassword")]
        [InlineData("password")]
        public void CheckPassword_WithWrongPassword(string password)
        {
            string hashedPassword = UserHelper.CalculateHash(password);
            string wrongPassword = "Password";

            bool result = UserHelper.CheckPassword(hashedPassword, wrongPassword);
            Assert.False(result);
        }

        [Theory(DisplayName = "CreateRandomPassword_CheckLength")]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        public void CreateRandomPassword_CheckLength(bool isRequireDigit, bool isRequireLowercase, bool isRequireNonLetterOrDigit, bool isRequireUppercase)
        {
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] randomNumber = new byte[sizeof(int)];
            random.GetNonZeroBytes(randomNumber);
            int value = (BitConverter.ToInt32(randomNumber, 0));

            int min = 6;
            int max = 14;
            int requireLength = ((value - min) % (max - min + 1) + (max - min + 1)) % (max - min + 1) + min;

            string generatedPassword = UserHelper.CreateRandomPassword(
                isRequireDigit,
                isRequireLowercase,
                isRequireNonLetterOrDigit,
                isRequireUppercase,
                requireLength);
            Assert.Equal(requireLength, generatedPassword.Length);
        }

        #endregion
    }
}
