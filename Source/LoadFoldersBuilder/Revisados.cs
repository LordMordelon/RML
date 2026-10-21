namespace LoadFoldersBuilder;

/** Las actualizaciones de Steam que ya se revisaron y no pedian cambiar la traduccion.
 *  Solo sirve para la lista de mods (docs/index.html): no interviene en la carga.
 *
 *  Viven en Revisados.tsv, en la raiz del repo: WorkshopID, la fecha de la actualizacion
 *  revisada y el nombre del mod, que esta solo para que se pueda leer. Lo escribe revisado.yml
 *  cuando el dueño abre un issue «Revisado: <WorkshopID>» desde la pagina; tambien se puede
 *  editar a mano.
 *
 *  Se guarda la fecha de la actualizacion y no la del dia en que se reviso: si el mod se vuelve
 *  a actualizar despues, la fecha de Steam pasa a ser mas nueva y la etiqueta vuelve sola.
 */
public static class Revisados
{
    public const string Archivo = "Revisados.tsv";

    /** Devuelve, por WorkshopID, la fecha yyyy-MM-dd de la actualizacion revisada. Nunca tira:
     *  sin archivo, o con una linea que no se entiende, la lista sale igual.
     */
    public static Dictionary<string, string> Leer(string RootPath)
    {
        var Fechas = new Dictionary<string, string>();
        string Ruta = Path.Combine(RootPath, Archivo);
        if (!File.Exists(Ruta))
            return Fechas;

        try
        {
            foreach (var Linea in File.ReadLines(Ruta))
            {
                var Campos = Linea.Split('\t');
                // El encabezado y cualquier linea a medio escribir no tienen un ID numerico.
                if (Campos.Length < 2 || !Campos[0].All(char.IsAsciiDigit) || Campos[0].Length is 0)
                    continue;

                if (DateOnly.TryParseExact(Campos[1].Trim(), "yyyy-MM-dd", out _))
                    Fechas[Campos[0]] = Campos[1].Trim();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("\e[93mNo se pudo leer {0}: la lista de mods sale sin las revisiones ({1}).\x1b[0m", Archivo, e.Message);
        }

        return Fechas;
    }
}
