/*
// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


namespace PcgTools.Synths.Common.Synth
{
    public class GmProgram : Program
    {
        public override string Name { get; set; }
        public string CategoryName { get; private set; }
        public string SubCategoryName { get; private set; }
        
        public GmProgram(ProgramBank programBank, int index, string name, string categoryName, string subCategoryName)
            : base(programBank, index)
        {
            //Bank = Parent
            Name = name;
            CategoryName = categoryName;
            SubCategoryName = subCategoryName;
        }

        public override int MaxNameLength
        {
            get { return -1; }
        }

        public override bool IsEmptyOrInit
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This is utility class but contains all GM",    program names and (sub)",  categories.
    /// </summary>
    public abstract class GmPrograms
    {
        /// <summary>
        /// Make sure order is: 1) Index, 2) Bank
        /// </summary>
        static readonly List<GmProgram> GmList = new List<GmProgram>()
        {
            new GmProgram("GM",     1, "Acoustic Piano", "Keyboard", "A.Piano"),
            new GmProgram("g(1)",   1, "Acoustic Piano w", "Keyboard", "A.Piano"),
            new GmProgram("GM",     1, "Acoustic Piano", "Keyboard", "A.Piano"),
            new GmProgram("g(1)",   1, "Acoustic Piano w","Keyboard", "A.Piano"),
            new GmProgram("g(2)",   1, "Acoustic Piano", "Keyboard", "A.Piano"),
            new GmProgram("GM",     2, "Bright Piano", "Keyboard", "A.Piano"),
            new GmProgram("g(1)",   2, "Bright Piano w", "Keyboard", "A.Piano"),
            new GmProgram("GM",     3, "El.Grand Piano", "Keyboard", "Real E.Piano"),
            new GmProgram("g(1)",   3, "El.Grand Piano w", "Keyboard", "Real E.Piano"),
            new GmProgram("GM",     4, "Honkey-Tonk", "Keyboard", "A.Piano"),
            new GmProgram("g(1)",   4, "Honkey-Tonk w", "Keyboard", "A.Piano"),
            new GmProgram("GM",     5, "Electric Piano 1", "Keyboard", "Real E.Piano"),
            new GmProgram("g(1)",   5, "Detuned E.Piano1", "Keyboard", "Real E.Piano"),
            new GmProgram("g(2)",   5, "ElectricPiano1 v", "Keyboard", "Real E.Piano"),
            new GmProgram("g(3)",   5, "60's E.Piano", "Keyboard", "Real E.Piano"),
            new GmProgram("GM",     6, "Electric Piano 2", "Keyboard", "Synth E.Piano"),
            new GmProgram("g(1)",   6, "Detuned E.Piano2", "Keyboard", "Synth E.Piano"),
            new GmProgram("g(2)",   6, "ElectricPiano2 v", "Keyboard", "Synth E.Piano"),
            new GmProgram("g(3)",   6, "E.Piano Legend", "Keyboard", "Synth E.Piano"),
            new GmProgram("g(4)",   6, "E.Piano Phase", "Keyboard", "Synth E.Piano"),
            new GmProgram("GM",     7, "Harpsichord", "Keyboard", "Clav/Harpsi"),
            new GmProgram("g(1)",   7, "Harpsi. oct-mix", "Keyboard", "Clav/Harpsi"),
            new GmProgram("g(2)",   7, "Harpsichord w", "Keyboard", "Clav/Harpsi"),
            new GmProgram("g(3)",   7, "Harpsi. key-off", "Keyboard", "Clav/Harpsi"),
            new GmProgram("GM",     8, "Clavi.", "Keyboard", "Clav/Harpsi"),
            new GmProgram("g(1)",   8, "Pulse Clavi.", "Keyboard", "Clav/Harpsi"),
            new GmProgram("GM",     9, "Celesta", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    10, "Glockenspiel", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    11, "Music Box", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    12, "Vibraphone", "Bell/Mallet", "Mallet"),
            new GmProgram("g(1)",  12, "Vibraphone w", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    13, "Marimba", "Bell/Mallet", "Mallet"),
            new GmProgram("g(1)",  13, "Marimba w", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    14, "Xylophone", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    15, "Tubular Bells", "Bell/Mallet", "Bell"),
            new GmProgram("g(1)",  15, "Church Bell", "Bell/Mallet", "Bell"),
            new GmProgram("g(2)",  15, "Carillon", "Bell/Mallet", "Bell"),
            new GmProgram("GM",    16, "Santur", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    17, "Drawbar Organ 1 1", "Organ", "E.Organ"),
            new GmProgram("g(1)",  17, "Det.DrawbarOrgan", "Organ", "E.Organ"),
            new GmProgram("g(2)",  17, "Italian60'sOrgan", "Organ", "E.Organ"),
            new GmProgram("g(3)",  17, "Drawbar Organ 2", "Organ", "E.Organ"),
            new GmProgram("GM",    18, "PercussiveOrgan1", "Organ", "E.Organ"),
            new GmProgram("g(1)",  18, "Det.Perc-Organ", "Organ", "E.Organ"),
            new GmProgram("g(2)",  18, "PercussiveOrgan2", "Organ", "E.Organ"),
            new GmProgram("GM",    19, "Rock", "Organ", "Organ E.Organ"),
            new GmProgram("GM",    20, "Church", "Organ", "Organ Pipe Organ"),
            new GmProgram("g(1)",  20, "Church Org Oct.", "Organ", "Pipe Organ"),
            new GmProgram("g(2)",  20, "Det.Church Org", "Organ", "Pipe Organ"),
            new GmProgram("GM",    21, "Reed", "Organ", "Organ Pipe Organ"),
            new GmProgram("g(1)",  21, "Puff", "Organ", "Organ Pipe Organ"),
            new GmProgram("GM",    22, "Accordion 1", "Woodwind/Reed", "Reed"),
            new GmProgram("g(1)",  22, "Accordion 2", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    23, "Harmonica", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    24, "Bandoneon", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    25, "Nylon Guitar 1", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("g(1)",  25, "Ukulele", "Guitar/Plucked", "Plucked"),
            new GmProgram("g(2)",  25, "NylonGtr.key-off", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("g(3)",  25, "Nylon Guitar 2", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("GM",    26, "Steel Guitar", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("g(1)",  26, "12-StringsGuitar", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("g(2)",  26, "Mandolin", "Guitar/Plucked", "Plucked"),
            new GmProgram("g(3)",  26, "Steel Gtr. Body", "Guitar/Plucked", "A.Guitar"),
            new GmProgram("GM",    27, "Jazz Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  27, "Pedal Steel Gtr.", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    28, "Clean Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  28, "Detuned Clean Gt", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(2)",  28, "Mid Tone Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    29, "Muted Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  29, "Funky Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(2)",  29, "Muted Guitar v", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(3)",  29, "Jazz Man", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    30, "Overdrive Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  30, "Guitar Pinch", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    31, "DistortionGuitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  31, "Feedback Guitar", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(2)",  31, "Dist. Rhythm Gtr", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    32, "Guitar Harmonics", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)",  32, "Guitar Feedback", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("GM",    33, "Acoustic Bass", "Bass/Synth Bass", "A.Bass"),
            new GmProgram("GM",    34, "Fingered Bass", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("g(1)",  34, "Finger Slap Bass", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("GM",    35, "Picked Bass", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("GM",    36, "Fretless Bass", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("GM",    37, "Slap Bass 1", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("GM",    38, "Slap Bass 2", "Bass/Synth Bass", "E.Bass"),
            new GmProgram("GM",    39, "Synth Bass 1", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(1)",  39, "Warm Synth Bass", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(2)",  39, "Synth Bass 3", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(3)",  39, "Clavi Bass", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(4)",  39, "Hammer", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("GM",    40, "Synth Bass 2", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(1)",  40, "Synth Bass 4", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(2)",  40, "Rubber Bass", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("g(3)",  40, "Attack Pulse", "Bass/Synth Bass", "Synth Bass"),
            new GmProgram("GM",    41, "Violin", "Strings", "Solo"),
            new GmProgram("g(1)",  41, "Slow Violin", "Strings", "Solo"),
            new GmProgram("GM",    42, "Viola", "Strings", "Solo"),
            new GmProgram("GM",    43, "Cello", "Strings", "Solo"),
            new GmProgram("GM",    44, "Contrabass", "Strings", "Solo"),
            new GmProgram("GM",    45, "Tremolo", "Strings", "Strings Ensemble"),
            new GmProgram("GM",    46, "Pizzicato Str.", "Strings", "Ensemble"),
            new GmProgram("GM",    47, "Orchestral Harp", "Guitar/Plucked", "Plucked"),
            new GmProgram("g(1)",  47, "Yang Chin", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",    48, "Timpani", "Drums", "Perc"),
            new GmProgram("GM",    49, "Strings", "Strings", "Ensemble"),
            new GmProgram("g(1)",  49, "Strings & Brass", "Strings", "Ensemble"),
            new GmProgram("g(2)",  49, "60's", "Strings", "Strings Ensemble"),
            new GmProgram("GM",    50, "Slow", "Strings", "Strings Ensemble"),
            new GmProgram("GM",    51, "Synth Strings 1", "Strings", "Ensemble"),
            new GmProgram("g(1)",  51, "Synth Strings 3", "Strings", "Ensemble"),
            new GmProgram("GM",    52, "Synth Strings 2", "Strings", "Ensemble"),
            new GmProgram("GM",    53, "Choir Aahs 1", "Vocal/Airy", "Vocal"),
            new GmProgram("g(1)",  53, "Choir Aahs 2", "Vocal/Airy", "Vocal"),
            new GmProgram("GM",    54, "Voice Oohs", "Vocal/Airy", "Vocal"),
            new GmProgram("g(1)",  54, "Humming", "Vocal/Airy", "Vocal"),
            new GmProgram("GM",    55, "Synth Vox", "Vocal/Airy", "Vocal"),
            new GmProgram("g(1)",  55, "Analog Voice", "Vocal/Airy", "Vocal"),
            new GmProgram("GM",    56, "Orchestra Hit Short", "Decay/Hit", "Hit"),
            new GmProgram("g(1)",  56, "Bass Hit Plus Short", "Decay/Hit", "Hit"),
            new GmProgram("g(2)",  56, "6th Hit Short", "Decay/Hit", "Hit"),
            new GmProgram("g(3)",  56, "Euro Hit Short", "Decay/Hit", "Hit"),
            new GmProgram("GM",    57, "Trumpet", "Brass", "Solo"),
            new GmProgram("g(1)",  57, "Soft Trumpet", "Brass", "Solo"),
            new GmProgram("GM",    58, "Trombone 1", "Brass", "Solo"),
            new GmProgram("g(1)",  58, "Trombone 2", "Brass", "Solo"),
            new GmProgram("g(2)",  58, "Bright Trombone", "Brass", "Solo"),
            new GmProgram("GM",    59, "Tuba", "Brass", "Solo"),
            new GmProgram("GM",    60, "Muted Trumpet 1", "Brass", "Solo"),
            new GmProgram("g(1)",  60, "Muted Trumpet 2", "Brass", "Solo"),
            new GmProgram("GM",    61, "French Horn", "Brass", "Solo"),
            new GmProgram("g(1)",  61, "Warm French Horn", "Brass", "Solo"),
            new GmProgram("GM",    62, "Brass Section 1", "Brass", "Ensemble"),
            new GmProgram("g(1)",  62, "Brass Section 2", "Brass", "Ensemble"),
            new GmProgram("GM",    63, "Synth Brass 1", "FastSynth", "Short Release"),
            new GmProgram("g(1)",  63, "Synth Brass 3", "FastSynth", "Short Release"),
            new GmProgram("g(2)",  63, "Analog Brass 1", "FastSynth", "Short Release"),
            new GmProgram("g(3)",  63, "Jump Brass", "Fast Synth", "Short Release"),
            new GmProgram("GM",    64, "Synth Brass 2", "FastSynth", "Short Release"),
            new GmProgram("g(1)",  64, "Synth Brass 4", "FastSynth", "Long Release"),
            new GmProgram("g(2)",  64, "Analog Brass 2", "FastSynth", "Short Release"),
            new GmProgram("GM",    65, "Soprano Sax", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    66, "Alto Sax", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    67, "Tenor Sax", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    68, "Baritone Sax", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    69, "Oboe", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    70, "English Horn", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    71, "Bassoon", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    72, "Clarinet", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",    73, "Piccolo", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    74, "Flute", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    75, "Recorder", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    76, "Pan Flute", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    77, "Blown Bottle", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    78, "Shakuhachi", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    79, "Whistle", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    80, "Ocarina", "Woodwind/Reed", "Woodwind"),
            new GmProgram("GM",    81, "Detuned Square", "FastSynth", "Short Release"),
            new GmProgram("g(1)",  81, "Simple Square", "LeadSynth", "Soft"),
            new GmProgram("g(2)",  81, "Simple Sine", "LeadSynth", "Soft"),
            new GmProgram("GM",    82, "Detuned Sawtooth", "FastSynth", "Short Release"),
            new GmProgram("g(1)",  82, "Simple Sawtooth", "LeadSynth", "Hard"),
            new GmProgram("g(2)",  82, "Sawtooth + Pulse", "LeadSynth", "Hard"),
            new GmProgram("g(3)",  82, "Double Sawtooth", "LeadSynth", "Hard"),
            new GmProgram("g(4)",  82, "Sequenced Analog Short", "Decay/Hit", "Short Decay"),
            new GmProgram("GM",    83, "Synth Calliope", "FastSynth", "Short Release"),
            new GmProgram("GM",    84, "Chiff Lead", "FastSynth", "Short Release"),
            new GmProgram("GM",    85, "Charang", "LeadSynth", "Hard"),
            new GmProgram("g(1)",  85, "Wire Lead", "LeadSynth", "Hard"),
            new GmProgram("GM",    86, "Air Voice", "Vocal/Airy", "Vocal"),
            new GmProgram("GM",    87, "5th Sawtooth", "LeadSynth", "Hard"),
            new GmProgram("GM",    88, "Bass & Lead", "LeadSynth", "Hard"),
            new GmProgram("g(1)",  88, "Soft Wurl", "LeadSynth", "Soft"),
            new GmProgram("GM",    89, "Fantasia", "FastSynth", "Long Release"),
            new GmProgram("GM",    90, "Warm Pad", "SlowSynth", "Dark"),
            new GmProgram("g(1)",  90, "Sine Pad", "SlowSynth", "Dark"),
            new GmProgram("GM",    91, "Polyphonic Synth", "FastSynth", "Short Release"),
            new GmProgram("GM",    92, "Space Voice", "Vocal/Airy", "Airy"),
            new GmProgram("g(1)",  92, "Itopia", "Vocal/Airy", "Airy"),
            new GmProgram("GM",    93, "Bowed Glass", "SlowSynth", "Dark"),
            new GmProgram("GM",    94, "Metallic Pad", "SlowSynth", "Dark"),
            new GmProgram("GM",    95, "Halo Pad", "Vocal/Airy", "Airy"),
            new GmProgram("GM",    96, "Sweep Pad", "SlowSynth", "Sweep"),
            new GmProgram("GM",    97, "Ice Rain", "FastSynth", "Long Release"),
            new GmProgram("GM",    98, "Sound Track", "SlowSynth", "Bright"),
            new GmProgram("GM",    99, "Crystal", "Bell/Mallet", "Bell"),
            new GmProgram("g(1)",  99, "Synth Mallet", "FastSynth", "Long Release"),
            new GmProgram("GM",   100, "Atmosphere", "FastSynth", "Long Release"),
            new GmProgram("GM",   101, "Brightness", "FastSynth", "Long Release"),
            new GmProgram("GM",   102, "Goblins", "MotionSynth", "Motion"),
            new GmProgram("GM",   103, "Echo Drops", "FastSynth", "Long Release"),
            new GmProgram("g(1)", 103, "Echo Bell", "FastSynth", "Long Release"),
            new GmProgram("g(2)", 103, "Echo Pan", "FastSynth", "Long Release"),
            new GmProgram("GM",   104, "Star Theme", "FastSynth", "Long Release"),
            new GmProgram("GM",   105, "Sitar 1", "Guitar/Plucked", "Plucked"),
            new GmProgram("g(1)", 105, "Sitar 2", "Guitar/Plucked", "Plucked"),
            new GmProgram("GM",   106, "Banjo", "Guitar/Plucked", "Plucked"),
            new GmProgram("GM",   107, "Shamisen", "Guitar/Plucked", "Plucked"),
            new GmProgram("GM",   108, "Koto", "Guitar/Plucked", "Plucked"),
            new GmProgram("g(1)", 108, "Taisho Koto", "Guitar/Plucked", "Plucked"),
            new GmProgram("GM",   109, "Kalimba", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",   110, "Bagpipe", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",   111, "Fiddle", "Strings", "Solo"),
            new GmProgram("GM",   112, "Shanai", "Woodwind/Reed", "Reed"),
            new GmProgram("GM",   113, "Tinkle Bell", "Bell/Mallet", "Bell"),
            new GmProgram("GM",   114, "Agogo", "Drums", "Perc"),
            new GmProgram("GM",   115, "Steel Drums", "Bell/Mallet", "Mallet"),
            new GmProgram("GM",   116, "Woodblock", "Drums", "Perc"),
            new GmProgram("g(1)", 116, "Castanets", "Drums", "Perc"),
            new GmProgram("GM",   117, "Taiko", "Drums", "Perc"),
            new GmProgram("g(1)", 117, "Concert BassDrum", "Drums", "Perc"),
            new GmProgram("GM",   118, "Melodic Tom 1", "Drums", "Natural Drums"),
            new GmProgram("g(1)", 118, "Melodic Tom 2", "Drums", "Natural Drums"),
            new GmProgram("GM",   119, "Synth Drum", "Drums", "Dance Drums"),
            new GmProgram("g(1)", 119, "Analog Tom", "Drums", "Dance Drums"),
            new GmProgram("g(2)", 119, "Electric Drum", "Drums", "Dance Drums"),
            new GmProgram("GM",   120, "Reverse Cymbal", "SFX", "Synthetic"),
            new GmProgram("GM",   121, "Gtr.Fret Noise", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)", 121, "Gtr.CuttingNoise", "Guitar/Plucked", "E.Guitar"),
            new GmProgram("g(1)", 121, "Bass String Slap", "Bass/Synth Bass", "A.Bass"),
            new GmProgram("GM",   122, "Breath Noise", "Vocal/Airy", "Airy"),
            new GmProgram("g(1)", 122, "Flute Key Click", "SFX", "Natural"),
            new GmProgram("GM",   123, "Seashore", "SFX", "Natural"),
            new GmProgram("g(1)", 123, "Rain", "SFX", "Natural"),
            new GmProgram("g(2)", 123, "Thunder", "SFX", "Natural"),
            new GmProgram("g(3)", 123, "Wind", "SFX", "Natural"),
            new GmProgram("g(4)", 123, "Stream", "SFX", "Natural"),
            new GmProgram("g(5)", 123, "Bubble", "SFX", "Natural"),
            new GmProgram("GM",   124, "Bird Tweet 1", "SFX", "Natural"),
            new GmProgram("g(1)", 124, "Dog", "SFX", "Natural"),
            new GmProgram("g(2)", 124, "Horse Gallop", "SFX", "Natural"),
            new GmProgram("g(3)", 124, "Bird Tweet 2", "SFX", "Natural"),
            new GmProgram("GM",   125, "Telephone Ring 1", "SFX", "Natural"),
            new GmProgram("g(2)", 125, "Door Creaking", "SFX", "Natural"),
            new GmProgram("g(3)", 125, "Door Slamming", "SFX", "Natural"),
            new GmProgram("g(4)", 125, "Scratch", "SFX", "Natural"),
            new GmProgram("g(5)", 125, "Wind Chime", "Drums", "Perc"),
            new GmProgram("g(1)", 125, "Telephone Ring 2", "SFX", "Natural"),
            new GmProgram("GM",   126, "Helicopter", "SFX", "Natural"),
            new GmProgram("g(1)", 126, "Car Engine", "SFX", "Natural"),
            new GmProgram("g(2)", 126, "Car Stop", "SFX", "Natural"),
            new GmProgram("g(3)", 126, "Car Pass", "SFX", "Natural"),
            new GmProgram("g(4)", 126, "Car Crash", "SFX", "Natural"),
            new GmProgram("g(5)", 126, "Siren", "SFX", "Synthetic"),
            new GmProgram("g(6)", 126, "Train", "SFX", "Natural"),
            new GmProgram("g(7)", 126, "Jetplane", "SFX", "Synthetic"),
            new GmProgram("g(8)", 126, "Starship", "SFX", "Synthetic"),
            new GmProgram("g(9)", 126, "Burst Noise", "SFX", "Synthetic"),
            new GmProgram("GM",   127, "Applause", "SFX", "Natural"),
            new GmProgram("g(1)", 127, "Laughing", "SFX", "Natural"),
            new GmProgram("g(2)", 127, "Screaming", "SFX", "Natural"),
            new GmProgram("g(3)", 127, "Punch", "SFX", "Natural"),
            new GmProgram("g(4)", 127, "Heart Beat", "SFX", "Natural"),
            new GmProgram("g(5)", 127, "Footsteps", "SFX", "Natural"),
            new GmProgram("GM",   128, "Gun Shot", "SFX", "Natural"),
            new GmProgram("g(1)", 128, "Machine Gun", "SFX", "Natural"),
            new GmProgram("g(2)", 128, "Lasergun", "SFX", "Synthetic"),
            new GmProgram("g(3)", 128, "Explosion", "SFX", "Natural"),
            new GmProgram("g(d)",   1, "STANDARD Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",   9, "ROOM Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",  17, "POWER Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",  25, "ELECTRONIC Kit", "Drums", "Dance Drums"),
            new GmProgram("g(d)",  26, "ANALOG Kit", "Drums", "Dance Drums"),
            new GmProgram("g(d)",  33, "JAZZ Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",  41, "BRUSH Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",  49, "ORCHESTRA Kit", "Drums", "Natural Drums"),
            new GmProgram("g(d)",  57, "SFX Kit", "SFX", "Natural")
        };

        public string GetName(Program program)
        {
            Debug.Assert(((ProgramBank) program.Bank).ListSubType == ProgramBank.ListSubType.Gm);
            var gmProgram = FindGmProgram(program);
            return gmProgram.Name;
        }

        public string GetCategoryAsName(Program program)
        {
            Debug.Assert(((ProgramBank)program.Bank).ListSubType == ProgramBank.ListSubType.Gm);
            var gmProgram = FindGmProgram(program);
            return gmProgram.CategoryName;
        }

        public string GetSubCategoryAsName(Program program)
        {
            Debug.Assert(((ProgramBank)program.Bank).ListSubType == ProgramBank.ListSubType.Gm);
            var gmProgram = FindGmProgram(program);
            return gmProgram.SubCategoryName;
        }

        GmProgram FindGmProgram(Program program)
        {
            // Find program.
            var foundProgram = GmList.FirstOrDefault(gmProgram => (gmProgram.Index + 1 == program.Index) && (gmProgram.Bank == program.Bank));
            if (foundProgram == null)
            {
                // Search GM bank (assumed gm list is sorted by index, then by bank).
                foundProgram = GmList.FirstOrDefault(gmProgram => (gmProgram.Index + 1 == program.Index));
            }

            return foundProgram;
        }
    }
}
*/