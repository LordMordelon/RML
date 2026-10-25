using System;
using System.Collections.Generic;
using System.Linq;

namespace RmlVentana.Logica
{
    /** En el orden en que salen en «Ver cambios». */
    public enum Categoria
    {
        Armas, Torretas, Construcciones, Ropa, Criaturas, Plantas, Objetos,
        Investigacion, Recetas, Salud, Escenarios, Interfaz, Otros,
    }

    public class GrupoDeCategoria<T>
    {
        public Categoria Categoria;
        public List<T> Textos;
    }

    /** «Agrupar por categoría» en «Ver cambios» (PLAN-VENTANA.md). Cada texto trae su objeto,
     *  «ThingDef/Gun_LMG» o «Keyed». Lo que no es un ThingDef se reconoce por su tipo, aca; un ThingDef
     *  puede ser arma, torreta, construccion o ropa, y eso lo decide el juego mirando el def cargado
     *  (VentanaCambios.CategoriaDe). */
    public static class Categorias
    {
        private static readonly Dictionary<string, Categoria> PorTipoDeDef = new Dictionary<string, Categoria>
        {
            ["Keyed"] = Categoria.Interfaz,
            ["ResearchProjectDef"] = Categoria.Investigacion,
            ["ResearchTabDef"] = Categoria.Investigacion,
            ["RecipeDef"] = Categoria.Recetas,
            ["HediffDef"] = Categoria.Salud,
            ["BodyDef"] = Categoria.Salud,
            ["BodyPartDef"] = Categoria.Salud,
            ["ScenarioDef"] = Categoria.Escenarios,
            ["PawnKindDef"] = Categoria.Criaturas,
            ["TerrainDef"] = Categoria.Construcciones,
        };

        /** «ThingDef/Gun_LMG» en sus dos partes; «Keyed» no tiene defName. Falso si el objeto esta vacio. */
        public static bool Partir(string Objeto, out string Tipo, out string DefName)
        {
            Tipo = null;
            DefName = null;
            if (string.IsNullOrEmpty(Objeto))
                return false;
            int Barra = Objeto.IndexOf('/');
            Tipo = Barra < 0 ? Objeto : Objeto.Substring(0, Barra);
            DefName = Barra < 0 ? null : Objeto.Substring(Barra + 1);
            return true;
        }

        /** Lo que se sabe sin mirar el def. Otros si el tipo no alcanza, como un ThingDef. */
        public static Categoria PorTipo(string Tipo) =>
            Tipo != null && PorTipoDeDef.TryGetValue(Tipo, out Categoria Categoria) ? Categoria : Categoria.Otros;

        /** «ThingDef/Beer» de una clave de TraduccionOficial («DefInjected/ThingDef/Beer.label»). */
        public static string ObjetoDeClaveOficial(string Clave)
        {
            string[] Partes = Clave.Split('/');
            if (Partes[0] == "Keyed")
                return "Keyed";
            if (Partes[0] == "DefInjected" && Partes.Length >= 3)
                return Partes[1] + "/" + Partes[2].Split('.')[0];
            return "";
        }

        /** Las categorias que tienen textos, en su orden, cada una con sus textos en el orden de antes. */
        public static List<GrupoDeCategoria<T>> Agrupar<T>(IEnumerable<T> Textos, Func<T, Categoria> CategoriaDe) =>
            Textos.GroupBy(CategoriaDe)
                .OrderBy(x => x.Key)
                .Select(x => new GrupoDeCategoria<T> { Categoria = x.Key, Textos = x.ToList() })
                .ToList();
    }
}
