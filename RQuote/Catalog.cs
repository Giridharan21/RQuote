using System;
using System.IO;

namespace RQuote
{
    public class Catalog
    {
        public string FilePath;
        public string FolderPath;
        public string ImageFolderPath
        {
            get
            {
                return String.IsNullOrEmpty(this.FolderPath) ? null : Path.Combine(this.FolderPath, "images");
            }
        }
        public string DisplayName
        {
            get
            {
                string value = "";
                if (!string.IsNullOrEmpty(this.FilePath))
                {
                    value = Path.GetFileNameWithoutExtension(this.FilePath);
                }
                return value;
            }
        }
    }
}
