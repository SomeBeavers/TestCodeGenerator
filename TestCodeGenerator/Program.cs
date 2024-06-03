using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;

namespace TestCodeGenerator;

class Program
{
    private const string FolderInBin = "GeneratedCode2";

    static async Task Main(string[] args)
    {
        var useAnotherClassTemplate = Template.Parse(@"
namespace ConsoleApp59;

public class Class{{ number }}
{
    public void ThrowException(string parameter)
    {
        try
        {
            new Class{{ number_plus_one }}().ThrowException(parameter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
");


        var classWithDeclarationTemplate = Template.Parse(@"
namespace ConsoleApp59;

public class Class{{ last_number }}
{
    public void ThrowException(string parameter)
    {
        try
        {
            var list = new List<string>() { """" };
            var enumerable = list.Where(x => x.Length > 0);
            enumerable.ToList().ForEach(x => Console.WriteLine(x));

            throw new MyException($""My custom exception {parameter} thrown after aaaaaaaaa"");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
        ");

        for (int i = 100; i < 300; i++)
        {
            var useAnotherClass = useAnotherClassTemplate.Render(new { number = i, number_plus_one = i+1 });
            WriteToFile(useAnotherClass, "Class"+i);
        }


        var classWithDeclaration = classWithDeclarationTemplate.Render(new { last_number = 300});
        WriteToFile(classWithDeclaration, "Class300");
    }

    private static void WriteToFile(string? source, string className)
    {
        var fileNameString = className + ".cs";
        Directory.CreateDirectory(FolderInBin);
        File.WriteAllText(FolderInBin+"/" + fileNameString, source);

        Console.WriteLine($"Generated file: {fileNameString}");
    }
}