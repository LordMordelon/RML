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
    }

    public class Novedad
    {
        public string PackageId;
        public string Nombre;
        public Cambio Cambio;
        public string Fecha;
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

                Resultado.Add(new EntradaDeNovedades
                {
                    PackageId = Campos[0].ToLowerInvariant(),
                    Nombre = Campos[1],
                    Anadido = Campos[2],
                    Ultimo = Campos[3],
                    Tipo = Campos[4],
                });
            }

            return Resultado;
        }

        /** Lo que no se vio, de lo mas nuevo a lo mas viejo. Una fila por mod: si su traduccion es
         *  nueva para el jugador sale como «Añadido», aunque despues haya cambiado. Vistas es lo de
         *  Claves la ultima vez que se cerro la ventana, o null si nunca se abrio. */
        public static List<Novedad> Pendientes(List<EntradaDeNovedades> Entradas, ICollection<string> Vistas, DateTime Hoy)
        {
            string Desde = Hoy.AddDays(-DiasLaPrimeraVez).ToString("yyyy-MM-dd");
            var Resultado = new List<Novedad>();

            foreach (EntradaDeNovedades Entrada in Entradas)
            {
                bool Cambiado = Entrada.Tipo == "actualizado" || Entrada.Tipo == "corregido";

                bool AnadidoSinVer = Vistas == null
                    ? string.CompareOrdinal(Entrada.Anadido, Desde) >= 0
                    : !Vistas.Contains(ClaveDeAnadido(Entrada));
                bool CambioSinVer = Cambiado && (Vistas == null
                    ? string.CompareOrdinal(Entrada.Ultimo, Desde) >= 0
                    : !Vistas.Contains(ClaveDeCambio(Entrada)));

                if (AnadidoSinVer)
                    Resultado.Add(Nueva(Entrada, Cambio.Anadido, Entrada.Anadido));
                else if (CambioSinVer)
                    Resultado.Add(Nueva(Entrada, Entrada.Tipo == "actualizado" ? Cambio.Actualizado : Cambio.Corregido, Entrada.Ultimo));
            }

            Resultado.Sort((A, B) =>
            {
                int PorFecha = string.CompareOrdinal(B.Fecha, A.Fecha);
                return PorFecha != 0 ? PorFecha : string.Compare(A.Nombre, B.Nombre, StringComparison.OrdinalIgnoreCase);
            });
            return Resultado;
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
