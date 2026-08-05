namespace Application.Helpers
{
    public class FileNamer
    {
        public static string GetFileNameWithoutExtension(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Path cannot be null or empty.", nameof(fullPath));

            return Path.GetFileNameWithoutExtension(fullPath);
        }

        public static string GetFileExtension(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("Path cannot be null or empty.", nameof(fullPath));

            return Path.GetExtension(fullPath);
        }
    }
}
