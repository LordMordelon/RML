using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RmlVentana.Logica
{
    public class EntradaTar
    {
        public string Nombre;
        public DateTime Fecha;
        public byte[] Datos;
    }

    /** Lee el .tar de idioma del juego. .NET Framework no trae lector de tar y el TarDirectory del juego
     *  no da las fechas, que son lo que dice cuando se empaqueto la traduccion. Entiende el formato
     *  ustar y los nombres largos de GNU y de pax; de lo demas, solo los archivos comunes. */
    public static class LectorTar
    {
        private const int Bloque = 512;

        public static IEnumerable<EntradaTar> Leer(Stream Tar)
        {
            var Cabecera = new byte[Bloque];
            string NombreLargo = null;

            while (Llenar(Tar, Cabecera, Bloque))
            {
                if (Cabecera.All(x => x == 0))
                    yield break;

                long Tamano = Octal(Cabecera, 124, 12);
                long Segundos = Octal(Cabecera, 136, 12);
                char Tipo = (char)Cabecera[156];
                byte[] Datos = new byte[Tamano];
                if (!Llenar(Tar, Datos, (int)Tamano))
                    yield break;
                Saltar(Tar, (Bloque - (int)(Tamano % Bloque)) % Bloque);

                if (Tipo == 'L')
                {
                    NombreLargo = Texto(Datos, 0, Datos.Length);
                    continue;
                }
                if (Tipo == 'x')
                {
                    NombreLargo = RutaPax(Encoding.UTF8.GetString(Datos)) ?? NombreLargo;
                    continue;
                }
                if (Tipo != '0' && Tipo != '\0')
                {
                    NombreLargo = null;
                    continue;
                }

                string Nombre = NombreLargo ?? NombreCorto(Cabecera);
                NombreLargo = null;
                yield return new EntradaTar
                {
                    Nombre = Nombre,
                    Fecha = DateTimeOffset.FromUnixTimeSeconds(Segundos).UtcDateTime,
                    Datos = Datos,
                };
            }
        }

        /** La fecha del archivo mas nuevo: la del empaquetado. Null si el tar no trae ninguno. */
        public static DateTime? FechaMasNueva(Stream Tar)
        {
            DateTime? Mayor = null;
            foreach (EntradaTar Entrada in Leer(Tar))
                if (Mayor == null || Entrada.Fecha > Mayor)
                    Mayor = Entrada.Fecha;
            return Mayor;
        }

        private static string NombreCorto(byte[] Cabecera)
        {
            string Nombre = Texto(Cabecera, 0, 100);
            bool Ustar = Encoding.ASCII.GetString(Cabecera, 257, 5) == "ustar";
            string Prefijo = Ustar ? Texto(Cabecera, 345, 155) : "";
            return Prefijo.Length > 0 ? Prefijo + "/" + Nombre : Nombre;
        }

        /** Los registros pax son «<largo> clave=valor\n»; solo interesa «path». */
        private static string RutaPax(string Registros)
        {
            foreach (string Linea in Registros.Split('\n'))
            {
                int Espacio = Linea.IndexOf(' ');
                if (Espacio > 0 && Linea.Substring(Espacio + 1).StartsWith("path="))
                    return Linea.Substring(Espacio + 6);
            }
            return null;
        }

        private static string Texto(byte[] Datos, int Desde, int Largo)
        {
            int Fin = Array.IndexOf(Datos, (byte)0, Desde, Largo);
            return Encoding.UTF8.GetString(Datos, Desde, (Fin < 0 ? Desde + Largo : Fin) - Desde);
        }

        /** Los numeros de la cabecera van en octal, a veces con espacios delante, y terminan en un
         *  espacio o un cero. */
        private static long Octal(byte[] Datos, int Desde, int Largo)
        {
            long Valor = 0;
            bool Empezo = false;
            for (int i = Desde; i < Desde + Largo; i++)
            {
                byte b = Datos[i];
                if (b >= '0' && b <= '7')
                {
                    Valor = Valor * 8 + (b - '0');
                    Empezo = true;
                }
                else if (Empezo || b != ' ')
                    break;
            }
            return Valor;
        }

        private static bool Llenar(Stream Origen, byte[] Destino, int Largo)
        {
            int Leidos = 0;
            while (Leidos < Largo)
            {
                int n = Origen.Read(Destino, Leidos, Largo - Leidos);
                if (n <= 0)
                    return false;
                Leidos += n;
            }
            return true;
        }

        private static void Saltar(Stream Origen, int Largo)
        {
            if (Largo > 0)
                Llenar(Origen, new byte[Largo], Largo);
        }
    }
}
