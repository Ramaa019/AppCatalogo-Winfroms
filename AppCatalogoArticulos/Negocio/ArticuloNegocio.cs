using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Referencia Librerias
using Dominio;
using DB;

using System.Diagnostics.Eventing.Reader;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

using System.Configuration;

namespace Negocio
{
    public class ArticuloNegocio
    {

        private AccesoDatos datos = new AccesoDatos();

        public List<Articulo> listar()
        {
            List<Articulo> listaArticulos = new List<Articulo>();
            // AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setConsulta("Select A.Id, Codigo, Nombre, A.Descripcion, M.id IdMarca, M.Descripcion Marca, C.Id IdCategoria, C.Descripcion Categoria, ImagenUrl, Precio From ARTICULOS A, CATEGORIAS C, MARCAS M Where M.Id = A.IdMarca And C.Id = A.IdCategoria");
                datos.ejecutarLectura();

                while(datos.Lector.Read())
                {
                    Articulo articulo = new Articulo();

                    articulo.Id = (int)datos.Lector["Id"];
                    articulo.Codigo = (string)datos.Lector["Codigo"];
                    articulo.Nombre = (string)datos.Lector["Nombre"];
                    articulo.Descripcion = (string)datos.Lector["Descripcion"];

                    articulo.Precio = (Decimal)datos.Lector["Precio"];

                    // Validar que la columna de la BD no sea null
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        articulo.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                    articulo.Marca = new Marca();
                    articulo.Marca.Id = (int)datos.Lector["IdMarca"];
                    articulo.Marca.Descripcion = (string)datos.Lector["Marca"];

                    articulo.Categoria = new Categoria();
                    articulo.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    articulo.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    listaArticulos.Add(articulo);
                }

                return listaArticulos;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    
        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> articulos = new List<Articulo>();

            try
            {
                string query = "SELECT A.Id, Codigo, Nombre, A.Descripcion, M.id IdMarca, M.Descripcion Marca, C.Id IdCategoria, C.Descripcion Categoria, ImagenUrl, Precio FROM ARTICULOS A, CATEGORIAS C, MARCAS M WHERE M.Id = A.IdMarca AND C.Id = A.IdCategoria AND ";

                switch (campo)
                {
                    case "Codigo":
                        switch (criterio)
                        {
                            case "Comienza con":
                                query += "A.Codigo like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                query += "A.Codigo like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                query += "A.Codigo like '%" + filtro + "%'";
                                break;
                        }
                        break;
                    case "Nombre":
                        switch (criterio)
                        {
                            case "Comienza con":
                                query += "A.Nombre like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                query += "A.Nombre like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                query += "A.Nombre like '%" + filtro + "%'";
                                break;
                        }
                        break;
                    case "Marca":
                        switch (criterio)
                        {
                            case "Comienza con":
                                query += "M.Descripcion like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                query += "M.Descripcion like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                query += "M.Descripcion like '%" + filtro + "%'";
                                break;
                        }
                        break;
                    case "Categoria":
                        switch (criterio)
                        {
                            case "Comienza con":
                                query += "C.Descripcion like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                query += "C.Descripcion like '%" + filtro + "'";
                                break;
                            case "Contiene":
                                query += "C.Descripcion like '%" + filtro + "%'";
                                break;
                        }
                        break;
                    case "Precio":
                        switch (criterio)
                        {
                            case "Mayor a":
                                query += "A.Precio > " + filtro;
                                break;
                            case "Menor a":
                                query += "A.Precio < " + filtro;
                                break;
                            case "Igual a":
                                query += "A.Precio = " + filtro;
                                break;
                        }
                        break;
                    default:
                        break;
                }

                datos.setConsulta(query);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo articulo = new Articulo();

                    articulo.Id = (int)datos.Lector["Id"];
                    articulo.Codigo = (string)datos.Lector["Codigo"];
                    articulo.Nombre = (string)datos.Lector["Nombre"];
                    articulo.Descripcion = (string)datos.Lector["Descripcion"];

                    articulo.Precio = (Decimal)datos.Lector["Precio"];

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        articulo.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                    articulo.Marca = new Marca();
                    articulo.Marca.Id = (int)datos.Lector["IdMarca"];
                    articulo.Marca.Descripcion = (string)datos.Lector["Marca"];

                    articulo.Categoria = new Categoria();
                    articulo.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    articulo.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    articulos.Add(articulo);
                }

                return articulos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void guardar(Articulo articulo)
        {
            try
            {
                datos.setConsulta("Insert Into ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio) Values (@codigo, @nombre, @descripcion, @idMarca, @idCategoria, @urlImg, @precio)");
                datos.setParametros("@codigo", articulo.Codigo);
                datos.setParametros("@nombre", articulo.Nombre);
                datos.setParametros("@descripcion", articulo.Descripcion);
                datos.setParametros("@idMarca", articulo.Marca.Id);
                datos.setParametros("@idCategoria", articulo.Categoria.Id);
                datos.setParametros("@urlImg", articulo.ImagenUrl);
                datos.setParametros("@precio", articulo.Precio);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo articulo)
        {
            try
            {
                datos.setConsulta("Update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, ImagenUrl = @urlImg, Precio = @precio Where Id = @id");
                
                datos.setParametros("@codigo", articulo.Codigo);
                datos.setParametros("@nombre", articulo.Nombre);
                datos.setParametros("@descripcion", articulo.Descripcion);
                datos.setParametros("@idMarca", articulo.Marca.Id);
                datos.setParametros("@idCategoria", articulo.Categoria.Id);
                datos.setParametros("@urlImg", articulo.ImagenUrl);
                datos.setParametros("@precio", articulo.Precio);

                datos.setParametros("@id", articulo.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminar(Articulo seleccionado)
        {
            try
            {
                datos.setConsulta("Delete From ARTICULOS Where Id = @id");
                datos.setParametros("@id", seleccionado.Id);

                string imagen = seleccionado.ImagenUrl;
                if (!(imagen.ToUpper().Contains("HTTP")) && File.Exists(ConfigurationManager.AppSettings["ImgArticulos"] + "\\" + imagen))
                {
                    File.Delete(ConfigurationManager.AppSettings["ImgArticulos"] + "\\" + imagen);
                }

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
