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
//
// -rutas solo comprueba el arbol de Data/ y Ajustes/ (ver Rutas.cs) y sale con 1 si hay algo que
// corregir. La CI lo usa para no publicar un mod que no carga.
//
// -copia comprueba lo mismo y, si esta bien, arma la copia liviana en output/ (ver
// CopiaLimpia.cs). Lo usa 02-armar-copia-limpia.cmd. -build tambien la arma, al final.
//
// -pagina solo reescribe ModList.tsv y docs/index.html, con las fechas de Steam del momento.
// Lo usa pagina-diaria.yml para que la pagina siga a Steam aunque nadie toque Data/: no
// escribe LoadFolders.xml, About.xml ni output/, que no dependen de Steam.
//
// -nota escribe nota-de-steam.txt, la nota de cambios desde la ultima subida al Workshop (ver
// NotaDeSteam.cs). Lo usa 03-subir-al-workshop.cmd; no toca nada mas.
var Comando = args.FirstOrDefault(x => x is "-build" or "-rutas" or "-copia" or "-pagina" or "-nota");

{
    Console.WriteLine("Powered by Rimworld Mod Korean\n::LoadFoldersBuilder::\n");

    if (Statics.IsPathValid is not true)
    {
        Console.WriteLine("\n\e[93mLa configuracion no es correcta.\nComprueba que el ejecutable de LoadFoldersBuilder este dentro de la carpeta del mod para el que genera el LoadFolders.\x1b[0m");
        StopProgram(1);
    }

    if (Statics.ReadSupportedVersions(Statics.RootPath!) is not true)
        StopProgram(1);

    if (Comando is "-rutas")
        StopProgram(Rutas.Informar(Statics.RootPath!) ? 0 : 1);

    if (Comando is "-copia")
        StopProgram(Rutas.Informar(Statics.RootPath!) && Dll.Compilar(Statics.RootPath!) && CopiaLimpia.Armar(Statics.RootPath!) ? 0 : 1);

    if (Comando is "-nota")
        StopProgram(NotaDeSteam.Escribir(Statics.RootPath!) ? 0 : 1);

    if (Comando is "-build" or "-pagina") goto StartBuild;

    Console.WriteLine("\nEscribe '-build' para empezar.");

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
    Console.WriteLine("\n\e[32mEmpieza la generacion\x1b[0m");
    TimeSpan TotalRunTime = TimeSpan.Zero;
    
    // 파일 및 폴더 구조 단위에서 유효성을 검사합니다.
    Stopwatch Stopwatch = Stopwatch.StartNew();
    string[] ValidPath = Statics.FindAndValidatePaths(Statics.TargetPath!);
    if (ValidPath.Length is 0)
    {
        Console.WriteLine("\e[93mNo se encontro ningun LoadFolders.Build.yaml en una ruta valida.\x1b[0m");
        StopProgram(1);
    }
    
    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32mCarpetas y archivos necesarios comprobados...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

    // Solo avisa: el indice se regenera igual, para no dejar al extractor ni a la CI con uno
    // viejo. El que frena la publicacion es -rutas.
    Rutas.Informar(Statics.RootPath!);
    
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
    Console.WriteLine("\e[32mLoadFolders.Build.yaml leidos...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
    
    // BuildRules의 의존 경로 설정을 완성합니다.
    Stopwatch.Restart();
    BuildRules FilteredRules = new BuildRules(BuildQueue);
    FilteredRules.CreateDependencyGraph(Statics.TargetPath!);
    
    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32mDependencias de carga armadas...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

    if (Comando is "-pagina")
    {
        EscribirListaDeMods(FilteredRules);
        Console.WriteLine("\e[32mTerminado\x1b[0m");
        StopProgram();
    }

    // 로드 구문을 작성하기 위한 최종 형태를 완성합니다.
    Stopwatch.Restart();
    LoadRack MainRack = new LoadRack(Statics.BuildVersions!);
    MainRack.Initialize(FilteredRules);

    // Ajustes/ va en su propio LoadRack: en el mismo, la carpeta de Ajustes/ de un mod se
    // juntaria con la de Data/ (la clave es el packageId) y una le pisaria la ruta a la otra.
    BuildRules ReglasDeAjustes = LeerAjustes(Deserializer);
    LoadRack RackDeAjustes = new LoadRack(Statics.BuildVersions!);
    RackDeAjustes.Initialize(ReglasDeAjustes);

    Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
    Console.WriteLine("\e[32mDatos preparados...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

    if (MainRack.GenerateXDocument() is { } CompleteXML && RackDeAjustes.GenerateXDocument() is { } XmlDeAjustes)
    {
        // Los ajustes van arriba de cada version. RimWorld recorre el LoadFolders.xml de abajo
        // hacia arriba (ModContentPack.InitLoadFolders lo invierte) y en Keyed y DefInjected la
        // clave que se lee ultima pisa a la anterior, asi que gana la carpeta de mas arriba.
        foreach (XElement Version in XmlDeAjustes.Root!.Elements())
            CompleteXML.Root!.Element(Version.Name)!.AddFirst(Version.Elements());

        // Common/ va sin IfModActive, para que el juego la cargue siempre: tiene la DLL.
        foreach (XElement Version in CompleteXML.Root!.Elements())
            Version.AddFirst(new XElement("li", Statics.CarpetaComun));

        // LoadFolders.xml 파일을 작성합니다.
        Stopwatch.Restart();

        try
        {
            CompleteXML.Save(Path.Combine(Statics.RootPath!, "LoadFolders.xml"), SaveOptions.None);
        }
        catch (IOException e) when (e.HResult is unchecked((int)0x800704C8))
        {
            Console.WriteLine("\e[93mOtro programa tiene bloqueado LoadFolders.xml.\nCierralo y vuelve a intentarlo.\nSi sigue igual, reinicia el Explorador de Windows desde el Administrador de tareas.\x1b[0m");
            StopProgram(1);
        }
        
        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32mLoadFolders.xml escrito...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);
        
        Stopwatch.Restart();
        EscribirListaDeMods(FilteredRules);
        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;

        // El forceLoadAfter de About.xml, para que RML cargue despues de cada mod que traduce.
        // Con los de Ajustes/, por si un ajuste corrige un texto propio de ese mod.
        Stopwatch.Restart();
        OrdenDeCarga.Actualizar(Statics.RootPath!, FilteredRules.Rules.Values.Concat(ReglasDeAjustes.Rules.Values)
            .SelectMany(Rule => Rule.PackageID));

        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32mforceLoadAfter de About.xml escrito...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

        // La copia liviana en output/, que es la que carga el juego por Mods\RML. Va al final,
        // con el LoadFolders.xml y el About.xml ya al dia, y con la DLL recien compilada. Si
        // algo falla solo avisa: el indice ya quedo bien escrito.
        Stopwatch.Restart();
        Dll.Compilar(Statics.RootPath!);
        CopiaLimpia.Armar(Statics.RootPath!);

        Stopwatch.Stop(); TotalRunTime += Stopwatch.Elapsed;
        Console.WriteLine("\e[32moutput escrito...{0:F3}s\x1b[0m", Stopwatch.Elapsed.TotalSeconds);

        Console.WriteLine("\e[32mTerminado\x1b[0m");
        Console.WriteLine("Tiempo total {0:F3}s", TotalRunTime.TotalSeconds);
    }
    else
    {
        Console.WriteLine("\e[93mHubo un problema al armar el XML.\x1b[0m");
        StopProgram(1);
    }
    
    StopProgram();
}

// 참고용으로 쓸 ModList.tsv 파일을 작성합니다.
// Y la misma lista como docs/index.html (la pagina de GitHub Pages, con links a Steam,
// que se ordena y se filtra).
void EscribirListaDeMods(BuildRules Reglas)
{
    var Cronometro = Stopwatch.StartNew();
    var ModList = Reglas.ExportModList();
    File.WriteAllText(Path.Combine(Statics.RootPath!, "ModList.tsv"), ModListFormatos.Tsv(ModList));
    Directory.CreateDirectory(Path.Combine(Statics.RootPath!, "docs"));
    File.WriteAllText(Path.Combine(Statics.RootPath!, "docs", "index.html"), ModListFormatos.Html(ModList));

    // El grafico de los mas pedidos, sin los que ya estan aca. No se commitea (ver Solicitudes.cs).
    var Pedidos = Solicitudes.Leer(Statics.RootPath!, Solicitudes.Pedidos);
    if (Pedidos is null)
        Console.WriteLine("\e[93mNo se encontro la rama {0}: el grafico de pedidos sale vacio.\x1b[0m", Solicitudes.Rama);
    var EnRml = ModList.Select(Fila => Fila.WorkshopID).OfType<string>().ToHashSet();
    File.WriteAllText(Path.Combine(Statics.RootPath!, "docs", Solicitudes.Pagina), Solicitudes.Html(
        Solicitudes.MasPedidos(Pedidos, Solicitudes.Leer(Statics.RootPath!, Solicitudes.SinTexto), EnRml)));
    Console.WriteLine("\e[32mModList.tsv, docs/index.html y docs/{1} escritos...{0:F3}s\x1b[0m", Cronometro.Elapsed.TotalSeconds, Solicitudes.Pagina);
}

// Las reglas de Ajustes/, armadas igual que las de Data/. Sin la carpeta no hay ninguna.
// No van a ModList.tsv ni a la pagina: no son traducciones de un mod, son retoques.
BuildRules LeerAjustes(IDeserializer Deserializer)
{
    string Carpeta = Statics.AjustesPath!;
    if (!Directory.Exists(Carpeta))
        return new BuildRules([]);

    Console.WriteLine("\nAjustes-> {0}", Carpeta);
    var Reglas = Statics.FindAndValidatePaths(Carpeta)
        .Select(Ruta => Statics.BuildYamlDeserialize(Deserializer, Ruta))
        .OfType<BuildRule>()
        .Where(Regla => Regla.IsValid is true);

    var Resultado = new BuildRules(Reglas);
    Resultado.CreateDependencyGraph(Carpeta);
    return Resultado;
}

void StopProgram(int Codigo = 0)
{
    // Solo se espera el enter si hay alguien para apretarlo. Lanzado desde otro
    // programa o desde la CI no hay teclado, y el codigo de salida es la unica
    // forma que tiene quien llama de enterarse de que algo fallo.
    if (Comando is null)
    {
        Console.WriteLine("\nCierra la ventana o presiona Enter para salir.");
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