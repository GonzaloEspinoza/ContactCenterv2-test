using DataP;
using Entidad;
using Entidad.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Logica
{
    public class LEstadoEstadoEvento : LBaseCC<EstadoEvento>
    {


        public EEstadoEvento Convert(EstadoEvento CP)
        {
            return new EEstadoEvento(CP);
        }
        public List<EEstadoEvento> Convert(List<EstadoEvento> L)
        {
            List<EEstadoEvento> LC = new List<EEstadoEvento>();
            L.ForEach(x => LC.Add(Convert(x)));
            return LC;
        }
        public List<EEstadoEvento> Obtener()
        {
            var Entidad = new List<EEstadoEvento>();
            using (var esquema = GetEsquema())
            {
                var resultado = (from x in esquema.EstadoEvento
                                 select x).ToList();
                Entidad = Convert(resultado);
                return Entidad;
            }
        }

        public int porEstado(int id)
        {


            using (var esquema = GetEsquema())
            {

                int resultado =
                  (from a in esquema.EstadoEvento
                   join b in esquema.Evento on a.nEstadoEventoId equals b.nEstadoEventoId
                   join c in esquema.EventoHilo on b.nEventoId equals c.nEventoId
                   where a.nEstadoEventoId == id
                   select a).Count();


                return resultado;
            }
        }

        public String ListaEstados()
        {


            using (var esquema = GetEsquema())
            {


                string strDatos;
                strDatos = "[['Estados','Tickets'],";
                var resultado = (from x in esquema.EstadoEvento
                                 select x).ToList();
                foreach (var item in resultado)
                {
                    int cantidad = porEstado(unchecked((int)item.nEstadoEventoId));
                    string nombre = item.sNombre;
                    strDatos = strDatos + "[";
                    strDatos = strDatos + "'" + nombre + "'" + "," + cantidad;
                    strDatos = strDatos + "],";
                }
                strDatos = strDatos.Remove(strDatos.Length - 1);
                strDatos = strDatos + "]";
                char oldChar = '\'';
                char newChar = '"';
                string strNew = strDatos.Replace(oldChar, newChar);

                return strNew;
            }
        }


        public EEstadoEvento Obtener(long id)
        {
            using (var esquema = GetEsquema())
            {
                EstadoEvento entidad = ObtenerEstadoEvento(esquema, id);
                if (entidad == null)
                {
                    throw new BussinessException("No se puede encontrar la EstadoEvento seleccionada.");
                }
                var resultado = Convert(entidad);
                return resultado;
            }
        }

        private EstadoEvento ObtenerEstadoEvento(dbFexpoCruzEntities esquema, long? EstadoEventoId)
        {
            EstadoEvento entidad = (from x in esquema.EstadoEvento
                                 where x.nEstadoEventoId == EstadoEventoId 
                                 select x).FirstOrDefault();
            return entidad;
        }
        public List<EEstadoEvento> ObtenerLista()
        {
            using (var esquema = GetEsquema())
            {
                var entidades = (from u in esquema.EstadoEvento
                                 select u);
                var resultado = Convert(entidades.ToList());
                return resultado;
            }
        }

     



        #region api
       
        

        
        #endregion



    }
}
