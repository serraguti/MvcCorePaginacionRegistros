using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

#region VISTAS SQL SERVER
//create view V_DEPT_INDIVIDUAL
//AS
//	select CAST(
//	row_number() over(order by dept_no) as int)

//    as posicion, isnull(dept_no, 0) as dept_no
//	, dnombre, loc from dept
//go
#endregion

#region STORED PROCEDURES
//create procedure SP_PAGINARGRUPO_DEPARTAMENTOS
//(@POSICION INT)
//AS
//    select dept_no, dnombre, loc from V_DEPT_INDIVIDUAL
//	where posicion >= @POSICION and posicion < (@POSICION + 2)
//GO
//ALTER procedure SP_PAGINARGRUPO_DEPARTAMENTOS
//(@POSICION INT, @registros int out)
//AS
//    --UN PROCEDIMIENTO DE PAGINACION SIEMPRE
//	--DEBE DEVOLVER EL NUMERO DE REGISTROS TOTALES
//	select @registros = count(dept_no) from V_DEPT_INDIVIDUAL

//    select dept_no, dnombre, loc from V_DEPT_INDIVIDUAL
//	where posicion >= @POSICION and posicion < (@POSICION + 2)
//GO
#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }
        
        public List<Departamento> 
            GetGrupoDepartamentos(int posicion
            , ref int numeroregistros)
        {
            string sql = "SP_PAGINARGRUPO_DEPARTAMENTOS @POSICION "
                + ", @registros OUT";
            SqlParameter paramposicion =
                new SqlParameter("@POSICION", posicion);
            SqlParameter paramregistros =
                new SqlParameter("@registros", -1);
            paramregistros.Direction = ParameterDirection.Output;
            var consulta =
                this.context.Departamentos.FromSqlRaw
                (sql, paramposicion, paramregistros);
            List<Departamento> departamentos = consulta.ToList();
            numeroregistros = (int)paramregistros.Value;
            //COMO ENVIAMOS EL NUMERO DE REGISTROS AL CONTROLLER????
            return departamentos;
        }

        public int GetNumeroRegistros()
        {
            return this.context.VistaDepartamentos.Count();
        }

        public VistaDepartamento GetVistaDepartamento(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion == posicion
                           select datos;
            return consulta.FirstOrDefault();
        }

        public List<VistaDepartamento> 
            GetGrupoVistaDepartamento(int posicion)
        {
            //select* from V_DEPT_INDIVIDUAL
            //WHERE POSICION >= 1 and posicion < (posicion + 2)
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion + 2)
                           select datos;
            return consulta.ToList();
        }
    }
}
