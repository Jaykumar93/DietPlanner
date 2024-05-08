using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Upload 
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Upload(IWebHostEnvironment webHostEnvironment) 
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<string> UploadProfileImg(IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImages");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);


                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }


                return "/ProfileImages/" + uniqueFileName;
            }
            return "/ProfileImages/default-profile.png";
        }

        public async Task<string> UploadCertificate(IFormFile CertificateFile)
        {
            if (CertificateFile != null && CertificateFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "CertificateFile");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + CertificateFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await CertificateFile.CopyToAsync(fileStream);
                }


                return "/CertificateFile/" + uniqueFileName;
            }
            return "/CertificateFile/default-profile.png";
        }
    }
}
