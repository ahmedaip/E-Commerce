namespace E_Commerce_Project.Services
{
    public class BrandService
    {
        public string SaveFile(IFormFile ImageFile)
        {
            try
            {
                var filename = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images\\", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }

                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return null;
            }

        }


        public bool RemoveFile(string filename)
        {
            try
            {

                var oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_images\\", filename);

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return false;
            }
        }
    }
}
