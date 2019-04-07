// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Common.Mvvm;
using Common.Utils;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CombiViewModel : ViewModel, ICombiViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ICombi Combi { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        private IPcgViewModel PcgViewModel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgViewModel"></param>
        /// <param name="combi"></param>
        public CombiViewModel(IPcgViewModel pcgViewModel, ICombi combi)
        {
            PcgViewModel = pcgViewModel;

            Combi = combi;

            // Select first if none selected.
            if ((Combi.Timbres.TimbresCollection.Any()) && 
                (Combi.Timbres.TimbresCollection.Count(item => item.IsSelected) == 0))
            {
                Combi.Timbres.TimbresCollection[0].IsSelected = true;
            }

            Combi.PcgRoot.PropertyChanged += OnPcgRootChanged;
            ReassignClearProgram();
        }


        /// <summary>
        /// 
        /// </summary>
        public Action UpdateUiContent { get; set; }


        /// <summary>
        /// 
        /// </summary>
        ICommand _moveUpCommand;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public ICommand MoveUpCommand
        {
            get
            {
                return _moveUpCommand ?? (_moveUpCommand = new RelayCommand(param => MoveUp(), param => CanExecuteMoveUpCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteMoveUpCommand
        {
            get
            {
                return ((Combi.Timbres.TimbresCollection.Count(item => item.IsSelected) > 0) && 
                    !Combi.Timbres.TimbresCollection[0].IsSelected);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        void MoveUp()
        {
            foreach (var timbre in Combi.Timbres.TimbresCollection.Where(item => item.IsSelected))
            {
                var otherTimbre = ((ICombi)(timbre.Parent.Parent)).Timbres.TimbresCollection[timbre.Index - 1];
                timbre.Swap(otherTimbre);

                timbre.IsSelected = false;
                otherTimbre.IsSelected = true;
            }
            UpdateUiContent();
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _moveDownCommand;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public ICommand MoveDownCommand

        {
            get
            {
                return _moveDownCommand ?? (_moveDownCommand = new RelayCommand(
                    param => MoveDown(), param => CanExecuteMoveDownCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteMoveDownCommand
        {
            get
            {
                return ((Combi.Timbres.TimbresCollection.Count(item => item.IsSelected) > 0) &&
                    !Combi.Timbres.TimbresCollection[Combi.Timbres.TimbresCollection.Count - 1].IsSelected);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void MoveDown()
        {
            for (var timbreIndex = Combi.Timbres.TimbresCollection.Count - 1; timbreIndex >= 0; timbreIndex--)
            {
                var timbre = Combi.Timbres.TimbresCollection[timbreIndex];
                if (Combi.Timbres.TimbresCollection.Where(item => item.IsSelected).Contains(timbre))
                {
                    var otherTimbre = ((ICombi)(timbre.Parent.Parent)).Timbres.TimbresCollection[timbre.Index + 1];
                    otherTimbre.IsSelected = true;
                    timbre.IsSelected = false;

                    timbre.Swap(otherTimbre);
                }
            }
            UpdateUiContent();
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _clearCommand;

        
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(param => Clear(), param => CanExecuteClearCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteClearCommand
        {
            get
            {
                return (Combi.Timbres.TimbresCollection.Count(item => item.IsSelected) > 0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void Clear()
        {
            foreach (var timbre in Combi.Timbres.TimbresCollection.Where(item => item.IsSelected))
            {
                timbre.Clear();
            }

            UpdateUiContent();
        }
        

        public Func<bool> ShowEditDialog { private get; set; }

        ICommand _editCombiCommand;
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public ICommand EditCombiCommand
        {
            get
            {
                return _editCombiCommand ?? 
                    (_editCombiCommand = new RelayCommand(param => EditCombi(), param => CanExecuteEditCombiCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteEditCombiCommand
        {
            get
            {
                return (Combi.Timbres.TimbresCollection.Count(item => item.IsSelected) == 1);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void EditCombi()
        {
            ShowEditDialog();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public override bool Revert()
        //{
        //    return true;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exit"></param>
        /// <returns></returns>
        public override bool Close(bool exit)
        {
            CloseWindow();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        string _assignedClearProgram;
        [UsedImplicitly]
    // ReSharper disable once MemberCanBePrivate.Global
        public string AssignedClearProgram
        {
            // ReSharper disable once UnusedMember.Global
            get { return _assignedClearProgram; }
            set
            {
                if (_assignedClearProgram != value)
                {
                    _assignedClearProgram = value;
                    OnPropertyChanged("AssignedClearProgram");
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        void ReassignClearProgram()
        {
            var root = Combi.PcgRoot;
            var assignedClearProgram = root.AssignedClearProgram ?? root.ProgramBanks[0][0];
            AssignedClearProgram = $"{assignedClearProgram.Id} {assignedClearProgram.Name}";
        }


        [UsedImplicitly]
        void OnPcgRootChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AssignedClearProgram":
                    ReassignClearProgram();
                    break;

                case "IsDirty":
                    // No action required
                    break;

                case "FileName":
                    PcgViewModel.UpdateTimbresWindows();
                    break;

               // default: Ignore file name (and possibly more.
            }
        }
    }
}




/*
ICommand _cleanCommand;
// ReSharper disable UnusedMember.Global
[UsedImplicitly]
public ICommand CleanCommand
// ReSharper restore UnusedMember.Global
{
    get
    {
        return _cleanCommand ?? (_cleanCommand = new RelayCommand(param => Clean(), param => true));
    }
}
*/


/*
/// <summary>
/// Clean the combi by iterating through all timbres and check them against the settings for cleaning.
/// </summary>
void Clean()
{
    foreach (var timbre in Combi.Timbres.Where(item => item.IsSelected))
    {
        var clear = false;

        // Check MIDI channel.
        if (Settings.Default.CleanTimbres_WithMidiChannel)
        {
            var midiChannel = timbre.GetParam("MIDI Channel");
                    
            // Clear if the MIDI Channel GCh should be checked.
            clear |= (timbre.HasMidiChannelGch && ((Settings.Default.CleanTimbres_MidiChannels & 1) == 1)); // 1 = Gch

            for (var channel = 1; channel < 16; channel++)
            {
                if (channel == 10)
                {
                    // For MIDI channel 10, check if it has the drum category.
                    clear |= ((midiChannel.Value == 9) && // 9 == MIDI Channel 10
                              ((Settings.Default.CleanTimbres_MidiChannels & (1 << 10)) > 0) &&
                              (!Settings.Default.CleanTimbres_OnlyCleanTimbre10 || timbre.HasDrumCategory));
                }
                else
                {
                    // Treat MIDI channels 1..16 except 10 all equal.
                    clear |= ((midiChannel.Value == channel - 1) &&
                              ((Settings.Default.CleanTimbres_MidiChannels & (1 << channel)) > 0));
                }
            }
        }

        // Check Mute status.
        if (Settings.Default.CleanTimbres_WithMuteOn)
        {
            var mute = timbre.GetParam("Mute");

            clear |= (mute != null) && mute.Value;
        }

        // Check Status.
        if (Settings.Default.CleanTimbres_WithStatus)
        {
            var status = timbre.GetParam("Status").Value;
            clear |= (((status == "Off") && ((Settings.Default.CleanTimbres_Statusses & (1 << 0)) > 0)) ||
                      ((status == "Int") && ((Settings.Default.CleanTimbres_Statusses & (1 << 1)) > 0)) ||
                      ((status == "Ext") && ((Settings.Default.CleanTimbres_Statusses & (1 << 2)) > 0)) ||
                      ((status == "Ex2") && ((Settings.Default.CleanTimbres_Statusses & (1 << 3)) > 0)));
 Add On 
        }

        // Clear the timbre if at least one of the above conditions is true.
        if (clear)
        {
            timbre.Clear();
        }
    }

    UpdateUiContent();
}
 */


/*
ICommand _sortTimbresCommand;
// ReSharper disable UnusedMember.Global
[UsedImplicitly]
public ICommand SortTimbresCommand
// ReSharper restore UnusedMember.Global
{
    get
    {
        return _sortTimbresCommand ?? (_sortTimbresCommand = new RelayCommand(param => SortTimbres(), param => true));
    }
}

        
/// <summary>
/// SortMethod the timbres according to the settings of Timbre sorting.
/// </summary>
private void SortTimbres()
{
    var selectedTimbres = Combi.Timbres.Where(item => item.IsSelected).ToList();
    var allTimbresSelected = selectedTimbres.Patches.Count == Combi.Timbres.CountPatches;

    // Remove timbre 10 from the list to sort, if the setting selected and drum category.
    if (Settings.Default.SortTimbres_KeepTimbre10Fixed)
    {
        var timbre10 = Combi.Timbres.Patches.Count >= 10 ? Combi.Timbres[10] : null;
        if (timbre10 != null)
        {
            var timbre10Program = timbre10.UsedProgram;
            if ((timbre10Program != null) && (timbre10.GetParam("Status").Value == "Int") &&
                timbre10Program.IsDrumProgram)
            {
                selectedTimbres.Remove(timbre10);
            }
        }
    }

    // Define sort keys.
    var sortKeys = new List<TimbreSorting.ESortKey>();

    foreach (var sortKey in Settings.Default.SortTimbres_SortKeys)
    {
        switch (sortKey)
        {
            case 'M': // Mute
                sortKeys.Add(TimbreSorting.ESortKey.ESortKeyMute);
                break;

            case 'S':
                sortKeys.Add(TimbreSorting.ESortKey.ESortKeyStatus);
                break;

            case 'V': // Key velocity
                sortKeys.Add(TimbreSorting.ESortKey.ESortKeyKeyVelocity);
                break;

            case 'Z': // Key Zone
                sortKeys.Add(TimbreSorting.ESortKey.ESortKeyKeyKeyZone);
                break;

            case 'C': // MIDI Channel
                sortKeys.Add(TimbreSorting.ESortKey.ESortKeyMidiChannel);
                break;

            default:
                throw new ApplicationException("Illegal sort key");
        }
    }

    // SortMethod selected timbres by sort key.
    var sortedTimbres = selectedTimbres.ToList();
    TimbreSorting.SortBy(sortedTimbres, sortKeys);

    // If all timbres are selected and sort order is starting with Status, run the Gaps group box options.
    // FUTURE if ((allTimbresSelected) && (sortKeys[0] == TimbreSorting.ESortKey.ESortKeyStatus))
    //{
    //  InsertGaps(sortedTimbres);
    //}
}

        
/// <summary>
/// Inserts gaps according to the settings.
/// </summary>
/// <param name="timbres"></param>
private void InsertGaps(List<Timbre> timbres)
{
    // Total number of timbres, typically 8 or 16.
    var nrTimbres = timbres.Patches.Count(); 
            
    // CountPatches number of internal timbres
    var nrInt = timbres.Patches.Count(t => t.GetParam("Status").Value == "Int"); 
            
    // CountPatches number of external timbres.
    var nrExt = timbres.Patches.Count(                          
        t => (t.GetParam("status").Value == "Ext") ||
             (t.GetParam("status").Value == "Ex2"));

    // Create a relocation list list, false means the timbre should not be able to relocate.
    var relocationList = new bool[nrTimbres];
    for (var index = 0; index < relocationList.Length; index++)
    {
        relocationList[index] = true;
    }

    // Assume external timbres are sorted last; mark them as false.
    for (var index = nrTimbres - nrExt; index < nrTimbres; index++)
    {
        relocationList[index] = false;
    }

    // If timbre 10 is a drum program, keep it.
    if (Settings.Default.SortTimbres_KeepTimbre10Fixed)
    {
        var timbre10 = Combi.Timbres.Patches.Count >= 10 ? Combi.Timbres[10] : null;
        if (timbre10 != null)
        {
            var timbre10Program = timbre10.UsedProgram;
            if ((timbre10Program != null) && (timbre10.GetParam("Status").Value == "Int") &&
                timbre10Program.IsDrumProgram)
            {
                relocationList[10] = false;

                // Keep gaps around 10.
                if (Settings.Default.SortTimbres_KeepTimbreAround10Fixed)
                {
                    relocationList[9] = false;
                    relocationList[11] = false;
                }
            }
        }
    }

    // Keep gap before externals.
    if ((Settings.Default.SortTimbres_GapBeforeExternals) && (nrExt > 0))
    {
        relocationList[nrTimbres - nrExt - 1] = false;
    }

    // Every index in gapList that is True, should be checked for adding gaps.

    var gapList = CreateGapList(timbres, relocationList, nrInt);
    RemoveExcessGaps(timbres, gapList);
    var destinationList = CreateDestinationList(timbres, gapList);
    MoveTimbres(timbres, destinationList);
}


/// <summary>
/// Creates a list with available spaces. Each element in the gap list means the number of gaps to be added after that index.
/// </summary>
/// <param name="timbres"></param>
/// <param name="availableSpaces"></param>
/// <param name="nrInt"></param>
private int[] CreateGapList(IEnumerable<Timbre> timbres, bool[] relocationList, int nrInt)
{
    var gapList = new int[relocationList.Length];

    var filledSpaces = gapList.Patches.Count(t => t); // CountPatches true's 

    while (nr_int + filledSpaces > usableSpaces)

    return gapList;
}


org_gaps = gap_list.clone()

usable_spaces = gap_list: number of True’s

while (nr_int + gap_list.sum > usable_spaces)

indices = indices with max value in gap_list

if indices.count >  0 -> index in org_gaps   …. Take care that e.g. midi channel gaps are not reduced to 2 terefor->org_gap

 

gap_list[last index with max value]--

    

    

def create_destination_list()

dest = new list<nr_timbres>()

dest_index = 0

for (int i = 0; i < nr_timbres; i++)

# Skip unavailables

while !available_spaces[i]

dest_index++

       

if timbres[i] == 'int'

dest[dest_index] = i

dest_index++

 

assert dest_index <= nr_timbres - nr_ext (?)

    

def move_timbres()

for i = dest.count - 1; i >= 0; i--) #iterate backwards

if i != dest[i]

move_timbre(i, dest[i])

       








    // Create a new list with the number of gaps requested.
        var gapsBetweenMidiChannels = Settings.Default.SortTimbres_GapsBetweenMidiChannels;
        var gapsBetweenKeyZones = Settings.Default.SortTimbres_GapsBetweenKeyZones;
        var fixedGaps = Settings.Default.SortTimbres_FixedGaps;

        var gapsTimbres = sortedTimbres.ToList();
        Timbre previousTimbre = null;

        // Add gaps (only when first sorting key is Status.
        if (Settings.Default.SortTimbres_SortKeys.StartsWith("S"))
        {
            foreach (var timbre in sortedTimbres)
            {
                // Add timbre.
                gapsTimbres.Add(timbre);

                // CountPatches gaps.
                var gaps = 0;
                if (previousTimbre != null)
                {
                    gaps = fixedGaps ?
                               Math.Max(
                                   timbre.GetParam("MIDI Channel") == previousTimbre.GetParam("MIDI Channel")
                                       ? 0 : gapsBetweenMidiChannels,
                                   ((timbre.GetParam("Key Top") == previousTimbre.GetParam("Key Top")) &&
                                    (timbre.GetParam("Key Top") == previousTimbre.GetParam("Key Top")))
                                       ? 0 : gapsBetweenKeyZones
                                   ) : 16; // Use 16 gaps when dynamic gaps is used
                }

                // Add gaps.
                for (var gap = 0; gap < gaps; gap++)
                {
                    gapsTimbres.Add(null);
                }

                previousTimbre = timbre;
            }

            // Remove excess of gaps.
            var finished = false;
            while (!finished)
            {

                finished = (((timbre10 == null) && (gapsTimbres.Patches.Count < Combi.Timbres.CountPatches)) ||
                            ((timbre10 != null) && (gapsTimbres.Patches.Count + 1 < Combi.Timbres.CountPatches)));

            }
        }

        // If timbre 10 is removed, it needs to be added on index 10 again.
        if (timbre10 != null)
        {
            //
        }
    }









    // Move all timbres.
    /*
    for (var index = 0; index < selectedTimbres.Patches.Count; index++)
    {
        var destinationIndex = selectedTimbres.FindIndex(0, timbre => timbre == destination[index]);

        var destination1Index = destination.First(timbre => timbre == selectedTimbres[index]).Index;
        var destination2Index = destination.First(timbre => timbre == selectedTimbres[destinationIndex]).Index;

        if (destination1Index != destination2Index)
        {
            selectedTimbres[index].Swap(selectedTimbres[destinationIndex]);

            var temp = destination[destination1Index];
            destination[destination1Index] = destination[destination2Index];
            destination[destination2Index] = temp;
        }
    }
            

    // Make indices from 0..end again.
    for (var index = start; index < selectedTimbres.Patches.Count; index++)
    {
        destination[index].Index = index;
    }
            
         
    UpdateUiContent();
}
         
 */