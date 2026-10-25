using System;
using System.Collections.Generic;

namespace RmlVentana.Logica
{
    public enum Cambio { Anadido, Actualizado, Corregido }

    /** Una fila de Novedades.tsv (Source/LoadFoldersBuilder/Novedades.cs). */
    public class EntradaDeNovedades
    {
        public string PackageId;
        public string Nombre;
        public string Anadido;
        public string Ultimo;
        /** «actualizado», «corregido» o vacio si solo hay la fecha en que se añadio. */
        public string Tipo;
        /** Si cambio en la ultima subida de RML al Workshop: «a» añadido, «c» cambiado, vacio si no.
         *  Null si el archivo no trae la columna (el builder no pudo consultar Steam). */
        public string EnLaUltima;
        /** Los textos cambiados, nuevos y quitados en esa subida (septima columna). Cero si no hay. */
        public int Cambiados, Nuevos, Quitados;
    }

    public class Novedad
    {
        public string PackageId;
        public string Nombre;
        public Cambio Cambio;
        public string Fecha;
        /** Lo que trae Cambios.tsv para este mod, solo si salio como «Actualizado» o «Corregido». */
        public int Cambiados, Nuevos, Quitados;

        /** Si lleva el boton «Ver cambios». */
        public bool TieneDetalle => Cambiados + Nuevos + Quitados > 0;

        /** De la traduccion oficial del juego (TraduccionOficial): la fila parpadea hasta cerrar la ventana. */
        public bool Oficial;

        /** De donde lee «Ver cambios» sus textos. Null es el Cambios.tsv de RML. */
        public string Archivo;
    }

    /** La pestaña «Novedades»: que cambio en RML desde la ultima vez que se abrio la ventana. */
    public static class NovedadesRml
    {
        /** La primera vez no hay «ultima vez»: se muestra lo de este ultimo tiempo. */
        public const int DiasLaPrimeraVez = 30;

        /** Las filas del archivo. El encabezado y las lineas que no tienen forma se saltan. */
        public static List<EntradaDeNovedades> Leer(string Texto)
        {
            var Resultado = new List<EntradaDeNovedades>();
            if (string.IsNullOrEmpty(Texto))
                return Resultado;

            foreach (string Linea in Texto.Split('\n'))
            {
                string[] Campos = Linea.TrimEnd('\r').Split('\t');
                if (Campos.Length < 5 || !EsFecha(Campos[2]) || !EsFecha(Campos[3]))
                    continue;

                var Entrada = new EntradaDeNovedades
                {
                    PackageId = Campos[0].ToLowerInvariant(),
                    Nombre = Campos[1],
                    Anadido = Campos[2],
                    Ultimo = Campos[3],
                    Tipo = Campos[4],
                    EnLaUltima = Campos.Length >= 6 ? Campos[5] : null,
                };

                string[] Cuenta = Campos.Length >= 7 ? Campos[6].Split(',') : new string[0];
                if (Cuenta.Length == 3 && int.TryParse(Cuenta[0], out int C) && int.TryParse(Cuenta[1], out int N) && int.TryParse(Cuenta[2], out int Q))
                {
                    Entrada.Cambiados = C;
                    Entrada.Nuevos = N;
                    Entrada.Quitados = Q;
                }

                Resultado.Add(Entrada);
            }

            return Resultado;
        }

        /** Si el archivo dice que cambio en la ultima subida de RML al Workshop. Sin eso, la primera
         *  vez se muestran los ultimos DiasLaPrimeraVez dias. */
        public static bool PorUltimaActualizacion(List<EntradaDeNovedades> Entradas) =>
            Entradas.Count > 0 && Entradas[0].EnLaUltima != null;

