using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RmlVentana.Logica
{
    /** El boton «Solicitar»: el pedido que se abre en GitHub y lo que el juego recuerda de el. */
    public static class Solicitudes
    {
        public const string Repositorio = "https://github.com/LordMordelon/RML";

        /** Un issue ya escrito. La linea «- <WorkshopID> <packageId>» es lo unico que va a leer el
         *  action de la etapa 4, asi que va sola en su renglon; el nombre va en el titulo, para leerlo
         *  a simple vista. La etiqueta solo la pone GitHub si quien abre el issue puede etiquetar. */
        public static string Enlace(string Nombre, string WorkshopId, string PackageId)
        {
            string Titulo = "Solicitud: " + Nombre;
            string Cuerpo = "Pido la traducción de este mod.\n\n" +
                            "- " + WorkshopId + " " + PackageId + "\n\n" +
                            "(No cambies la línea de arriba: es la que se lee para contar los pedidos.)";

            return Repositorio + "/issues/new?labels=solicitud" +
                   "&title=" + Uri.EscapeDataString(Titulo) +
                   "&body=" + Uri.EscapeDataString(Cuerpo);
        }

        /** Las solicitudes de mods que ya entraron a RML se borran solas. */
        public static void Limpiar(IDictionary<string, string> Guardadas, ICollection<string> TraducidosPorRml)
        {
            foreach (string Id in Guardadas.Keys.Where(TraducidosPorRml.Contains).ToList())
                Guardadas.Remove(Id);
        }

        /** «18-oct» o «Oct 18», segun el formato y los meses del idioma: {0} es el dia, {1} el mes y
         *  {2} el año, para el formato largo («18-oct-2026»). Vacio si la fecha no se lee. */
        public static string FechaCorta(string FechaIso, string[] Meses, string Formato)
        {
            if (!DateTime.TryParseExact(FechaIso, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime Fecha) ||
                Meses == null || Meses.Length != 12)
                return "";

            return string.Format(Formato, Fecha.Day, Meses[Fecha.Month - 1].Trim(), Fecha.Year);
        }
    }
}
