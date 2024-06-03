using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Scriban;

namespace TestCodeGenerator;

public class CQRS
{
    static async Task Main1(string[] args)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("DatabaseLib", LanguageNames.CSharp);
        var documents = Directory.GetFiles("C:\\Work\\Test Projects\\CQRS_2024\\DatabaseLib", "*.cs", SearchOption.AllDirectories)
            .Select(file => project.AddDocument(Path.GetFileName(file), File.ReadAllText(file)));

        project = documents.Last().Project;

        var compilation = await project.GetCompilationAsync();

        //        var template = Template.Parse(@"
        //public class {{ model_name }}CreateCommand
        //{
        //    // Add properties here
        //}
        //    public record AddAnimalCommand(AnimalCommandModel AnimalCommandModel) : IRequest<int>;

        //public class {{ model_name }}CreateCommandHandler : IRequestHandler<{{ model_name }}CreateCommand, {{ model_name }}>
        //{
        //    private readonly DbContext _context;

        //    public {{ model_name }}CreateCommandHandler(DbContext context)
        //    {
        //        _context = context;
        //    }

        //    public async Task<{{ model_name }}> Handle({{ model_name }}CreateCommand request, CancellationToken cancellationToken)
        //    {
        //        var entity = new {{ model_name }}
        //        {
        //            // Assign properties here
        //        };

        //        _context.Add(entity);
        //        await _context.SaveChangesAsync(cancellationToken);

        //        return entity;
        //    }
        //}");

        List<Template> templates = new();
        var createCommand = Template.Parse(@"
using MediatR;
using QueryCommandHandler_Web.CommandModels;

namespace QueryCommandHandler_Web.Command
{
    public record {{ model_name }}{{ action_name }}Command({{ model_name }}CommandModel {{ model_name }}CommandModel) : IRequest<int>;
}
");


        //    var updateCommand = Template.Parse(@"
        //public record {{ model_name }}UpdateCommand({{ model_name }}CommandModel {{ model_name }}CommandModel) : IRequest<int>;");
        //    var removeCommand = Template.Parse(@"
        //public record {{ model_name }}RemoveCommand({{ model_name }}CommandModel {{ model_name }}CommandModel) : IRequest<int>;");

        templates = [createCommand/*, updateCommand, removeCommand*/];

        //        var createModel = Template.Parse(@"
        //using DatabaseLib.Model;

        //namespace QueryCommandHandler_Web.CommandModels;

        //public class {{ model_name }}CommandModel
        //{
        //    public int Id { get; set; }
        //}

        //public static class {{ model_name }}CommandModelExtensions
        //{
        //    public static {{ model_name }} ToDB{{ model_name }}(this {{ model_name }}CommandModel " + @"{{ model_name }}".ToLower() + @")
        //    {
        //        return new {{ model_name }}()
        //        {
        //        };
        //    }
        //}
        //");
        //        templates = [createModel];

        //        var createCommandHandler = Template.Parse(@"
        //using DatabaseLib;
        //using MediatR;
        //using QueryCommandHandler_Web.CommandModels;

        //namespace QueryCommandHandler_Web.Command
        //{
        //    internal sealed class {{ model_name }}{{ action_name }}CommandHandler(AnimalContext context) : IRequestHandler<{{ model_name }}{{ action_name }}Command, int>
        //    {
        //        public async Task<int> Handle({{ model_name }}{{ action_name }}Command request, CancellationToken cancellationToken)
        //        {
        //            // context.Animals.Add(request.AnimalCommandModel.ToAnimal());
        //            return await context.SaveChangesAsync(cancellationToken);
        //        }
        //    }
        //}
        //");

        //        templates = [createCommandHandler];
        //        var createCommand = Template.Parse(@"
        //using MediatR;
        //using QueryCommandHandler_Web.CommandModels;

        //namespace QueryCommandHandler_Web.Command
        //{
        //    public record {{ model_name }}{{ action_name }}Command({{ model_name }}CommandModel {{ model_name }}CommandModel) : IRequest<int>;
        //}
        //");
        //        var createQuery = Template.Parse(@"
        //using MediatR;
        //using QueryCommandHandler_Web.QueryModels;

        //namespace QueryCommandHandler_Web.Query
        //{
        //    public record {{ model_name }}{{ action_name }}Query(int Id) : IRequest<{{ model_name }}QueryModel>;
        //}
        //");

        //        templates = [createQuery];

        //        var createModel = Template.Parse(@"
        //using DatabaseLib.Model;

        //namespace QueryCommandHandler_Web.QueryModels;

        //        public class {{ model_name }}QueryModel
        //        {
        //            public int Id { get; set; }
        //        }

        //        public static class {{ model_name }}QueryModelExtensions
        //        {
        //    public static {{ model_name }}QueryModel To{{ model_name }}QueryModel(this {{ model_name }}? {{ model_name2 }})
        //    {
        //        return new {{ model_name }}QueryModel { Id = {{ model_name2 }}.Id};
        //    }
        //        }
        //        ");
        //        templates = [createModel];

        var createQueryHandler = Template.Parse(@"
using DatabaseLib;
using MediatR;
using QueryCommandHandler_Web.Query;
using QueryCommandHandler_Web.QueryModels;

namespace QueryCommandHandler_Web.QueryHandler
{
    internal sealed class {{ model_name }}{{ action_name }}QueryHandler(AnimalContext context)
        : IRequestHandler<{{ model_name }}{{ action_name }}Query, {{ model_name }}QueryModel>
    {
        public async Task<{{ model_name }}QueryModel> Handle({{ model_name }}{{ action_name }}Query request, CancellationToken cancellationToken)
        {
            return (await context.{{ model_name }}s.FindAsync(request.Id, cancellationToken))!.To{{ model_name }}QueryModel();
        }
    }
}
        ");

        templates = [createQueryHandler];



        foreach (var document in documents)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var modelClasses = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var modelClass in modelClasses)
            {
                var className = modelClass.Identifier.Text;
                foreach (var template in templates)
                {
                    //var source1 = template.Render(new { model_name = className, action_name = "Create" });
                    //WriteToFile(source1, className, "Create", "CommandHandler");

                    //var source2 = template.Render(new { model_name = className, action_name = "Update" });
                    //WriteToFile(source2, className, "Update", "CommandHandler");
                    //var source3 = template.Render(new { model_name = className, action_name = "Remove" });
                    //WriteToFile(source3, className, "Remove", "CommandHandler");

                    //var source4 = template.Render(new { model_name = className });
                    //WriteToFile(source4, className, "CommandModel");

                    //var source1 = template.Render(new { model_name = className, model_name2 = className.ToLower()});
                    //WriteToFile(source1, className, null, "QueryModel");

                    var source1 = template.Render(new { model_name = className, action_name = "GetById" });
                    WriteToFile(source1, className, "GetById", "QueryHandler");

                }
            }
        }
    }

    private static void WriteToFile(string? source, string className, string? actionName, string command)
    {
        var fileName = Template.Parse(@"{{ model_name }}{{ action_name }}" + command + @".cs");
        var fileNameString = fileName.Render(new { model_name = className, action_name = actionName });
        Directory.CreateDirectory("GeneratedCode8");
        File.WriteAllText("GeneratedCode8/" + fileNameString, source);

        Console.WriteLine($"Generated file: {fileNameString}");
    }
}