using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RIO
{
    public static class FormControls
    {

        public static int mouseStartX { get; set; }
        public static int mouseStartY { get; set; }
        public static int formStartX { get; set; }
        public static int formStartY { get; set; }

        public static TForm getForm<TForm>() where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }
    }
}
