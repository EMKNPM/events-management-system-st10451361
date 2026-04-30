using Microsoft.AspNetCore.Http;
using ContractMaster.Web.Services;

namespace ContractMaster.Tests;

public class FileValidationTests
{
    [Theory]
    [InlineData("contract.pdf", true)]
    [InlineData("agreement.PDF", true)]
    [InlineData("malware.exe", false)]
    [InlineData("document.txt", false)]
    [InlineData("image.jpg", false)]
    [InlineData("spreadsheet.xlsx", false)]
    public void FileExtensionValidation_OnlyPdfAllowed(string fileName, bool expectedIsValid)
    {
        // Arrange - Create a mock file with PDF header
        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF header
        var fileMock = new FormFile(
            new MemoryStream(fileContent),
            0,
            fileContent.Length,
            "file",
            fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = expectedIsValid ? "application/pdf" : "application/octet-stream"
        };

        // Act - Validate the file
        var result = FileValidationService.ValidateFile(fileMock);

        // Assert - Check the result
        if (expectedIsValid)
        {
            Assert.True(result.IsValid);
        }
        else
        {
            Assert.False(result.IsValid);
            Assert.Contains("Only .pdf files are allowed", result.ErrorMessage);
        }
    }

    [Fact]
    public void EmptyFile_ShouldBeInvalid()
    {
        // Arrange - Create an empty file
        var emptyFile = new FormFile(new MemoryStream(), 0, 0, "file", "empty.pdf");

        // Act - Validate
        var result = FileValidationService.ValidateFile(emptyFile);

        // Assert - Should fail
        Assert.False(result.IsValid);
        Assert.Equal("Please select a file to upload.", result.ErrorMessage);
    }

    [Fact]
    public void NullFile_ShouldBeInvalid()
    {
        // Act - Validate null file
        var result = FileValidationService.ValidateFile(null);

        // Assert - Should fail
        Assert.False(result.IsValid);
        Assert.Equal("Please select a file to upload.", result.ErrorMessage);
    }

    [Fact]
    public void LargeFile_ExceedingMaxSize_ShouldBeInvalid()
    {
        // Arrange - Create a file larger than 10MB (11MB)
        var largeFileStream = new MemoryStream(new byte[11 * 1024 * 1024]);
        var largeFile = new FormFile(largeFileStream, 0, largeFileStream.Length, "file", "large.pdf");

        // Act
        var result = FileValidationService.ValidateFile(largeFile);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("File size cannot exceed", result.ErrorMessage);
    }
}