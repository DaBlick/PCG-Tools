using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.ViewModels.Commands.PcgCommands
{
    public class ModelCompatibility
    {
        /// <summary>
        /// Returns true if two versions of workstation models are compatible.
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static bool AreOsVersionsCompatible(Models.EOsVersion version1, Models.EOsVersion version2)
        {
            return (version1 == version2) ||
                   IsTrinityVersionCompatible(version1, version2) ||
                   IsMicroKorgVersionCompatible(version1, version2) ||
                   IsKronosVersionCompatible(version1, version2);
                   // IsKrossVersionCompatible(version1, version2); // Samples are different
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        private static bool IsTrinityVersionCompatible(Models.EOsVersion version1, Models.EOsVersion version2)
        {
            return ((version1 == Models.EOsVersion.EOsVersionTrinityV2) ||
                    (version1 == Models.EOsVersion.EOsVersionTrinityV3)) &&
                   ((version2 == Models.EOsVersion.EOsVersionTrinityV2) ||
                    (version2 == Models.EOsVersion.EOsVersionTrinityV3));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        private static bool IsMicroKorgVersionCompatible(Models.EOsVersion version1, Models.EOsVersion version2)
        {
            return ((version1 == Models.EOsVersion.EOsVersionMicroKorgXl) ||
                    (version1 == Models.EOsVersion.EOsVersionMicroKorgXlPlus)) &&
                   ((version2 == Models.EOsVersion.EOsVersionMicroKorgXl) ||
                    (version2 == Models.EOsVersion.EOsVersionMicroKorgXlPlus));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        private static bool IsKronosVersionCompatible(Models.EOsVersion version1, Models.EOsVersion version2)
        {
            return ((version1 == Models.EOsVersion.EOsVersionKronos2x) ||
                    (version1 == Models.EOsVersion.EOsVersionKronos3x)) &&
                   ((version2 == Models.EOsVersion.EOsVersionKronos2x) ||
                    (version2 == Models.EOsVersion.EOsVersionKronos3x));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        private static bool IsKrossVersionCompatible(Models.EOsVersion version1, Models.EOsVersion version2)
        {
            return ((version1 == Models.EOsVersion.EOsVersionKross) ||
                    (version1 == Models.EOsVersion.EOsVersionKross2)) &&
                   ((version2 == Models.EOsVersion.EOsVersionKross) ||
                    (version2 == Models.EOsVersion.EOsVersionKross2));
        }


        /// <summary>
        /// Returns true if the two workstation models are compatible.
        /// </summary>
        /// <returns></returns>
        public static bool AreModelsCompatible(IModel model1, IModel model2)
        {
            // Models are compatible if they are equal or if both are either Triton Classic/Studio/Rack or Triton Extreme.
            return (model1.ModelType == model2.ModelType) ||
                   IsTritonModelCompatible(model1, model2) ||
                   IsMicroKorgModelCompatible(model1, model2);
                   // IsKrossModelCompatible(model1, model2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model1"></param>
        /// <param name="model2"></param>
        /// <returns></returns>
        private static bool IsMicroKorgModelCompatible(IModel model1, IModel model2)
        {
            return ((model1.ModelType == Models.EModelType.MicroKorgXl) ||
                    (model1.ModelType == Models.EModelType.MicroKorgXlPlus)) &&
                   ((model2.ModelType == Models.EModelType.MicroKorgXl) ||
                    (model2.ModelType == Models.EModelType.MicroKorgXlPlus));
        }


        /// <summary>
        /// Triton V2 and V3 are equal (i.e. program S and M program banks contain equal types of patches.
        /// </summary>
        /// <param name="model1"></param>
        /// <param name="model2"></param>
        /// <returns></returns>
        private static bool IsTritonModelCompatible(IModel model1, IModel model2)
        {
            return ((model1.ModelType == Models.EModelType.TritonTrClassicStudioRack) ||
                    (model1.ModelType == Models.EModelType.TritonExtreme)) &&
                   ((model2.ModelType == Models.EModelType.TritonTrClassicStudioRack) ||
                    (model2.ModelType == Models.EModelType.TritonExtreme));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model1"></param>
        /// <param name="model2"></param>
        /// <returns></returns>
        private static bool IsKrossModelCompatible(IModel model1, IModel model2)
        {
            return ((model1.ModelType == Models.EModelType.Kross) ||
                    (model1.ModelType == Models.EModelType.Kross2)) &&
                   ((model2.ModelType == Models.EModelType.Kross) ||
                    (model2.ModelType == Models.EModelType.Kross2));
        }
    }
}
