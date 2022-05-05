namespace Ponant.Medical.Common.Tests.Tests
{
    using Xunit;

    public class TestStringHelper : TestBase
    {
        #region Tests

        [Theory(DisplayName = "RemoveDiacritics_stringWithoutAnyAccent")]
        [InlineData("stringWithoutAnyAccent")]
        public void RemoveDiacritics_stringWithoutAnyAccent(string stringToBeProcessed)
        {
            string removedDiacriticsString = StringHelper.RemoveDiacritics(stringToBeProcessed);
            Assert.Equal(removedDiacriticsString, stringToBeProcessed);
        }

        [Theory(DisplayName = "RemoveDiacritics_WithAccents_ShouldNotBeEqual")]
        [InlineData("éàèîù")]
        [InlineData("chaîneAvecUnAccent")]
        [InlineData("cetteChaîneEstchaînée")]
        public void RemoveDiacritics_WithAccents_ShouldNotBeEqual(string stringToBeProcessed)
        {
            string removedDiacriticsString = StringHelper.RemoveDiacritics(stringToBeProcessed);
            Assert.NotEqual(removedDiacriticsString, stringToBeProcessed);            
        }

        [Theory(DisplayName = "RemoveDiacritics_WithAccents_CompareStringResult")]
        [InlineData("cetteChaîneEstchaînée")]
        public void RemoveDiacritics_WithAccents_CompareStringResult(string stringToBeProcessed)
        {
            string removedDiacriticsString = StringHelper.RemoveDiacritics(stringToBeProcessed);
            Assert.Equal("cetteChaineEstchainee", removedDiacriticsString);
        }

        [Theory(DisplayName = "CleanFileName_stringWithoutSpecialCharacters")]
        [InlineData("stringWithoutSpecialCharacters")]
        public void CleanFileName_stringWithoutSpecialCharacters(string stringToBeProcessed)
        {
            string cleanedString = StringHelper.CleanFileName(stringToBeProcessed);
            Assert.Equal(cleanedString, stringToBeProcessed);
        }

        [Theory(DisplayName = "CleanFileName_stringWithASpecialCharacter")]
        [InlineData("stringWithA#SpecialCharacter")]
        public void CleanFileName_stringWithASpecialCharacter(string stringToBeProcessed)
        {
            string cleanedString = StringHelper.CleanFileName(stringToBeProcessed);
            Assert.NotEqual(cleanedString, stringToBeProcessed);
            Assert.Equal("stringWithA_SpecialCharacter", cleanedString);
        }

        [Theory(DisplayName = "CleanFileName_stringWithMultipleSpecialCharacters")]
        [InlineData("#string$With%Multiple&Special+Characters*")]
        public void CleanFileName_stringWithMultipleSpecialCharacters(string stringToBeProcessed)
        {
            string cleanedString = StringHelper.CleanFileName(stringToBeProcessed);
            Assert.NotEqual(cleanedString, stringToBeProcessed);
            Assert.Equal("_string_With_Multiple_Special_Characters_", cleanedString);
        }

        #endregion
    }
}
