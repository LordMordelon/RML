# Glosario: Fortified Features Framework

Fortified Features Framework (FFF) es el framework de AobaKuma que usan The Dead Man's Switch y sus
expansiones: muchos de sus textos (alertas, cierres de instalación, oleadas, sitios hostiles) se ven
dentro de esos mods. Por eso sigue los términos de
[GLOSARIO-DMS.md](../!AobaKuma/The%20Dead%20Man's%20Switch%20-%203819799478/GLOSARIO-DMS.md), y
[RML/GLOSARIO.md](../../GLOSARIO.md), la terminología oficial de RimWorld, manda sobre los dos.

Armado el 24-oct-2026 con las 744 entradas que ya estaban traducidas: cada término sale de una
traducción en uso, salvo los de la última tabla, que son nuevos.

## Conceptos del framework

| Inglés | Español | Notas |
|---|---|---|
| mech | **meca** (pl. mecas), masculino | Como en el glosario general y en DMS: «un meca», «este meca», «los mecas». No «mecanoide», que es *mechanoid*. Unificado el 24-oct-2026 |
| mechanoid | **mecanoide** | |
| automatroid | **automatroide** | Como en DMS |
| mech frame | **estructura de meca** | |
| mech modification | **modificación de meca** | También el botón y el preajuste («preajuste de meca») |
| deactivated mech | **meca desactivado** | Las cápsulas `FFF_MechCapsule_*`, por tamaño: pequeño, mediano, grande, enorme, colosal |
| bandwidth | **banda ancha** | Como en DMS |
| hack | **hackear** | «Intentar hackear {0}», «No se puede hackear» |
| facility | **instalación** | |
| lockdown / lockdown controller | **cierre** / **controlador de cierre** | «Cierre de la instalación en {0}» |
| blast gates | **compuertas blindadas** | |
| alert level (de seguridad) | **nivel de alerta** | El de los escáneres de una instalación. No confundir con *readiness* (ver abajo) |
| perimeter scanner | **escáner perimetral** | «Escáner desconectado», «Escáner rearmándose: {0}» |
| countermeasure | **contramedida** | |
| airspace blockade | **bloqueo del espacio aéreo** | |
| precision bombardment | **bombardeo de precisión** | |
| hostile site / reinforcements inbound | **sitio hostil** / **refuerzos en camino** | |
| huntdown / hunted by {0} | **persecución** / **perseguidos por {0}** | Ver «Pendientes» |
| raid wave | **oleada de asalto** | «Oleada de asalto entrante», «Oleada de asalto en camino» |
| recycle | **reciclar** | |
| mounted weapon | **arma montada** | |
| deploy / deployment platform | **desplegar** / **plataforma de despliegue** | «Despliegue automático» |
| drone / drone platform / drone pack | **dron** / **plataforma de drones** / **mochila de drones** | |
| camouflage | **camuflaje** | «Esquema de camuflaje», «Camuflaje militar», «Camuflaje digital» |
| ammo | **munición** | «Cambiar el tipo de munición» |
| shielding device | **dispositivo de blindaje** | |
| shape charge | **carga hueca** | Como en DMS |

## Preparación para el combate de las facciones (24-oct-2026)

Llegó con la actualización del mod del 24-oct-2026: las facciones suben y bajan de nivel según lo que
pasa, declaran guerras y mandan compañías. Ninguno tenía traducción en uso, así que son propuestas.

| Inglés | Español | Notas |
|---|---|---|
| readiness | **preparación para el combate** | Propuesta. No «nivel de alerta», que ya es el de los escáneres de una instalación |
| Level {0} | **Nivel {0}** | |
| {0} on alert / {0} stands down | **{0} en alerta** / **{0} baja la guardia** | Títulos de las cartas |
| war declared / war ended | **Guerra declarada** / **Fin de la guerra** | |
| settlement taken | **Asentamiento tomado** | |
| permanent enemy | **enemigo permanente** | |
| company | **«esta fuerza ({1})»** | `{1}` es el nombre de la compañía, que viene de un def y puede ser de cualquier género: va entre paréntesis para no tener que adivinarlo |
| platoons | **pelotones** | Como «líder de pelotón» en DMS |
| follow-up units | **las unidades que vengan detrás** | |

## Pendientes

Inconsistencias que ya estaban en las traducciones. No se corrigieron el 24-oct-2026 porque cambian
textos que no estaban en el diff; se dejan anotadas para decidirlas con Angel.

- **El género de «meca».** DMS lo usa en masculino («un meca» 23 veces, «los mecas» 15) y FFF lo
  mezcla: 9 en masculino y 7 en femenino, «esta meca» (`FFF_Camouflage.description`,
  `FFF.NeedMoreBandwidth`, `FFF.MechModification.GizmoDesc`) y «la meca» (`FFF_PaintMech`,
  `FFF.HackFailedDesc`), más las cinco cápsulas «meca desactivada». En DMS queda un «las mecas»
  (`ThingDef/go2jb9.xml`).
- **«mech» traducido como «mecanoide»** en 5 entradas: `FFF_EjectDeactivatedMech`,
  `FFF_HackDeactivatedMech`, `FFF.MechReturn` y su descripción, `FFF.Reason.FrameNotSupported` y
  `FFF.Message.AddonBroken`. `FFF.FrameNotSupported`, con el mismo inglés, dice «estructura de meca».
- **EMP:** una entrada dice «PEM» («deshabilitada por PEM»); el resto de RML deja «EMP».
- **huntdown:** FFF dice «persecución» y DMS, «incursión de cacería». Los dos se ven en el mismo juego.
