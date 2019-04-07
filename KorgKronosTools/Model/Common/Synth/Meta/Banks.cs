using System;
using System.Linq;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Banks<T> : ObservableObject, IBanks where T:IBank
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IPcgMemory _pcgMemory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected Banks(IPcgMemory pcgMemory)
        {
            BankCollection = new ObservableBankCollection<IBank>();
            _pcgMemory = pcgMemory;
        }


        /// <summary>
        /// 
        /// </summary>
        public IObservableBankCollection<IBank> BankCollection { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        protected void Add(T bank)
        {
            BankCollection.Add(bank);
        }


        /// <summary>
        /// 
        /// </summary>
        public int CountPatches { get { return BankCollection.Sum(bank => bank.CountPatches); } }


        /// <summary>
        /// 
        /// </summary>
        public int CountWritablePatches { get { return BankCollection.Sum(bank => bank.CountWritablePatches); } }


        /// <summary>
        /// 
        /// </summary>
        public virtual int CountFilledBanks { get { return BankCollection.Count(bank => bank.IsFilled); } }


        /// <summary>
        /// 
        /// </summary>
        public int CountFilledPatches { get { return BankCollection.Sum(bank => bank.CountFilledPatches); } }

        
        /// <summary>
        /// 
        /// </summary>
        public INavigable Parent => _pcgMemory.Root;


        /// <summary>
        /// 
        /// </summary>
        public IMemory Root => _pcgMemory.Root;


        /// <summary>
        /// 
        /// </summary>
        public int ByteOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int ByteLength
        {
            get { throw new NotSupportedException();} 
            set { throw new NotSupportedException();} 
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IBank this[int index] => BankCollection[index];


        /// <summary>
        /// Used in unit tests.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IBank this[string name]
        {
            get { return BankCollection.First(n => n.Id == name); }
        }


        /// <summary>
        /// 
        /// </summary>
        public int CountFilledAndNonEmptyPatches
        {
            get { return BankCollection.Sum(bank => bank.CountFilledAndNonEmptyPatches); }
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract void Fill();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgId"></param>
        /// <returns></returns>
        public virtual IBank GetBankWithPcgId(int pcgId)
        {
            return BankCollection.FirstOrDefault(bank => bank.PcgId == pcgId);
        }



        /// <summary>
        /// 
        /// </summary>
        public virtual int IndexOfBank(IBank bank)
        {
            for (var index = 0; index < BankCollection.Count; index++)
            {
                if (BankCollection[index] == bank)
                {
                    return index;
                }
            }

            throw new ApplicationException("Bank not found");
        }
    }
}
