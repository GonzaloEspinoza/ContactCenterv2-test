using DataP;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class LBitacora : LBaseCC<Bitacora>
    {
        public List<Bitacora> Consultar(TipoBitacora TipoBitacora, AccionBitacora AccionBitacora, DateTime Inicio, DateTime Fin, long? UsuarioId)
        {
            using (var esquema = GetEsquema())
            {
                List<Bitacora> Bitacoras = (from b in esquema.Bitacora where b.FechaHora >= Inicio && b.FechaHora <= Fin && (TipoBitacora != TipoBitacora.Ninguno ? b.Tipo == (int)TipoBitacora : true) && (AccionBitacora != AccionBitacora.Ninguna ? b.Accion == AccionBitacora.ToString() : true) && (UsuarioId != null ? b.UsuarioId == UsuarioId : true) select b).ToList();
                foreach (var item in Bitacoras)
                {
                    if (item.UsuarioId != null)
                    {
                        item.Usuario.NickName.Trim();
                    }
                }
                return Bitacoras;
            }
        }
    }
}
