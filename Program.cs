
using System.Security.Cryptography;

class Program
{
    public static void Main(string[] args)
    {
        //Set the console to white and text to black
        Console.BackgroundColor = ConsoleColor.White;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Black;

        //We get the file name from the user
        Console.WriteLine("Welcome to the file integrity checker, please introduce the name and extension of the file you want to check.");
        Console.WriteLine("File must not contain any illegal characters and must end in '.txt' (ex. TestFile.txt)");
        string fileName = GetUserFileName();
        Console.WriteLine("Success " + fileName);

        //We check if the file exists to either create it or not
        while (!File.Exists(fileName))
        {
            Console.Clear();
            Console.WriteLine($"'{fileName}' does not exist, would you like to create it? Y|N");
            bool createFile = YNQuestion();
            if (createFile)
            {
                FileStream newFile = File.Create(fileName);
                //IO. exception. Is being read.
                newFile.Close();
            }
            else
            {
                Console.WriteLine($"Please add a file with the name {fileName} under the following directory: " + Directory.GetCurrentDirectory());
                Console.WriteLine($"Press any key to check for the file {fileName} again.");
                Console.ReadKey();
            }
        }
        Console.Clear();
        Console.WriteLine("Would you want to provide an original checksum for the provided file (Y) or should the current checksum be used? (N) Y|N");
        bool provideChecksum = YNQuestion();
        string originalChecksum;
        if (provideChecksum)
        {
            Console.WriteLine("Please provide a valid checksum.");
            originalChecksum = Console.ReadLine() ?? string.Empty;
        }
        else
        {
            originalChecksum = GenerateCheckSumMD5(fileName);
        }
        Console.Clear();
        Console.WriteLine(fileName + " exists with an initial checksum of " + originalChecksum);
        PromptFileIntegrityCheck(fileName, originalChecksum);
    }

    public static string GetUserFileName()
    {
        string? fileName = Console.ReadLine();

        while (!IsFileValid(fileName))
        {
            Console.Clear();
            Console.WriteLine("File name is not valid. Name must not contain any illegal characters and must end in '.txt' (ex. TestFile.txt)");
            fileName = Console.ReadLine();
        }

        return fileName!;
    }

    public static bool IsFileValid(string? fileName)
    {
        bool fileNameValid = !string.IsNullOrWhiteSpace(fileName) &&
                              fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;

        bool fileNameExtension = fileName!.EndsWith(".txt");

        return fileNameValid && fileNameExtension;
    }

    public static bool YNQuestion()
    {
        string userInput;
        do
        {
            userInput = Console.ReadLine() ?? string.Empty;
            userInput = userInput.ToUpper();
        }
        while (userInput != "Y" && userInput != "N");
        return userInput == "Y";
    }

    public static string GenerateCheckSumMD5(string filepath)
    {
        MD5 md5 = MD5.Create();
        FileStream? stream = File.OpenRead(filepath);
        byte[] checksum = md5.ComputeHash(stream);
        string base64Hash = Convert.ToBase64String(checksum);
        return base64Hash;
    }

    public static string GenerateCheckSumSHA256(string filepath)
    {
        SHA256 sha256 = SHA256.Create();
        FileStream? stream = File.OpenRead(filepath);
        byte[] checksum = sha256.ComputeHash(stream);
        string base64Hash = Convert.ToBase64String(checksum);
        return base64Hash;
    }

    public static void PromptFileIntegrityCheck(string fileName, string originalChecksum)
    {
        Console.WriteLine($"Press any key to check {fileName}´s file integrity.");
        Console.WriteLine("The given file will be checked for a match with the checksum: " + originalChecksum);
        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
        CheckFileIntegrity(fileName, originalChecksum);
    }


    // Method to check file integrity, ie compare checksums
    public static void CheckFileIntegrity(string fileName, string originalChecksum)
    {
        string currentChecksum = GenerateCheckSumMD5(fileName);
        Console.Clear();

        Console.WriteLine(currentChecksum.Equals(originalChecksum)
                        ? "CHECKSUM MATCHES. File Integrity Is Observed."
                        : "CHECKSUM MISMATCH. File Integrity Is Compromised.");

        PromptFileIntegrityCheck(fileName, originalChecksum);
    }
}

