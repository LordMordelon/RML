using System;
using System.Collections.Generic;
using System.Linq;

namespace RmlVentana.Logica
{
    /** Lo que cambio en un mod activo desde su copia en el PC (CopiaDeTextos). Visto se pone al cerrar
     *  la ventana, cuando la copia ya quedo al dia: abierta de nuevo en la misma partida, no sale. */
    public class CambiosDeUnMod
    {
        public string PackageId;
        /** El de la carpeta de RML, para un mod que no esta en Novedades.tsv. */
        public string Nombre;
        public int Cambiados, Nuevos, Quitados;
        public bool Visto;

        public int Total => Cambiados + Nuevos + Quitados;
    }

    /** «Ver cambios» contra la version que el jugador tenia en su PC (PLAN-CAMBIOS-LOCALES.md). Las filas
     *  siguen saliendo de Novedades.tsv (NovedadesRml), salvo para los mods con copia, que tienen la
     *  respuesta exacta: si cambiaron, salen con lo del PC; si no, no salen. Un mod sin copia (inactivo,
     *  o la primera vez) sigue con lo del builder. */
    public static class CambiosLocales
    {
        /** El Cambios.tsv local, en Config/, con el mismo formato que el del builder. */
        public const string Archivo = "RML_Cambios.tsv";

        public static CambiosDeUnMod Contar(string PackageId, string Nombre, List<TextosDeTraduccion.Cambio> Cambios) =>
            new CambiosDeUnMod
            {
                PackageId = PackageId,
                Nombre = Nombre,
                Cambiados = Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Cambiado),
                Nuevos = Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Nuevo),
                Quitados = Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Quitado),
            };

        /** Las filas de la pestaña con lo del PC. Pendientes y YaVistas son los de NovedadesRml.ParaMostrar;
         *  Locales, los mods con copia (con cero cambios si no cambiaron); ArchivoLocal, donde quedo su
         *  detalle. Algo del PC sin ver reemplaza a lo ya visto. */
        public static List<Novedad> Aplicar(List<Novedad> Pendientes, bool YaVistas, List<EntradaDeNovedades> Entradas,
                                            IDictionary<string, CambiosDeUnMod> Locales, string ArchivoLocal, string Hoy, out bool YaVistasDespues)
        {
            var SinVer = Locales.Values.Where(x => !x.Visto && x.Total > 0).ToList();
            YaVistasDespues = YaVistas && SinVer.Count == 0;
            if (YaVistasDespues)
                return Pendientes;

            var Resultado = (YaVistas ? new List<Novedad>() : Pendientes.ToList())
                .Where(x => x.Oficial || !Locales.ContainsKey(x.PackageId))
                .ToList();

            foreach (CambiosDeUnMod Mod in SinVer)
            {
                EntradaDeNovedades Entrada = Entradas.FirstOrDefault(x => x.PackageId == Mod.PackageId);
                Resultado.Add(new Novedad
                {
                    PackageId = Mod.PackageId,
                    Nombre = Entrada?.Nombre ?? Mod.Nombre,
                    // Si tenia copia, el mod no es nuevo para el jugador aunque el builder lo de por añadido.
                    Cambio = Entrada?.Tipo == "actualizado" ? Cambio.Actualizado : Cambio.Corregido,
                    Fecha = Entrada?.Ultimo ?? Hoy,
                    Cambiados = Mod.Cambiados,
                    Nuevos = Mod.Nuevos,
                    Quitados = Mod.Quitados,
                    Archivo = ArchivoLocal,
                });
            }

            NovedadesRml.Ordenar(Resultado);
            return Resultado;
        }
    }
}
