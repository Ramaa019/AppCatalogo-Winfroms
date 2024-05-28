using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCatalogo
{
    public partial class FrmAltaArticulo : Form
    {
        private Articulo articulo = null;

        // Modal para sabir img
        private OpenFileDialog archivo = null;

        public FrmAltaArticulo()
        {
            InitializeComponent();
        }

        public FrmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;

            Text = "Modificar Artículo";
            lblTitulo.Text = "Modificar Artículo";
        }

        private void FrmAltaArticulo_Load(object sender, EventArgs e)
        {
            if(articulo != null)
            {
                txtCodigo.Text = articulo.Codigo;
                txtNombre.Text = articulo.Nombre;
                txtDescripcion.Text = articulo.Descripcion;

                cargarMarcas();
                cbxMarca.SelectedValue = articulo.Marca.Id;

                cargarCategorias();
                cbxCategoria.SelectedValue = articulo.Categoria.Id;

                txtUrlImg.Text = articulo.ImagenUrl;
                Helpers.cargarImagen(pbxImg ,articulo.ImagenUrl);

                txtPrecio.Text = articulo.Precio.ToString();
            }
        }

        private void cargarMarcas()
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();
            try
            {
                cbxMarca.DataSource = marcaNegocio.listar();
                // Establecer relacion [Clave : Valor] - para luego seleccionar o cargar la marca en el formulario de un articulo que se modifica
                cbxMarca.ValueMember = "id";
                cbxMarca.DisplayMember = "descripcion";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cbxMarca_DropDown(object sender, EventArgs e)
        {
            cargarMarcas();
        }

        private void cargarCategorias()
        {
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            try
            {
                cbxCategoria.DataSource = categoriaNegocio.listar();
                // Establecer relacion [Clave : Valor] - para luego seleccionar o cargar la categoria en el formulario de un articulo que se modifica
                cbxCategoria.ValueMember = "id";
                cbxCategoria.DisplayMember = "descripcion";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cbxCategoria_DropDown(object sender, EventArgs e)
        {
            cargarCategorias();
        }

        private void txtUrlImg_Leave(object sender, EventArgs e)
        {
            Helpers.cargarImagen(pbxImg, txtUrlImg.Text);
        }

        private void btnAgregarImg_Click(object sender, EventArgs e)
        {
            // Modal de windows para agregar archivo
            archivo = new OpenFileDialog();

            // Aceptar solo imagenes jpg
            archivo.Filter = "jpg|*.jpg; | png|*.png";

            // Si se subio una imagen
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                // Agregar al textbox y al pictureBox
                txtUrlImg.Text = archivo.FileName;
                Helpers.cargarImagen(pbxImg, archivo.FileName);
            }
        }

        private string guardarImagenLocal()
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
            string destinationPath = Path.Combine(ConfigurationManager.AppSettings["ImgArticulos"], uniqueFileName);

            // Guardar la imagen en la carpeta configurada
            File.Copy(archivo.FileName, destinationPath);

            return uniqueFileName;
        }

        private bool validarInputs()
        {

            // CODIGO
            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                lblCodigo.ForeColor = Color.Red;
                MessageBox.Show("Ingresa el Código del Artículo");
                return true;
            } 
            else
            {
                lblCodigo.ForeColor = Color.Black;
            }

            // NOMBRE
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                lblNombre.ForeColor = Color.Red;
                MessageBox.Show("Ingresa el Nombre del Artículo");
                return true;
            }
            else
            {
                lblNombre.ForeColor = Color.Black;
            }

            // DESCRIPCION
            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                lblDescripcion.ForeColor = Color.Red;
                MessageBox.Show("Ingresa el Nombre del Artículo");
                return true;
            }
            else
            {
                lblDescripcion.ForeColor = Color.Black;
            }

            // MARCA
            if (cbxMarca.SelectedItem == null)
            {
                lblMarca.ForeColor = Color.Red;
                MessageBox.Show("Seleccione la Marca del Artículo");
                return true;
            }
            else
            {
                lblMarca.ForeColor = Color.Black;
            }

            // CATEGORIA
            if (cbxCategoria.SelectedItem == null)
            {
                lblCategoria.ForeColor = Color.Red;
                MessageBox.Show("Seleccione la Categoría del Artículo");
                return true;
            }
            else
            {
                lblCategoria.ForeColor = Color.Black;
            }

            // URL IMAGEN
            if(string.IsNullOrEmpty(txtUrlImg.Text))
            {
                lblUrlImg.ForeColor = Color.Red;
                MessageBox.Show("Ingresa una Imagen del Artículo");
                return true;
            }
            else
            {
                lblUrlImg.ForeColor = Color.Black;
            }

            // PRECIO
            if (string.IsNullOrEmpty(txtPrecio.Text))
            {
                lblPrecio.ForeColor = Color.Red;
                MessageBox.Show("Ingrese el Precio del Articulo");
                return true;
            }
            else
            {
                lblPrecio.ForeColor = Color.Black;
            }

            if (!(Helpers.soloDecimal(txtPrecio.Text)))
            {
                lblPrecio.ForeColor = Color.Red;
                MessageBox.Show("El Precio no es válido");
                return true;
            }
            else
            {
                lblPrecio.ForeColor = Color.Black;
            }

            return false;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio artNegocio = new ArticuloNegocio();

            try
            {
                if (validarInputs())
                    return;

                if(articulo == null)
                    articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;

                articulo.Marca = (Marca)cbxMarca.SelectedItem;
                articulo.Categoria = (Categoria)cbxCategoria.SelectedItem;

                articulo.Precio = Decimal.Parse(txtPrecio.Text);

                if (archivo != null && !(txtUrlImg.Text.ToUpper().Contains("HTTP")))
                {
                    string nombreImagen = guardarImagenLocal();
                    articulo.ImagenUrl = nombreImagen;
                }
                else
                {
                    articulo.ImagenUrl = txtUrlImg.Text;
                }

                if(articulo.Id != 0)
                {
                    artNegocio.modificar(articulo);
                    MessageBox.Show("Artículo actualizado correctamente");
                } 
                else
                {
                    artNegocio.guardar(articulo);
                    MessageBox.Show("Artículo agregado correctamente");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
