using Microsoft.Data.SqlClient;
using MvcCoreLinqToSql.Models;
using System.Data;
using System.Security.Cryptography;

namespace MvcCoreLinqToSql.Repositories
{
    public class RepositoryEmpleados
    {
        //SOLO TENDREMOS UNA TABLA A NIVEL DE CLASE PARA 
        //NUESTRAS CONSULTAS
        private DataTable tablaEmpleados;

        public RepositoryEmpleados()
        {
            string connectionString = @"Data Source=LOCALHOST\DEVELOPER;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Encrypt=True;Trust Server Certificate=True";
            string sql = "select * from EMP";
            //CREAMOS EL ADAPTADOR PUENTE ENTRE SQL SERVER Y LINQ
            SqlDataAdapter ad = new SqlDataAdapter(sql, connectionString);
            this.tablaEmpleados = new DataTable();
            //TRAEMOS LOS DATOS PARA LINQ
            ad.Fill(this.tablaEmpleados);
        }

        //METODO PARA RECUPERAR TODOS LOS EMPLEADOS
        public List<Empleado> GetEmpleados()
        {
            //LAS CONSULTAS SE ALMACENAN EN GENERICOS (var)
            var consulta =
                from datos in this.tablaEmpleados.AsEnumerable()
                select datos;
            //AHORA MISMO TENEMOS DENTRO DE CONSULTA LA INFORMACION
            //DE LOS EMPLEADOS
            //LOS DATOS VIENEN EN FORMATO TABLA, CADA ELEMENTO
            //DE UNA TABLA ES UNA FILA.  (DataRow)
            //DEBEMOS RECORRER LAS FILAS, EXTRAERLAS Y CONVERTIRLAS 
            //A NUESTRO MODEL Empleado
            List<Empleado> empleados = new List<Empleado>();
            //RECORREMOS CADA FILA DE LA CONSULTAS
            foreach (var row in consulta)
            {
                //PARA EXTRAR DATOS DE UN DataRow
                //DataRow.Field<tipodato>("COLUMNA")
                Empleado emp = new Empleado();
                emp.IdEmpleado = row.Field<int>("EMP_NO");
                emp.Apellido = row.Field<string>("APELLIDO");
                emp.Oficio = row.Field<string>("OFICIO");
                emp.Salario = row.Field<int>("SALARIO");
                emp.IdDepartamento = row.Field<int>("DEPT_NO");
                empleados.Add(emp);
            }
            return empleados;
        }

        public Empleado FindEmpleado(int idEmpleado)
        {
            //FILTRAMOS NUESTRA CONSULTA
            var consulta =
                from datos in this.tablaEmpleados.AsEnumerable()
                where datos.Field<int>("EMP_NO") == idEmpleado
                select datos;
            //NOSOTROS SABEMOS QUE ESTA CONSULTA DEVUELVE UNA 
            //FILA, PERO Linq SIEMPRE DEVUELVE UN CONJUNTO
            //DENTRO DE ESTE CONJUNTO TENEMOS METODOS LAMBDA 
            //PARA HACER COSITAS
            //POR EJEMPLO, PODRIAMOS CONTAR, PODRIAMOS SABER EL MAXIMO
            //O RECUPERAR EL PRIMER ELEMENTO DEL CONJUNTO
            var row = consulta.First();
            Empleado empleado = new Empleado();
            empleado.IdEmpleado = row.Field<int>("EMP_NO");
            empleado.Apellido = row.Field<string>("APELLIDO");
            empleado.Oficio = row.Field<string>("OFICIO");
            empleado.Salario = row.Field<int>("SALARIO");
            empleado.IdDepartamento = row.Field<int>("DEPT_NO");
            return empleado;
        }

        public List<Empleado> GetEmpleadosOficioSalario
            (string oficio, int salario)
        {
            var consulta = from datos in
                               this.tablaEmpleados.AsEnumerable()
                           where datos.Field<string>("OFICIO") == oficio
                           && datos.Field<int>("SALARIO") >= salario
                           select datos;
            if (consulta.Count() == 0)
            {
                //SIEMPRE DEVOLVEMOS NULL
                return null;
            }
            else
            {
                List<Empleado> empleados = new List<Empleado>();
                foreach (var row in consulta)
                {
                    Empleado emp = new Empleado
                    {
                        IdEmpleado = row.Field<int>("EMP_NO"),
                        Apellido = row.Field<string>("APELLIDO"),
                        Oficio = row.Field<string>("OFICIO"),
                        Salario = row.Field<int>("SALARIO"),
                        IdDepartamento = row.Field<int>("DEPT_NO")
                    };
                    empleados.Add(emp);
                }
                return empleados;
            }
        }

        public ResumenEmpleados GetEmpleadosOficio(string oficio)
        {
            var consulta = from datos in
                               this.tablaEmpleados.AsEnumerable()
                           where datos.Field<string>("OFICIO")
                           == oficio
                           orderby datos.Field<string>("OFICIO")
                           select datos;
            //SI NO EXISTEN REGISTROS, DEBEMOS CONTROLARLO
            if (consulta.Count() == 0)
            {
                //VALORES NEUTROS
                ResumenEmpleados model = new ResumenEmpleados();
                model.Personas = 0;
                model.MaximoSalario = 0;
                model.MediaSalarial = 0;
                model.Empleados = null;
                return model;
            }
            else
            {
                //QUIERO ORDENAR EMPLEADOS POR SU SALARIO
                consulta = consulta.OrderBy(z => z.Field<int>("SALARIO"));
                int personas = consulta.Count();
                int maximo = consulta.Max(x => x.Field<int>("SALARIO"));
                double media = consulta.Average(x => x.Field<int>("SALARIO"));
                List<Empleado> empleados = new List<Empleado>();
                foreach (var row in consulta)
                {
                    Empleado emp = new Empleado
                    {
                        IdEmpleado = row.Field<int>("EMP_NO"),
                        Apellido = row.Field<string>("APELLIDO"),
                        Oficio = row.Field<string>("OFICIO"),
                        Salario = row.Field<int>("SALARIO"),
                        IdDepartamento = row.Field<int>("DEPT_NO")
                    };
                    empleados.Add(emp);
                }
                ResumenEmpleados model = new ResumenEmpleados();
                model.Personas = personas;
                model.MaximoSalario = maximo;
                model.MediaSalarial = media;
                model.Empleados = empleados;
                return model;
            }
        }

        public List<string> GetOficios()
        {
            var consulta = (from datos in
                               this.tablaEmpleados.AsEnumerable()
                           select datos.Field<string>("OFICIO")).Distinct();
            //AHORA MISMO YA TENEMOS LO QUE NECESITAMOS, UN CONJUNTO DE 
            //STRING.
            //LA NORMA SUELE SER DEVOLVER LA COLECCION GENERICA List<T>
            return consulta.ToList();
        }
    }
}
