// Powered by Rimworld Mod Korean and follows its copyright policy.

using System.Diagnostics;
using System.Xml.Linq;
using LoadFoldersBuilder;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// [터미널 빌드 커맨드]
// Rider 문제인지 MSBuild 문제인지 버튼으로 하면 Main 진입점을 못찾음
// dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=embedded -p:ExcludeAllSymbols=true -o "../../out"

// Permitir uso no interactivo: LoadFoldersBuilder -build
//
// Sin esto el builder no se puede lanzar desde otro programa, y no es que sea
// incomodo: con la entrada redirigida ReadLine devuelve null, cae en el default
// del switch y el proceso gira para siempre. Va declarado aca afuera para que lo
// vean las funciones locales del final.
var Comando = args.FirstOrDefault(x => x is "-build");

{
    Console.WriteLine("Powered by Rimworld Mod Korean\n::LoadFoldersBuilder::\n");

    if (Statics.IsPathValid is not true)
    {
        Console.WriteLine("\n\e[93m구성 환경이 올바르지 않습니다.\nLoadFoldersBuilder 실행 파일이 LoadFolders를 생성할 올바른 모드 폴더 아래에 있는지 확인하세요.\x1b[0m");
        StopProgram(1);
    }

    if (Statics.ReadSupportedVersions(Statics.RootPath!) is not true)
        StopProgram(1);

    if (Comando is "-build") goto StartBuild;

    Console.WriteLine("\n빌드를 시작하려면 '-build' 명령어를 입력하세요.");

    while (true)
    {
        switch (Console.ReadLine())
        {
            case "-build": goto StartBuild;
            // Se acabo la entrada y no va a llegar ningun comando mas.
            case null: StopProgram(1); break;
            default: ClearLastLine(); break;
        }
    }
    
    StartBuild: // 하지마루요
    Console.WriteLine("\n\e[32m빌드 시작\x1b[0m");
    TimeSpan TotalRunTime = TimeSpan.Zero;
    
    // 파일 및 폴더 구조 단위에서 유효성을 검사합니다.
    Stopwatch Stopwatch = Stopwatch.StartNew();
    string[] ValidPath = Statics.FindAndValidatePaths(Statics.TargetPath!);
    if (ValidPath.Length is 0)
    {
        Console.WriteLine("\e[93m유효한 경로에 위치한 LoadFolders.Build.yaml 파일을 하나도 찾을 수 없습니다.\x1b[0m");
        StopProgram(1);
    }
    
    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32m폴더 구조 및 필수 파일 확인 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
    
    // LoadFolders.Build.yaml을 불러들여 BuildRule 타입으로 변환합니다.
    Stopwatch.Restart();
    IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(NullNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    List<BuildRule> BuildQueue = new List<BuildRule>(ValidPath.Length);
    for (int i = 0; i < ValidPath.Length; i++)
    {
        if (Statics.BuildYamlDeserialize(Deserializer, ValidPath[i]) is {} FriedFish && FriedFish.IsValid is true)
            BuildQueue.Add(FriedFish);
    }
    
    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32mLoadFolders.Build.yaml 읽기 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
    
    // BuildRules의 의존 경로 설정을 완성합니다.
    Stopwatch.Restart();
    BuildRules FilteredRules = new BuildRules(BuildQueue);
    FilteredRules.CreateDependencyGraph(Statics.TargetPath!);
    
    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32m로드 의존성 그래프 생성 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
    
    // 로드 구문을 작성하기 위한 최종 형태를 완성합니다.
    Stopwatch.Restart();
    LoadRack MainRack = new LoadRack(Statics.BuildVersions!);
    MainRack.Initialize(FilteredRules);

    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32m데이터 전처리 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

    if (MainRack.GenerateXDocument() is { } CompleteXML)
    {

        // LoadFolders.xml 파일을 작성합니다.
        Stopwatch.Restart();

        try
        {
            CompleteXML.Save(Path.Combine(Statics.RootPath!, "LoadFolders.xml"), SaveOptions.None);
        }
        catch (IOException e) when (e.HResult is unchecked((int)0x800704C8))
        {
            Console.WriteLine("\e[93m다른 프로그램이 LoadFolders.xml의 쓰기 권한을 점유하고 있습니다.\n해당 프로그램을 닫고 다시 시도하세요.\n문제가 해결되지 않는다면 작업 관리자에서 Windows 탐색기를 '다시 시작'해보세요.\x1b[0m");
            StopProgram(1);
        }
        
        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32mLoadFolders.xml 작성 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
        
        // 참고용으로 쓸 ModList.tsv 파일을 작성합니다.
        // Y la misma lista como ModList.md (se ve en GitHub, con links a Steam) y como
        // docs/index.html (la pagina de GitHub Pages, que se ordena y se filtra).
        Stopwatch.Restart();
        var ModList = FilteredRules.ExportModList();
        File.WriteAllText(Path.Combine(Statics.RootPath!, "ModList.tsv"), ModListFormatos.Tsv(ModList));
        File.WriteAllText(Path.Combine(Statics.RootPath!, "ModList.md"), ModListFormatos.Markdown(ModList));
        Directory.CreateDirectory(Path.Combine(Statics.RootPath!, "docs"));
        File.WriteAllText(Path.Combine(Statics.RootPath!, "docs", "index.html"), ModListFormatos.Html(ModList));

        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32mModList.tsv, ModList.md, docs/index.html 작성 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

        // El forceLoadAfter de About.xml, para que RML cargue despues de cada mod que traduce.
        Stopwatch.Restart();
        OrdenDeCarga.Actualizar(Statics.RootPath!, FilteredRules.Rules.Values.SelectMany(Rule => Rule.PackageID));

        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32mAbout.xml forceLoadAfter 작성 완료...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

        Console.WriteLine("\e[32m작업 완료\x1b[0m");
        Console.WriteLine("총 작업시간 {0:F3}s", TotalRunTime.TotalSeconds);
    }
    else
    {
        Console.WriteLine("\e[93mXML 구조를 형성하는 중 문제가 발생했습니다.\x1b[0m");
        StopProgram(1);
    }
    
    StopProgram();
}

void StopProgram(int Codigo = 0)
{
    // Solo se espera el enter si hay alguien para apretarlo. Lanzado desde otro
    // programa o desde la CI no hay teclado, y el codigo de salida es la unica
    // forma que tiene quien llama de enterarse de que algo fallo.
    if (Comando is null)
    {
        Console.WriteLine("\n창을 닫거나 엔터키를 눌러 프로그램을 종료하십시오.");
        Console.ReadLine();
    }

    Environment.Exit(Codigo);
}

void ClearLastLine()
{
    // Sin consola no hay cursor que mover: Console.CursorTop tira IOException
    // cuando la salida esta redirigida.
    if (Console.IsOutputRedirected)
        return;

    if (Console.CursorTop > 0)
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.BufferWidth)); 
        Console.SetCursorPosition(0, Console.CursorTop);
    }
}