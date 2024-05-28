using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCatalogo
{
    public static class Helpers
    {

        public static void ocultarColumnas(DataGridView dgv, string[] columnas)
        {
            foreach(string columna in columnas)
            {
                dgv.Columns[columna].Visible = false;
            }
        }


        public static bool soloDecimal(string cadena)
        {
            foreach (char c in cadena)
            {
                if (!(char.IsDigit(c) || c != '.'))
                    return false;
            }

            return true;
        }

        public static void cargarImagen(PictureBox pbx, string img)
        {
            try
            {
                if (!(img.ToUpper().Contains("HTTP")) && File.Exists(ConfigurationManager.AppSettings["ImgArticulos"] + "\\" + img))
                {
                    pbx.Load(ConfigurationManager.AppSettings["ImgArticulos"] + "\\" + img);
                }
                else
                {
                    pbx.Load(img);
                }
            }
            catch (Exception)
            {
                pbx.Load("https://live.staticflickr.com/65535/53740790123_74f4dc8ba9_m.jpg");
            }
        }

    }
}
