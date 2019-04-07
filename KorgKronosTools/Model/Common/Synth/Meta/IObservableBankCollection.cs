using System.Collections.Generic;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservableBankCollection<T> : IList<T> where T:IBank
    {
    }
}
