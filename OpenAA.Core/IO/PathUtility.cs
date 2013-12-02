namespace OpenAA.IO
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class PathUtility
    {
        /// <summary>
        /// アプリケーションのデータ保存用ディレクトリを取得する。
        /// ApplicationData + Company + Product
        /// </summary>
        /// <returns>The data path.</returns>
        public static string GetDataPath()
        {
            var companyName = "__null__";
            var productName = "__null__";

            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
            {
                asm = Assembly.GetExecutingAssembly();
                if (asm == null)
                {
                }
            }

            if (asm != null)
            {
                var companyAttr = (System.Reflection.AssemblyCompanyAttribute)Attribute
                    .GetCustomAttribute(asm, typeof(System.Reflection.AssemblyCompanyAttribute));
                if (null != companyAttr && !string.IsNullOrEmpty(companyAttr.Company))
                {
                    companyName = companyAttr.Company;
                }

                var productAttr = (System.Reflection.AssemblyProductAttribute)Attribute
                    .GetCustomAttribute(asm, typeof(System.Reflection.AssemblyProductAttribute));
                if (null != productAttr && !string.IsNullOrEmpty(productAttr.Product))
                {
                    productName = productAttr.Product;
                }
            }

            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                companyName + Path.DirectorySeparatorChar + productName);

            return path;
        }
    }
}

