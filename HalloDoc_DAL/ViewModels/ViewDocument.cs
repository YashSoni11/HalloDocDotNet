using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class ViewDocument
    {
        public int FileId { get; set; }
        public string filename { get; set; }

        public string uploader { get; set; }    

        public DateTime uploadDate { get; set; }

        


    }
}