        /** Lo que no se vio, de lo mas nuevo a lo mas viejo. Una fila por mod: si su traduccion es
         *  nueva para el jugador sale como «Añadido», aunque despues haya cambiado. Vistas es lo de
         *  Claves la ultima vez que se cerro la ventana, o null si nunca se abrio: ahi sale solo la
         *  ultima actualizacion de RML (o los ultimos dias, si el archivo no la trae). */
        public static List<Novedad> Pendientes(List<EntradaDeNovedades> Entradas, ICollection<string> Vistas, DateTime Hoy)
        {
            string Desde = Hoy.AddDays(-DiasLaPrimeraVez).ToString("yyyy-MM-dd");
            bool PorUltima = PorUltimaActualizacion(Entradas);
            var Resultado = new List<Novedad>();

            foreach (EntradaDeNovedades Entrada in Entradas)
            {
                bool Cambiado = Entrada.Tipo == "actualizado" || Entrada.Tipo == "corregido";

                bool AnadidoSinVer, CambioSinVer;
                if (Vistas != null)
                {
                    AnadidoSinVer = !Vistas.Contains(ClaveDeAnadido(Entrada));
                    CambioSinVer = Cambiado && !Vistas.Contains(ClaveDeCambio(Entrada));
                }
                else if (PorUltima)
                {
                    AnadidoSinVer = Entrada.EnLaUltima == "a";
                    CambioSinVer = Cambiado && Entrada.EnLaUltima == "c";
                }
                else
                {
                    AnadidoSinVer = string.CompareOrdinal(Entrada.Anadido, Desde) >= 0;
                    CambioSinVer = Cambiado && string.CompareOrdinal(Entrada.Ultimo, Desde) >= 0;
                }

                if (AnadidoSinVer)
                    Resultado.Add(Nueva(Entrada, Cambio.Anadido, Entrada.Anadido));
                else if (CambioSinVer)
                {
                    Novedad Novedad = Nueva(Entrada, Entrada.Tipo == "actualizado" ? Cambio.Actualizado : Cambio.Corregido, Entrada.Ultimo);
                    Novedad.Cambiados = Entrada.Cambiados;
                    Novedad.Nuevos = Entrada.Nuevos;
                    Novedad.Quitados = Entrada.Quitados;
                    Resultado.Add(Novedad);
                }
            }

            Resultado.Sort((A, B) =>
            {
                int PorFecha = string.CompareOrdinal(B.Fecha, A.Fecha);
                return PorFecha != 0 ? PorFecha : string.Compare(A.Nombre, B.Nombre, StringComparison.OrdinalIgnoreCase);
            });
            return Resultado;
        }

        /** Lo que muestra la pestaña: lo no visto y, si ya se vio todo, lo de la ultima actualizacion
         *  de RML (o de los ultimos dias), con YaVistas en true. Al cerrar la ventana todo queda visto, y
         *  sin esto al volver a abrirla desde los ajustes la lista salia vacia y no habia como volver a
         *  «Ver cambios». */
        public static List<Novedad> ParaMostrar(List<EntradaDeNovedades> Entradas, ICollection<string> Vistas, DateTime Hoy, out bool YaVistas)
        {
            var SinVer = Pendientes(Entradas, Vistas, Hoy);
            YaVistas = false;
            if (SinVer.Count > 0 || Vistas == null)
                return SinVer;

            var Ultimas = Pendientes(Entradas, null, Hoy);
            YaVistas = Ultimas.Count > 0;
            return Ultimas;
        }

        /** Lo que se guarda al cerrar la ventana: cada cambio que ya se mostro. */
        public static List<string> Claves(List<EntradaDeNovedades> Entradas)
        {
            var Resultado = new List<string>();
            foreach (EntradaDeNovedades Entrada in Entradas)
            {
                Resultado.Add(ClaveDeAnadido(Entrada));
                Resultado.Add(ClaveDeCambio(Entrada));
            }
            return Resultado;
        }

        private static string ClaveDeAnadido(EntradaDeNovedades Entrada) => "a|" + Entrada.PackageId + "|" + Entrada.Anadido;

        private static string ClaveDeCambio(EntradaDeNovedades Entrada) => "c|" + Entrada.PackageId + "|" + Entrada.Ultimo;

        private static Novedad Nueva(EntradaDeNovedades Entrada, Cambio Cambio, string Fecha) =>
            new Novedad { PackageId = Entrada.PackageId, Nombre = Entrada.Nombre, Cambio = Cambio, Fecha = Fecha };

        private static bool EsFecha(string Texto) =>
            DateTime.TryParseExact(Texto, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _);
    }
}
