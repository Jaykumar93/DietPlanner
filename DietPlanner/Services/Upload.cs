using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


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

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + CertificateFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await CertificateFile.CopyToAsync(fileStream);
                }


                return "/CertificateFile/" + uniqueFileName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> UploadMealImage(IFormFile mealImage)
        {
            if (mealImage != null && mealImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "MealImage");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);


                var uniqueFileName = Guid.NewGuid().ToString() + "_" + mealImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await mealImage.CopyToAsync(fileStream);
                }
                return "/MealImage/" + uniqueFileName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> UploadPlanImage(IFormFile planImage)
        {
            if (planImage != null && planImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "planImage");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);


                var uniqueFileName = Guid.NewGuid().ToString() + "_" + planImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await planImage.CopyToAsync(fileStream);
                }
                return "/planImage/" + uniqueFileName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> UploadRewardImage(IFormFile rewardImage)
        {
            if (rewardImage != null && rewardImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "rewardImage");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);


                var uniqueFileName = Guid.NewGuid().ToString() + "_" + rewardImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await rewardImage.CopyToAsync(fileStream);
                }
                return "/rewardImage/" + uniqueFileName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> UploadChallengeImage(IFormFile challengeImage)
        {
            if (challengeImage != null && challengeImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "challengeImage");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);


                var uniqueFileName = Guid.NewGuid().ToString() + "_" + challengeImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await challengeImage.CopyToAsync(fileStream);
                }
                return "/challengeImage/" + uniqueFileName;
            }
            else
            {
                return null;
            }
        }
    }
}
