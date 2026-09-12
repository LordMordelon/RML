using System.Text.Json;

namespace LoadFoldersBuilder;

/** Fecha de la ultima actualizacion de cada mod en el Workshop, sacada de la API publica de Steam.
 *  Solo sirve para la lista de mods (ModList.md y .tsv): no interviene en la carga.
 */
public static class ActualizacionesSteam
{
    private const string Url = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

    /** La API no documenta un maximo de IDs por consulta. Con lotes chicos no hace falta saberlo. */
    private const int TamanoDeLote = 100;

    /** Devuelve, por WorkshopID, la fecha yyyy-MM-dd (UTC) de su ultima actualizacion en Steam.
     *  Nunca tira: el extractor corre el builder y puede estar sin conexion, y eso no es motivo
     *  para no generar la lista. En ese caso devuelve un diccionario vacio.
     */
    public static Dictionary<string, string> Consultar(IEnumerable<string> WorkshopIDs)
    {
        var Fechas = new Dictionary<string, string>();

        try
        {
            using var Cliente = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

            foreach (var Lote in WorkshopIDs.Distinct().Chunk(TamanoDeLote))
            {
                var Campos = new List<KeyValuePair<string, string>> { new("itemcount", Lote.Length.ToString()) };
                for (int i = 0; i < Lote.Length; i++)
                    Campos.Add(new($"publishedfileids[{i}]", Lote[i]));

                using var Respuesta = Cliente.PostAsync(Url, new FormUrlEncodedContent(Campos)).GetAwaiter().GetResult();
                Respuesta.EnsureSuccessStatusCode();

                using var Json = JsonDocument.Parse(Respuesta.Content.ReadAsStream());
                foreach (var Mod in Json.RootElement.GetProperty("response").GetProperty("publishedfiledetails").EnumerateArray())
                {
                    // result distinto de 1: el mod se borro, se oculto o el ID no existe.
                    // No hay fecha que poner.
                    if (!Mod.TryGetProperty("result", out var Resultado) || Resultado.GetInt32() is not 1) continue;
                    if (!Mod.TryGetProperty("time_updated", out var Actualizado)) continue;

                    var Fecha = DateTimeOffset.FromUnixTimeSeconds(Actualizado.GetInt64()).UtcDateTime;
                    Fechas[Mod.GetProperty("publishedfileid").GetString()!] = Fecha.ToString("yyyy-MM-dd");
                }
            }
        }
        catch (Exception e)
        {
            // Sin conexion, Steam caido o una respuesta con otra forma.
            Console.WriteLine("\e[93mNo se pudo consultar Steam: la lista de mods queda sin fecha de actualizacion ({0}).\x1b[0m", e.Message);
            return new Dictionary<string, string>();
        }

        return Fechas;
    }
}
