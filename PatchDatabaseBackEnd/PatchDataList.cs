using System.Collections.Generic;
using System.Text;

namespace PatchDatabaseBackEnd
{
    /// <summary>
    /// 
    /// </summary>
    public class PatchDataList
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PatchData> PatchList { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public PatchDataList()
        {
            PatchList = new List<PatchData>();
        }


        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var patch in PatchList)
            {
                builder.AppendLine($"{patch.PatchName}: {patch.Author}, {patch.Description}");
            }

            return builder.ToString();
        }
    }
}
