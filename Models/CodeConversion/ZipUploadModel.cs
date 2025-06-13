using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace PromptEngineering.Models.CodeConversion
{
    public class ZipUploadModel
    {
        public IFormFile ZipFile { get ;set; }
        public string FileName { get; set; }
        public string FriendlyName { get; set; }
    }
}