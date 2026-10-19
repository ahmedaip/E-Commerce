namespace E_Commerce_Project.Services
{
    public enum ProductImageType
    {
        MainImg,
        SubImage
    }
    public class ProductService
    {
        public string SaveFile(IFormFile ImageFile, ProductImageType productImageType = ProductImageType.MainImg)
        {
            try
            {
                var filepath = "";
                var filename = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                if (productImageType == ProductImageType.MainImg)
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\", filename);
                }

                else if (productImageType == ProductImageType.SubImage)
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images\\", filename);

                }
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


        public bool RemoveFile(string filename, ProductImageType productImageType = ProductImageType.MainImg)
        {
            try
            {
                var oldpath = "";
                if (productImageType == ProductImageType.MainImg)
                {

                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\", filename);
                }
                else if (productImageType == ProductImageType.SubImage)
                {
                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images\\", filename);
                }

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
