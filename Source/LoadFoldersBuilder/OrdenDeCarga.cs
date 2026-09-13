using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LoadFoldersBuilder;

/** El forceLoadAfter de About/About.xml, con el packageId de cada mod traducido.
 *  Hace que RimWorld cargue RML despues de todos ellos, que es lo que hace ganar a sus
 *  traducciones, sin depender de que el jugador lo ponga ultimo a mano. Un packageId que no
 *  esta activo se ignora, asi que la lista puede llevarlos todos.
 */
public static class OrdenDeCarga
{
    /** El bloque que es de este generador. El resto del About.xml se escribe a mano. */
    private static readonly Regex Bloque = new(@"[ \t]*<forceLoadAfter>.*?</forceLoadAfter>",
        RegexOptions.Singleline | RegexOptions.Compiled);

    /** Reescribe el forceLoadAfter, o lo agrega antes de </ModMetaData> si no esta.
     *  Solo toca ese bloque, sin reformatear lo demas: por eso es texto y no un XDocument,
     *  que al guardar cambiaria la sangria y los saltos de linea de todo el archivo.
     *  Nunca tira: si algo sale mal avisa y deja el About.xml como estaba.
     */
    public static void Actualizar(string RootPath, IEnumerable<string> PackageIDs)
    {
        string AboutPath = Path.Combine(RootPath, "About", "About.xml");

        try
        {
            byte[] Bytes = File.ReadAllBytes(AboutPath);
            bool ConBom = Bytes.AsSpan().StartsWith(Encoding.UTF8.Preamble);
            string Texto = new UTF8Encoding(false).GetString(Bytes, ConBom ? 3 : 0, Bytes.Length - (ConBom ? 3 : 0));

            // El salto de linea que ya usa el archivo, para no mezclar dos en el mismo.
            string Salto = Texto.Contains("\r\n") ? "\r\n" : "\n";

            var Nuevo = new StringBuilder();
            Nuevo.Append("    <forceLoadAfter>").Append(Salto);
            Nuevo.Append("        <!-- GENERADO por Source/LoadFoldersBuilder: el packageId de cada mod de Data/. No editar a mano. -->").Append(Salto);
            // Ordenados y sin repetir, para que dos corridas iguales den el mismo archivo.
            foreach (string PackageID in PackageIDs.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
                Nuevo.Append("        <li>").Append(new XText(PackageID)).Append("</li>").Append(Salto);
            Nuevo.Append("    </forceLoadAfter>");

            string Resultado;
            if (Bloque.IsMatch(Texto))
                Resultado = Bloque.Replace(Texto, Nuevo.ToString().Replace("$", "$$"), 1);
            else if (Texto.LastIndexOf("</ModMetaData>", StringComparison.Ordinal) is var Cierre and >= 0)
                Resultado = Texto.Insert(Cierre, Nuevo + Salto + Salto);
            else
            {
                Console.WriteLine("\e[93mAbout.xml no tiene </ModMetaData>: no se actualizo su forceLoadAfter.\x1b[0m");
                return;
            }

            // Que siga siendo un XML valido antes de pisar el archivo.
            XDocument.Parse(Resultado);

            if (Resultado != Texto)
                File.WriteAllBytes(AboutPath, [.. (ConBom ? Encoding.UTF8.Preamble : []), .. new UTF8Encoding(false).GetBytes(Resultado)]);
        }
        catch (Exception e)
        {
            Console.WriteLine("\e[93mNo se pudo actualizar el forceLoadAfter de About.xml ({0}).\x1b[0m", e.Message);
        }
    }
}
