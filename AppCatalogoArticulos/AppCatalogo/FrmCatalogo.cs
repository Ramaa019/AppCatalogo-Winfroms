using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Dominio;
using Negocio;

namespace AppCatalogo
{
    public partial class FrmCatalogo : Form
    {
        public FrmCatalogo()
        {
            InitializeComponent();
        }

        private void FrmCatalogo_Load(object sender, EventArgs e)
        {
            cargarArticulos();

            cbxCampo.Items.Add("Codigo");
            cbxCampo.Items.Add("Nombre");
            cbxCampo.Items.Add("Marca");
            cbxCampo.Items.Add("Categoria");
            cbxCampo.Items.Add("Precio");
        }

        private void cargarArticulos()
        {
            ArticuloNegocio ArtNegocio = new ArticuloNegocio();

            try
            {
                List<Articulo> listaArticulos = ArtNegocio.listar();
                
                if(listaArticulos.Count != 0)
                {
                    dgvArticulos.DataSource = listaArticulos;
                    string[] columnas = { "Id", "ImagenUrl" };
                    Helpers.ocultarColumnas(dgvArticulos, columnas);
                    Helpers.cargarImagen(pbxImgArticulo,listaArticulos[0].ImagenUrl);
                } 
                else
                {
                    dgvArticulos.DataSource = "";
                }

            }   
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                Helpers.cargarImagen(pbxImgArticulo, seleccionado.ImagenUrl);
            } 
            else
            {
                return;
            }
        }

        private void cbxCampo_SelectedIndexChanged(object sender, EventArgs e)
        {

            cbxCriterio.Items.Clear();

            // Por el Reset
            if (cbxCampo.SelectedIndex == -1) return;

            string opcion = cbxCampo.SelectedItem.ToString();

            if (opcion == "Codigo" || opcion == "Nombre" || opcion == "Marca" || opcion == "Categoria")
            {
                cbxCriterio.Items.Add("Comienza con");
                cbxCriterio.Items.Add("Termina con");
                cbxCriterio.Items.Add("Contiene");
            }
            else if (opcion == "Precio")
            {
                cbxCriterio.Items.Add("Mayor a");
                cbxCriterio.Items.Add("Menor a");
                cbxCriterio.Items.Add("Igual a");
            }   
        }

        private bool validarFiltro()
        {
            if (cbxCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un \"Campo\" de busqueda");
                return true;
            }

            if(cbxCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un \"Criterio\" de busqueda");
                return true;
            }

            if(cbxCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtBuscar.Text))
                {
                    MessageBox.Show("Ingrese un número para buscar");
                    return true;
                }

                if (!(Helpers.soloDecimal(txtBuscar.Text)))
                {
                    MessageBox.Show("Se debe ingresar un número para buscar");
                    return true;
                }
            }

            return false;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio ArtNegocio = new ArticuloNegocio();

            try
            {
                if (validarFiltro()) return;

                string campo = cbxCampo.SelectedItem.ToString();
                string criterio = cbxCriterio.SelectedItem.ToString();
                string filtro = txtBuscar.Text;

                dgvArticulos.DataSource = ArtNegocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void btnResetearBusqueda_Click(object sender, EventArgs e)
        {
            List<Articulo> listaVacia = new List<Articulo>();
            dgvArticulos.DataSource = listaVacia;
            cargarArticulos();

            cbxCampo.SelectedIndex = -1;
            cbxCriterio.SelectedIndex = -1;
            txtBuscar.Text = "";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaArticulo nuevoArticulo = new FrmAltaArticulo();
            nuevoArticulo.ShowDialog();

            cargarArticulos();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
                return;

            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
            FrmAltaArticulo modificarArticulo = new FrmAltaArticulo(seleccionado);
            modificarArticulo.ShowDialog();

            cargarArticulos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
                return;

            ArticuloNegocio artNegocio = new ArticuloNegocio();

            try
            {
                DialogResult result = MessageBox.Show("¿Eliminar este Artículo?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
                if (result == DialogResult.Yes)
                {
                    Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    // Reseteo el PictureBox para que no utilice la imagen a eliminar
                    Helpers.cargarImagen(pbxImgArticulo, "");

                    artNegocio.eliminar(seleccionado);
                    cargarArticulos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow == null)
                return;

            Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            string detalle = "";
            detalle += "Código: " + seleccionado.Codigo + "\n";
            detalle += "Nombre: " + seleccionado.Nombre + "\n";
            detalle += "Descripción: " + seleccionado.Descripcion + "\n";
            detalle += "Marca: " + seleccionado.Marca.Descripcion + "\n";
            detalle += "Categoría: " + seleccionado.Categoria.Descripcion + "\n";
            detalle += "Precio: $" + seleccionado.Precio.ToString();

            MessageBox.Show(detalle, "Características del Artículo");
        }
    }
}