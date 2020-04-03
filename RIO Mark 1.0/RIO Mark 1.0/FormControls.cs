using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rioSystem_2._0
{
    public class FormControls
    {

        public int mouseStartX { get; set; }
        public int mouseStartY { get; set; }
        public int formStartX { get; set; }
        public int formStartY { get; set; }

        public TForm getForm<TForm>() where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }
    }
}
