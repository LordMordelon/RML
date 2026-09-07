using System.Text.RegularExpressions;
using System.Xml.Linq;
using RimworldExtractorInternal;

namespace FileNameEncoder;

// dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded -p:ExcludeAllSymbols=true -o "../../out"

static class Program
{
    static void Main(string[] args)
    {
        // Permitir uso no interactivo: FileNameEncoder <ruta> [<ruta> ...]
        if (args.Length > 0)
        {
            foreach (var Ruta in args) FileNameEncoder(Ruta);
            return;
        }

        while (true)
        {
            Console.WriteLine("Ingresa la ruta de la carpeta cuyos archivos quieres renombrar (vacio para salir).");
            // ReadLine devuelve null al cerrarse la entrada. El codigo original
            // hacia continue, lo que dejaba el proceso girando en vano al
            // ejecutarlo con la entrada redirigida.
            if (Console.ReadLine() is not { } UserInput) return;
            if (string.IsNullOrWhiteSpace(UserInput)) return;

            FileNameEncoder(UserInput);
        }
    }
    
    public static void FileNameEncoder(string DirectoryPath)
    {
        DirectoryInfo Directory = new DirectoryInfo(DirectoryPath.Trim().Trim('\"'));
        if (!Directory.Exists)
        {
            Console.WriteLine("La ruta no es valida.");
            return;
        }

        string TranslationFolderName;
        Regex FolderNameChecker = new Regex(@"^\d+\.\d+$", RegexOptions.Compiled);
        if (FolderNameChecker.IsMatch(Directory.Name) && Directory.Parent is not null)
            TranslationFolderName = Directory.Parent.Name;
        else
            TranslationFolderName = Directory.Name;

        var DefInjectedDirectory =
            Directory.EnumerateDirectories("DefInjected", SearchOption.AllDirectories).FirstOrDefault();
        var KeyedDirectory =
            Directory.EnumerateDirectories("Keyed", SearchOption.AllDirectories).FirstOrDefault();
        
        var DefInjectedFiles = DefInjectedDirectory?.EnumerateFiles("*", SearchOption.AllDirectories);
        var KeyedFiles = KeyedDirectory?.EnumerateFiles("*", SearchOption.AllDirectories);

        if (DefInjectedFiles is not null && DefInjectedFiles.Any())
        {
            Dictionary<string, List<string>> Sort = new Dictionary<string, List<string>>();
            foreach (var File in DefInjectedFiles)
            {
                var ClassName = Path.GetRelativePath(DefInjectedDirectory!.FullName, File.DirectoryName!);
                if (!Sort.TryGetValue(ClassName, out var ClassList))
                    Sort.Add(ClassName, new List<string>());
                
                Sort[ClassName].Add(File.FullName);
            }

            foreach (var XMLFiles in Sort)
            {
                if (XMLFiles.Value.Count > 1)
                {
                    foreach (var XMLFile in XMLFiles.Value)
                    {
                        XDocument XDoc = XDocument.Load(XMLFile);
                        var NodName = XDoc.Root.Elements().FirstOrDefault();
                        var NewFileName = Utils.GenerateFileName(TranslationFolderName, XMLFiles.Key, NodName.Name.LocalName) + ".xml";
                        var NewPath = Path.Combine(DefInjectedDirectory!.FullName, XMLFiles.Key, NewFileName);
                        MoveIfNeeded(XMLFile, NewPath);
                    }

                }
                else
                {
                    var NewFileName = Utils.GenerateFileName(TranslationFolderName, XMLFiles.Key) + ".xml";
                    var NewPath = Path.Combine(DefInjectedDirectory!.FullName, XMLFiles.Key, NewFileName);
                    MoveIfNeeded(XMLFiles.Value.First(), NewPath);
                }
                
            }
        }

        RenameFlatFolder(KeyedFiles, KeyedDirectory, TranslationFolderName, "Keyed");

        // Patches no se toca: el extractor ya los nombra con el mod al que le aplica cada
        // uno, que es legible y estable —mismo mod, mismo archivo—, o sea la propiedad por
        // la que existia este renombrado. Codificarlos solo perderia esa informacion.
    }

    /** Keyed y Patches no se agrupan por clase: son una sola carpeta plana.
     *  Con un unico XML se conserva el nombre de dos componentes de siempre.
     *  Con varios hay que desambiguar, porque de lo contrario todos los archivos
     *  apuntan al mismo destino y el segundo File.Move lanza IOException.
     *  Se usa el nombre original del archivo como tercer componente: el extractor
     *  ya lo genera de forma determinista, asi que el hash resultante es estable.
     */
    private static void RenameFlatFolder(IEnumerable<FileInfo>? Files, DirectoryInfo? Directory,
        string TranslationFolderName, string TypeName)
    {
        if (Files is null) return;

        // Materializar antes de mover: EnumerateFiles es perezoso y renombrar
        // durante la enumeracion da resultados indefinidos.
        var XMLFiles = Files.ToList();
        if (XMLFiles.Count == 0) return;

        foreach (var XMLFile in XMLFiles)
        {
            var NewFileName = XMLFiles.Count > 1
                ? Utils.GenerateFileName(TranslationFolderName, TypeName, Path.GetFileNameWithoutExtension(XMLFile.Name)) + ".xml"
                : Utils.GenerateFileName(TranslationFolderName, TypeName) + ".xml";
            var NewPath = Path.Combine(Directory!.FullName, NewFileName);
            MoveIfNeeded(XMLFile.FullName, NewPath);
        }
    }

    /** Mover un archivo sobre si mismo lanza IOException, asi que ejecutar la
     *  herramienta dos veces seguidas fallaria. Con esta guarda es idempotente.
     */
    private static void MoveIfNeeded(string OldPath, string NewPath)
    {
        if (string.Equals(OldPath, NewPath, StringComparison.OrdinalIgnoreCase)) return;
        File.Move(OldPath, NewPath);
    }
}