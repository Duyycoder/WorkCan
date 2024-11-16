using BaseApp.Constants;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BaseApp.Service
{

    public interface ICloudinaryService
    {
        Task<string> DeleteImageBySecureUrlAsync(string secureUrl);

        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
    }

    public class CloudinaryService : ICloudinaryService
    {

        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary) => _cloudinary = cloudinary;

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            // Validate file size
            if (file.Length < CommonConstants.MIN_FILE_SIZE_IN_BYTES)
            {
                uploadResult.Error = new Error { Message = "File's size is at least 1kb." };
                return uploadResult;
            }
            if (file.Length > CommonConstants.MAX_FILE_SIZE_IN_BYTES)
            {
                uploadResult.Error = new Error { Message = "File's size cannot be more than 10mb." };
                return uploadResult;
            }

            // Validate file type
            if (!CommonConstants.ALLOWED_FILE_TYPES.Contains(file.ContentType))
            {
                uploadResult.Error = new Error { Message = "File's type should only be image/jpeg, image/png, image/jpg, image/gif." };
                return uploadResult;
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Quality("auto").FetchFormat("auto"),
                Overwrite = true
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult;
        }

        public async Task<string> DeleteImageBySecureUrlAsync(string secureUrl)
        {
            try
            {
                string folderName = "WorkScan";
                Uri uri = new Uri(secureUrl);
                string path = uri.AbsolutePath;
                string publicId = path.Substring(path.IndexOf("folderName")).Replace($"/{folderName}/", "").Replace(".jpg", "").Replace(".png", "");

                var deletionParams = new DeletionParams($"{folderName}/{publicId}");
                var result = await _cloudinary.DestroyAsync(deletionParams);

                return result.Result; // == "ok"
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
