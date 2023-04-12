using System;
using birds_shop_api.Models.BirdsShop;


namespace birds_shop_api.Utils
{
    public class SaveImage
    {
        public static string Save(string image, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, Convert.FromBase64String(image));
            }
            catch (Exception ex)
            {
                return "error";
            }
            return "done";


        }

    }
}
