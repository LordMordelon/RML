namespace LoadFoldersBuilder;

/** Las actualizaciones de Steam que ya se revisaron y no pedian cambiar la traduccion.
 *  Solo sirve para la lista de mods (docs/index.html): no interviene en la carga.
 *
 *  Viven en Revisados.tsv, en la rama «revisados» y no en master: cada marca es un commit, y
 *  asi no llenan el historial de las traducciones. Columnas: WorkshopID, la fecha de la
 *  actualizacion revisada y el nombre del mod, que esta solo para que se pueda leer. Lo escribe
 *  revisado.yml cuando el dueño abre un issue «Revisado: <WorkshopID>» desde la pagina.
 *
 *  Se guarda la fecha de la actualizacion y no la del dia en que se reviso: si el mod se vuelve
 *  a actualizar despues, la fecha de Steam pasa a ser mas nueva y la etiqueta vuelve sola.
 */
public static class Revisados
{
    public const string Rama = "revisados";
    public const string Archivo = "Revisados.tsv";

    /** Devuelve, por WorkshopID, la fecha yyyy-MM-dd de la actualizacion revisada. Nunca tira:
     *  sin la rama, o con una linea que no se entiende, la lista sale igual, sin revisiones.
     *
     *  Primero la copia del remoto, que es la que escribe revisado.yml: en local queda al dia con
     *  cualquier git pull o git fetch, aunque nadie haya cambiado a esa rama.
     */
    public static Dictionary<string, string> Leer(string RootPath)
    {
        var Fechas = new Dictionary<string, string>();

        var Lineas = UltimosCambios.Git(RootPath, "show", $"origin/{Rama}:{Archivo}")
                     ?? UltimosCambios.Git(RootPath, "show", $"{Rama}:{Archivo}");
        if (Lineas is null)
        {
            Console.WriteLine("\e[93mNo se encontro la rama {0}: la lista de mods sale sin las revisiones.\x1b[0m", Rama);
            return Fechas;
        }

        foreach (var Linea in Lineas)
        {
            var Campos = Linea.TrimEnd('\r').Split('\t');
            // El encabezado y cualquier linea a medio escribir no tienen un ID numerico.
            if (Campos.Length < 2 || Campos[0].Length is 0 || !Campos[0].All(char.IsAsciiDigit))
                continue;

            if (DateOnly.TryParseExact(Campos[1].Trim(), "yyyy-MM-dd", out _))
                Fechas[Campos[0]] = Campos[1].Trim();
        }

        return Fechas;
    }
}
