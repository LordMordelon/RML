using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    /** Las piezas del boton que no deciden nada pero leen lo que viene de afuera: GitHub, el zip, el
     *  .tar del juego y lo que queda guardado en los ajustes. */
    [TestClass]
    public class TraduccionOficialPiezasTests
    {
        private static readonly DateTime Fecha = new DateTime(2026, 10, 17, 0, 15, 0, DateTimeKind.Utc);

        [TestMethod]
        public void ConOtraVersionEnGitHubNoSeOfreceActualizar()
        {
            Assert.AreEqual(EstadoOficial.OtraVersion,
                TraduccionOficial.EstadoDe(Fecha.AddDays(2), Fecha, null, TarPresente: true, FaltaReiniciar: false, MismaVersion: false));
        }

        [TestMethod]
        public void ConOtraVersionLoInstaladoSinUsarSeSigueOfreciendoBorrar()
        {
            Assert.AreEqual(EstadoOficial.SinUsar,
                TraduccionOficial.EstadoDe(Fecha.AddDays(2), Fecha, Fecha, TarPresente: true, FaltaReiniciar: false, MismaVersion: false));
        }

        [TestMethod]
        public void UnDlcAtrasadoAlcanzaParaQueHayaArreglos()
        {
            Assert.AreEqual(EstadoOficial.HayArreglos,
                TraduccionOficial.Combinar(new[] { EstadoOficial.AlDia, EstadoOficial.HayArreglos, EstadoOficial.AlDia }));
        }

        [TestMethod]
        public void ElEstadoDeTodoEsElMasUrgenteDeLasPartes()
        {
            Assert.AreEqual(EstadoOficial.SinUsar, TraduccionOficial.Combinar(new[] { EstadoOficial.HayArreglos, EstadoOficial.SinUsar }));
            Assert.AreEqual(EstadoOficial.AlDia, TraduccionOficial.Combinar(new[] { EstadoOficial.AlDia, EstadoOficial.AlDia }));
            Assert.AreEqual(EstadoOficial.OtraVersion, TraduccionOficial.Combinar(new[] { EstadoOficial.HayArreglos, EstadoOficial.OtraVersion }));
        }

        [TestMethod]
        public void SinPartesNoSeSabe()
        {
            Assert.AreEqual(EstadoOficial.SinConsulta, TraduccionOficial.Combinar(new EstadoOficial[0]));
        }

        [TestMethod]
        public void LaRespuestaDeUnaCarpetaEsUnaListaConUnCambio()
        {
            string Json = "[{\"sha\": \"0123456789abcdef0123456789abcdef01234567\", \"commit\": {" +
                          "\"committer\": {\"name\": \"GitHub\", \"date\": \"2026-09-02T12:00:00Z\"}}}]";

            Assert.IsTrue(TraduccionOficial.LeerCommit(Json, out _, out DateTime Cuando));
            Assert.AreEqual(new DateTime(2026, 9, 2, 12, 0, 0, DateTimeKind.Utc), Cuando);
            Assert.IsFalse(TraduccionOficial.LeerCommit("[]", out _, out _));
        }

        [TestMethod]
        public void UnXmlConMarcaDeOrdenDeBytesSeLeeIgual()
        {
            var Textos = TraduccionOficial.LeerXml("\uFEFF<?xml version=\"1.0\" encoding=\"UTF-8\"?><LanguageData><a>uno</a></LanguageData>", "Keyed/");

            Assert.AreEqual("uno", Textos["Keyed/a"].Texto);
        }

        [TestMethod]
        public void ElPrefijoEsLaCarpetaDelDefOKeyed()
        {
            Assert.AreEqual("DefInjected/ThingDef/", TraduccionOficial.PrefijoDe("DefInjected/ThingDef/Alcohol_Beer.xml"));
            Assert.AreEqual("DefInjected/ThingDef/", TraduccionOficial.PrefijoDe("DefInjected\\ThingDef\\Alcohol_Beer.xml"));
            Assert.AreEqual("Keyed/", TraduccionOficial.PrefijoDe("Keyed/Misc.xml"));
            Assert.AreEqual("Keyed/", TraduccionOficial.PrefijoDe("Keyed/Sub/Misc.xml"));
        }

        [TestMethod]
        public void LoQueNoEsKeyedNiDefInjectedNoSeCompara()
        {
            Assert.IsNull(TraduccionOficial.PrefijoDe("Strings/Names/Animal_Male.txt"));
            Assert.IsNull(TraduccionOficial.PrefijoDe("LanguageInfo.xml"));
            Assert.IsNull(TraduccionOficial.PrefijoDe("DefInjected/ThingDef/"));
        }

        [TestMethod]
        public void DeLaRespuestaDeGitHubSaleElCodigoYLaFecha()
        {
            string Json = "{\"sha\": \"0123456789abcdef0123456789abcdef01234567\", \"node_id\": \"x\", \"commit\": {" +
                          "\"author\": {\"name\": \"A\", \"date\": \"2026-10-18T10:00:00Z\"}," +
                          "\"committer\": {\"name\": \"GitHub\", \"email\": \"n\", \"date\": \"2026-10-19T01:04:37Z\"}," +
                          "\"message\": \"Update\"}, \"parents\": [{\"sha\": \"ffffffffffffffffffffffffffffffffffffffff\"}]}";

            Assert.IsTrue(TraduccionOficial.LeerCommit(Json, out string Sha, out DateTime Cuando));
            Assert.AreEqual("0123456789abcdef0123456789abcdef01234567", Sha);
            Assert.AreEqual(new DateTime(2026, 10, 19, 1, 4, 37, DateTimeKind.Utc), Cuando);
            Assert.AreEqual(DateTimeKind.Utc, Cuando.Kind);
        }

        [TestMethod]
        public void UnaRespuestaRaraDeGitHubNoSeLee()
        {
            Assert.IsFalse(TraduccionOficial.LeerCommit("{\"message\": \"API rate limit exceeded\"}", out _, out _));
            Assert.IsFalse(TraduccionOficial.LeerCommit(null, out _, out _));
        }

        [TestMethod]
        public void ElLectorDeTarDaNombreFechaYContenido()
        {
            byte[] Tar = ArmarTar(("Keyed/Misc.xml", "<LanguageData/>", Fecha), ("DefInjected/ThingDef/" + new string('a', 120) + ".xml", "x", Fecha.AddHours(-1)));

            var Entradas = LectorTar.Leer(new MemoryStream(Tar)).ToList();

            Assert.AreEqual(2, Entradas.Count);
            Assert.AreEqual("Keyed/Misc.xml", Entradas[0].Nombre);
            Assert.AreEqual("<LanguageData/>", Encoding.UTF8.GetString(Entradas[0].Datos));
            Assert.AreEqual(Fecha, Entradas[0].Fecha);
            Assert.AreEqual("DefInjected/ThingDef/" + new string('a', 120) + ".xml", Entradas[1].Nombre);
        }

        [TestMethod]
        public void LaFechaDelTarEsLaDeSuArchivoMasNuevo()
        {
            byte[] Tar = ArmarTar(("Keyed/a.xml", "a", Fecha.AddDays(-5)), ("Keyed/b.xml", "b", Fecha));

            Assert.AreEqual(Fecha, LectorTar.FechaMasNueva(new MemoryStream(Tar)));
        }

        [TestMethod]
        public void LaRaizDelZipEsLaCarpetaQueEnvuelveTodo()
        {
            Assert.AreEqual("RimWorld-SpanishLatin-abc/", TraduccionOficial.RaizDelZip(new[]
            {
                "RimWorld-SpanishLatin-abc/", "RimWorld-SpanishLatin-abc/README.md", "RimWorld-SpanishLatin-abc/Core/LanguageInfo.xml",
            }));
            Assert.AreEqual("", TraduccionOficial.RaizDelZip(new[] { "README.md", "Core/LanguageInfo.xml" }));
        }

        [TestMethod]
        public void UnaRutaQueSaleDeSuCarpetaNoSeExtrae()
        {
            Assert.IsFalse(TraduccionOficial.RutaSegura("Core/../../Windows/x.dll"));
            Assert.IsFalse(TraduccionOficial.RutaSegura("/Windows/x.dll"));
            Assert.IsFalse(TraduccionOficial.RutaSegura("C:/Windows/x.dll"));
            Assert.IsTrue(TraduccionOficial.RutaSegura("Core/Keyed/Misc.xml"));
        }

        [TestMethod]
        public void UnaFilaGuardadaSeVuelveUnaNovedad()
        {
            string Guardada = NovedadOficial.Linea("Biotech", "2026-10-19", 3, 1, 0);

            Novedad Leida = NovedadOficial.Leer(Guardada, Nombre: "Biotech (traducción oficial)", Archivo: "c:/x.tsv");

            Assert.AreEqual("ludeon.rimworld.biotech", Leida.PackageId);
            Assert.AreEqual("Biotech", NovedadOficial.CarpetaDe(Guardada));
            Assert.AreEqual("Biotech (traducción oficial)", Leida.Nombre);
            Assert.AreEqual(Cambio.Actualizado, Leida.Cambio);
            Assert.AreEqual("2026-10-19", Leida.Fecha);
            Assert.AreEqual(3, Leida.Cambiados);
            Assert.AreEqual(1, Leida.Nuevos);
            Assert.IsTrue(Leida.Oficial);
            Assert.AreEqual("c:/x.tsv", Leida.Archivo);
        }

        [TestMethod]
        public void UnaFilaGuardadaRotaNoSeLee()
        {
            Assert.IsNull(NovedadOficial.Leer("Biotech|fecha", "x", "y"));
            Assert.IsNull(NovedadOficial.Leer(null, "x", "y"));
        }

        /** Contra el .tar real, si el juego esta en esta maquina. En la CI no esta y sale omitida. */
        [TestMethod]
        public void ElTarDelJuegoSeLeeEntero()
        {
            string Tar = @"D:\SteamLibrary\steamapps\common\RimWorld\Data\Core\Languages\" + TraduccionOficial.Idioma + ".tar";
            if (!File.Exists(Tar))
                Assert.Inconclusive("RimWorld no esta instalado en D:\\SteamLibrary.");

            List<EntradaTar> Entradas;
            using (var Archivo = File.OpenRead(Tar))
                Entradas = LectorTar.Leer(Archivo).ToList();

            var Textos = new Dictionary<string, TextoOficial>();
            foreach (EntradaTar Entrada in Entradas)
            {
                string Prefijo = TraduccionOficial.PrefijoDe(Entrada.Nombre);
                if (Prefijo != null)
                    foreach (var Par in TraduccionOficial.LeerXml(Encoding.UTF8.GetString(Entrada.Datos), Prefijo))
                        Textos[Par.Key] = Par.Value;
            }

            Assert.IsTrue(Entradas.Count > 500, "Entradas: " + Entradas.Count);
            Assert.IsTrue(Entradas.All(x => x.Nombre.Length > 0 && x.Fecha.Year >= 2024));
            Assert.IsTrue(Textos.Count > 10000, "Textos: " + Textos.Count);
            Assert.IsTrue(Textos.ContainsKey("DefInjected/ThingDef/Beer.label"));
            Assert.IsTrue(Textos.Values.Count(x => x.Ingles != "") > Textos.Count / 2, "Sin ingles: la mayoria deberia traerlo");
        }

        private static byte[] ArmarTar(params (string Nombre, string Contenido, DateTime Fecha)[] Archivos)
        {
            var Salida = new MemoryStream();
            using (var Tar = new TarWriter(Salida, TarEntryFormat.Gnu, leaveOpen: true))
            {
                foreach (var Archivo in Archivos)
                {
                    var Entrada = new GnuTarEntry(TarEntryType.RegularFile, Archivo.Nombre)
                    {
                        ModificationTime = new DateTimeOffset(Archivo.Fecha),
                        DataStream = new MemoryStream(Encoding.UTF8.GetBytes(Archivo.Contenido)),
                    };
                    Tar.WriteEntry(Entrada);
                }
            }
            return Salida.ToArray();
        }
    }
}
