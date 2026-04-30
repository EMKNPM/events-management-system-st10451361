namespace ContractMaster.Web.Services
{
    public static class FileValidationService
    {
        private static readonly string[] AllowedExtensions = { ".pdf" };
        private static readonly long MaxFileSize = 10 * 1024 * 1024; // 10MB

        public static (bool IsValid, string ErrorMessage) ValidateFile(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Please select a file to upload.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                return (false, $"Only .pdf files are allowed. You uploaded {extension}");
            }

            if (file.Length > MaxFileSize)
            {
                return (false, $"File size cannot exceed {MaxFileSize / 1024 / 1024}MB");
            }

            return (true, string.Empty);
        }
    }
}