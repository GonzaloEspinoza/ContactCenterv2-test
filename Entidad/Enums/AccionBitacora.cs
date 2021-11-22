using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Enums
{
    public enum AccionBitacora
    {
        Ninguna = -1,
        Creacion = 0,
        Modificacion = 1,
        Eliminacion = 2,
        Anulacion = 3,
        InicioSesion = 4,
        CambioPassword = 5,
        Habilitacion = 6,
        Deshabilitacion = 7,
        Obteniendo = 8,
        Vigencia = 9,
        Curso = 10,
        Finalizado = 11
    }
}
