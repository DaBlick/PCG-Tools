using Common.Mvvm;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableBankCollection<T> : ObservableCollectionEx<T>, IObservableBankCollection<T> where T:IBank
    {
    }
}
