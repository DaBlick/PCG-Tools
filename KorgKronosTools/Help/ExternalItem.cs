using PcgTools.Annotations;

namespace PcgTools.Help
{
    public class ExternalItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private string _bitmapPath;

        /// <summary>
        /// 
        /// </summary>
        public string BitmapPath
        {
            [UsedImplicitly] get { return _bitmapPath; }
            set { _bitmapPath = "/PcgTools;component/Help/External Links/" + value; }
        }
    }
}
