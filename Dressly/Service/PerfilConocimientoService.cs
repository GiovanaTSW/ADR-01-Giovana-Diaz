using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public class PerfilConocimientoService : IPerfilConocimientoService
{
    private static readonly Dictionary<string, TipoCuerpoInfo> Cuerpos = new()
    {
        ["Reloj de arena"] = new()
        {
            Nombre = "Reloj de arena",
            Descripcion = "Hombros y caderas del mismo ancho con una cintura marcada y definida. Es considerada una silueta equilibrada naturalmente.",
            Caracteristicas = new()
            {
                "Hombros alineados con el ancho de las caderas",
                "Cintura visiblemente más estrecha",
                "Curvas proporcionadas en la parte superior e inferior",
                "Distribución equilibrada del peso corporal"
            },
            PrendasRecomendadas = new()
            {
                "Prendas entalladas que marquen la cintura",
                "Cinturones para resaltar la silueta",
                "Vestidos corte princesa o línea A",
                "Faldas lápiz que sigan la curva natural",
                "Blazers ajustados con pinzas"
            },
            PrendasEvitar = new()
            {
                "Ropa demasiado holgada que oculte la silueta",
                "Prendas sin forma que no marquen la cintura",
                "Capas voluminosas sin estructura"
            }
        },
        ["Triángulo"] = new()
        {
            Nombre = "Triángulo (Pera)",
            Descripcion = "Caderas más anchas que los hombros, con la cintura marcada. El peso se concentra en la parte inferior del cuerpo.",
            Caracteristicas = new()
            {
                "Caderas visiblemente más anchas que los hombros",
                "Cintura definida",
                "Piernas y glúteos con mayor volumen",
                "Parte superior del cuerpo más delgada"
            },
            PrendasRecomendadas = new()
            {
                "Prendas que agreguen volumen en la parte superior (volantes, hombreras)",
                "Colores claros o estampados en la parte superior",
                "Colores oscuros en la parte inferior",
                "Escotes amplios (barco, V, hombro descubierto)",
                "Faldas línea A y pantalones rectos"
            },
            PrendasEvitar = new()
            {
                "Prendas ajustadas en la cadera y glúteos",
                "Bolsillos laterales o detalles que sumen volumen en cadera",
                "Faldas lápiz muy ajustadas",
                "Pantalones clareados o con estampados en la parte inferior"
            }
        },
        ["Triángulo invertido"] = new()
        {
            Nombre = "Triángulo invertido",
            Descripcion = "Hombros más anchos que las caderas, con una silueta que se estrecha hacia abajo. La parte superior del cuerpo es la más prominente.",
            Caracteristicas = new()
            {
                "Hombros más anchos que las caderas",
                "Poco volumen en la cadera y glúteos",
                "Piernas delgadas",
                "Espalda y pecho con mayor presencia"
            },
            PrendasRecomendadas = new()
            {
                "Escotes en V y corazón para suavizar los hombros",
                "Volumen en la parte inferior (faldas con vuelo, pantalones palazzo)",
                "Colores oscuros en la parte superior, claros en la inferior",
                "Faldas plisadas o con detalles en la cadera",
                "Pantalones con bolsillos o detalles laterales"
            },
            PrendasEvitar = new()
            {
                "Hombros con hombreras o mangas voluminosas",
                "Escotes bardot o rectos sin soporte",
                "Estampados llamativos en la parte superior",
                "Prendas muy ajustadas en la parte inferior"
            }
        },
        ["Rectángulo"] = new()
        {
            Nombre = "Rectángulo",
            Descripcion = "Hombros y caderas del mismo ancho con poca definición de cintura. La silueta es recta y equilibrada.",
            Caracteristicas = new()
            {
                "Hombros y caderas alineados en el mismo ancho",
                "Cintura poco definida",
                "Silueta recta y atlética",
                "Distribución pareja del peso corporal"
            },
            PrendasRecomendadas = new()
            {
                "Cinturones para crear ilusión de cintura",
                "Prendas con peplum o capas que agreguen volumen donde se desee",
                "Cortes asimétricos que rompan la línea recta",
                "Vestidos con pinzas o fruncidos en la cintura",
                "Chaquetas con estructura que definan la silueta"
            },
            PrendasEvitar = new()
            {
                "Ropa demasiado recta sin ningún tipo de definición",
                "Prendas completamente holgadas de arriba a abajo",
                "Faldas rectas sin ningún detalle"
            }
        },
        ["Manzana"] = new()
        {
            Nombre = "Manzana (Ovalada)",
            Descripcion = "Volumen concentrado en la parte media del torso, con piernas y brazos delgados. La cintura es la parte más ancha del cuerpo.",
            Caracteristicas = new()
            {
                "Volumen en la zona media del torso",
                "Piernas y brazos delgados",
                "Cintura poco definida o ancha",
                "Pecho y espalda con volumen"
            },
            PrendasRecomendadas = new()
            {
                "Escotes en V que estilicen el torso",
                "Prendas con cintura imperio que fluyan desde el busto",
                "Telas fluidas y ligeras que no se marquen",
                "Chaquetas abiertas que creen líneas verticales",
                "Pantalones rectos que equilibren la silueta"
            },
            PrendasEvitar = new()
            {
                "Ropa ajustada en la cintura",
                "Cinturones anchos que marquen el área media",
                "Telas rígidas o muy pegadas al cuerpo",
                "Prendas con detalles horizontales en el torso"
            }
        }
    };

    private static readonly Dictionary<string, ColorimetriaInfo> Estaciones = new()
    {
        ["Primavera"] = new()
        {
            Nombre = "Primavera",
            Descripcion = "Paleta de colores cálidos y luminosos que evocan la frescura y vitalidad de la primavera.",
            Caracteristicas = new()
            {
                "Piel con subtono cálido (dorado o amarillento)",
                "Cabello con reflejos dorados, cobrizos o rubios",
                "Ojos en tonos cálidos (avellana, verde claro, azul con destellos dorados)",
                "La piel reacciona bien al sol, se broncea fácilmente"
            },
            Explicacion = "Los colores cálidos y brillantes de la paleta de Primavera armonizan con los subtonos dorados de tu piel porque comparten la misma base cromática. El coral, melocotón y dorado reflejan la luz de manera similar a tu tono natural, creando una apariencia radiante y saludable. Los colores demasiado fríos o apagados pueden hacer que tu piel luzca cansada o grisácea al contrastar con tu calidez natural.",
            ColoresPrincipales = new()
            {
                ["coral"] = "#FF7043",
                ["melocotón"] = "#FFAB91",
                ["beige"] = "#F5F5DC",
                ["dorado"] = "#FFC107",
                ["verde claro"] = "#A5D6A7"
            },
            ColoresComplementarios = new()
            {
                ["amarillo sol"] = "#FFF176",
                ["naranja claro"] = "#FFCC80",
                ["turquesa claro"] = "#80DEEA",
                ["salmón"] = "#FF8A65",
                ["lavanda cálida"] = "#E1BEE7"
            },
            ColoresNeutros = new()
            {
                ["marfil"] = "#FFF8E1",
                ["crema"] = "#FFFDE7",
                ["camel"] = "#C2A878",
                ["topo claro"] = "#BCAAA4",
                ["arena"] = "#D7CCC8"
            }
        },
        ["Verano"] = new()
        {
            Nombre = "Verano",
            Descripcion = "Paleta de colores fríos y suaves que reflejan la serenidad y elegancia del verano.",
            Caracteristicas = new()
            {
                "Piel con subtono frío (rosado o azulado)",
                "Cabello en tonos fríos (ceniza, rubio ceniza, castaño claro)",
                "Ojos en tonos fríos (azul grisáceo, verde grisáceo, avellana frío)",
                "La piel se quema fácilmente y broncea con dificultad"
            },
            Explicacion = "Los colores fríos y suaves de la paleta de Verano se alinean con los subtonos rosados de tu piel porque comparten matices azulados. La lavanda, el azul grisáceo y el rosa palo reflejan la luz de forma armoniosa con tu tono natural, proyectando una imagen fresca y sofisticada. Los colores cálidos intensos pueden sobresaturar tu rostro y descompensar el equilibrio cromático natural.",
            ColoresPrincipales = new()
            {
                ["lavanda"] = "#CE93D8",
                ["azul grisáceo"] = "#90A4AE",
                ["rosa palo"] = "#F8BBD0",
                ["gris perla"] = "#ECEFF1",
                ["blanco roto"] = "#FAFAFA"
            },
            ColoresComplementarios = new()
            {
                ["azul cielo"] = "#81D4FA",
                ["menta"] = "#A5D6A7",
                ["lila"] = "#E1BEE7",
                ["rosa chicle"] = "#F48FB1",
                ["gris azulado"] = "#78909C"
            },
            ColoresNeutros = new()
            {
                ["beige frío"] = "#E0E0E0",
                ["gris medio"] = "#BDBDBD",
                ["azul pizarra"] = "#607D8B",
                ["marfil frío"] = "#F5F5F5",
                ["plata"] = "#CFD8DC"
            }
        },
        ["Otoño"] = new()
        {
            Nombre = "Otoño",
            Descripcion = "Paleta de colores cálidos y terrosos que capturan la riqueza y profundidad del otoño.",
            Caracteristicas = new()
            {
                "Piel con subtono cálido profundo (dorado, amarillo o rojizo)",
                "Cabello en tonos cálidos intensos (castaño rojizo, cobrizo, caoba)",
                "Ojos en tonos cálidos profundos (marrón, ámbar, verde oliva)",
                "La piel se broncea con facilidad y rara vez se quema"
            },
            Explicacion = "Los colores terrosos y profundos de la paleta de Otoño resuenan con los subtonos cálidos intensos de tu piel porque ambos comparten pigmentación dorada-rojiza. La terracota, mostaza y verde oliva complementan tu coloración natural al reflejar los mismos pigmentos que tu piel y cabello producen de forma orgánica. Los colores pastel o demasiado fríos pueden hacer que tu rostro pierda luminosidad.",
            ColoresPrincipales = new()
            {
                ["terracota"] = "#BF360C",
                ["mostaza"] = "#F9A825",
                ["verde oliva"] = "#827717",
                ["marrón"] = "#795548",
                ["naranja"] = "#E64A19"
            },
            ColoresComplementarios = new()
            {
                ["borgoña"] = "#880E4F",
                ["verde bosque"] = "#2E7D32",
                ["dorado intenso"] = "#FF8F00",
                ["berenjena"] = "#4A148C",
                ["ladrillo"] = "#D84315"
            },
            ColoresNeutros = new()
            {
                ["beige cálido"] = "#F5F5DC",
                ["marrón claro"] = "#A1887F",
                ["topo"] = "#8D6E63",
                ["crema"] = "#FFF3E0",
                ["camel oscuro"] = "#A1887F"
            }
        },
        ["Invierno"] = new()
        {
            Nombre = "Invierno",
            Descripcion = "Paleta de colores intensos y fríos que transmiten la fuerza y nitidez del invierno.",
            Caracteristicas = new()
            {
                "Piel con subtono frío intenso (azul, rosado intenso o neutro)",
                "Cabello en tonos oscuros y fríos (negro, castaño oscuro ceniza)",
                "Ojos en tonos intensos (negro, marrón oscuro, azul intenso, verde esmeralda)",
                "Alto contraste natural entre el color de piel, cabello y ojos"
            },
            Explicacion = "Los colores intensos y puros de la paleta de Invierno potencian el alto contraste natural de tus rasgos porque crean una sinergia cromática audaz y definida. El negro, blanco puro, rojo intenso y azul marino reflejan la luz de manera limpia y directa, igual que tu coloración natural. Los colores apagados o demasiado suaves pueden diluir tu presencia visual y hacer que tus rasgos pierdan impacto.",
            ColoresPrincipales = new()
            {
                ["negro"] = "#212121",
                ["blanco"] = "#FAFAFA",
                ["rojo"] = "#B71C1C",
                ["azul marino"] = "#0D47A1",
                ["fucsia"] = "#AD1457"
            },
            ColoresComplementarios = new()
            {
                ["plata"] = "#BDBDBD",
                ["esmeralda"] = "#1B5E20",
                ["púrpura"] = "#6A1B9A",
                ["azul eléctrico"] = "#1565C0",
                ["cereza"] = "#C62828"
            },
            ColoresNeutros = new()
            {
                ["gris oscuro"] = "#424242",
                ["blanco hielo"] = "#ECEFF1",
                ["azul medianoche"] = "#1A237E",
                ["carbón"] = "#37474F",
                ["marfil"] = "#F5F5F5"
            }
        }
    };

    private static readonly Dictionary<string, ContrasteInfo> Contrastes = new()
    {
        ["Bajo"] = new()
        {
            Nombre = "Bajo",
            Descripcion = "Tus rasgos faciales tienen poca diferencia de tono entre sí. Piel, cabello y ojos se encuentran en un rango cromático similar.",
            Explicacion = "El contraste bajo significa que no hay una gran diferencia entre el color de tu piel, cabello y ojos. Por ejemplo, piel clara con cabello rubio claro y ojos azules, o piel morena con cabello castaño claro. Este tipo de contraste se beneficia de combinaciones suaves y armoniosas donde los colores fluyan naturalmente entre sí sin saltos bruscos.",
            Recomendaciones = new()
            {
                "Looks monocromáticos que estilicen la figura",
                "Degradados y combinaciones tono sobre tono",
                "Texturas y tejidos que aporten profundidad sin color",
                "Colores suaves y empolvados que mantengan la armonía",
                "Evitar contrastes muy fuertes que compitan con tus rasgos"
            }
        },
        ["Medio"] = new()
        {
            Nombre = "Medio",
            Descripcion = "Existe un contraste moderado entre tus rasgos. Hay diferencia notable pero no extrema entre piel, cabello y ojos.",
            Explicacion = "El contraste medio es el más equilibrado: tus rasgos tienen suficiente diferencia para soportar combinaciones variadas sin abrumar tu rostro. Por ejemplo, piel media con cabello castaño y ojos marrones, o piel clara con cabello castaño oscuro. Puedes jugar tanto con combinaciones suaves como con contrastes moderados.",
            Recomendaciones = new()
            {
                "Combinaciones equilibradas de 2 a 3 colores",
                "Mezcla de tonos medios sin llegar a extremos",
                "Tanto looks monocromáticos como con contraste moderado",
                "Estampados que combinen colores de tu paleta",
                "Prendas en bloque de color bien combinadas"
            }
        },
        ["Alto"] = new()
        {
            Nombre = "Alto",
            Descripcion = "Tus rasgos presentan un marcado contraste entre el color de tu piel, cabello y ojos.",
            Explicacion = "El contraste alto es característico de personas donde la diferencia entre sus rasgos es muy notoria. Por ejemplo, piel muy clara con cabello negro y ojos oscuros, o piel oscura con cabello muy claro. Este tipo de contraste puede soportar y lucir combinaciones audaces sin que los colores compitan con el rostro, sino que se potencian mutuamente.",
            Recomendaciones = new()
            {
                "Combinaciones de alto contraste como blanco con negro",
                "Colores puros y saturados de tu paleta personal",
                "Bloques de color definidos sin miedo",
                "Accesorios llamativos que complementen el look",
                "Prendas monocromáticas con un acento de color fuerte"
            }
        }
    };

    private static readonly HashSet<string> OjosCalidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "Marrón", "Avellana", "Verde", "Verde oliva", "Verde claro"
    };

    private static readonly HashSet<string> OjosFrios = new(StringComparer.OrdinalIgnoreCase)
    {
        "Azul", "Azul intenso", "Azul grisáceo", "Gris", "Verde grisáceo"
    };

    private static readonly HashSet<string> OjosIntensos = new(StringComparer.OrdinalIgnoreCase)
    {
        "Negro", "Marrón oscuro", "Azul intenso", "Verde esmeralda"
    };

    public TipoCuerpoInfo? ObtenerInfoTipoCuerpo(string? tipoCuerpo)
    {
        if (string.IsNullOrEmpty(tipoCuerpo)) return null;
        return Cuerpos.TryGetValue(tipoCuerpo, out var info) ? info : null;
    }

    public ColorimetriaInfo? ObtenerInfoColorimetria(string? colorimetria)
    {
        if (string.IsNullOrEmpty(colorimetria)) return null;
        return Estaciones.TryGetValue(colorimetria, out var info) ? info : null;
    }

    public ContrasteInfo? ObtenerInfoContraste(string? contraste)
    {
        if (string.IsNullOrEmpty(contraste)) return null;
        return Contrastes.TryGetValue(contraste, out var info) ? info : null;
    }

    public string? DetectarEstacion(string? subtonoPiel, string? intensidadCabello, string? colorOjos)
    {
        if (string.IsNullOrEmpty(subtonoPiel) || string.IsNullOrEmpty(intensidadCabello))
            return null;

        var sub = subtonoPiel;
        var cab = intensidadCabello;
        var ojos = colorOjos ?? "";

        switch (sub)
        {
            case "Cálido":
                if (cab == "Claro") return "Primavera";
                if (cab == "Medio")
                {
                    if (OjosFrios.Contains(ojos)) return "Primavera";
                    return "Otoño";
                }
                if (cab == "Oscuro") return "Otoño";
                break;

            case "Frío":
                if (cab == "Claro") return "Verano";
                if (cab == "Medio")
                {
                    if (OjosIntensos.Contains(ojos)) return "Invierno";
                    return "Verano";
                }
                if (cab == "Oscuro") return "Invierno";
                break;

            case "Neutro":
                if (cab == "Claro") return "Primavera";
                if (cab == "Medio") return "Verano";
                if (cab == "Oscuro") return "Otoño";
                break;
        }

        return null;
    }
}
